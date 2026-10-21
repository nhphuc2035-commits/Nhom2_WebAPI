using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Nhom2__WebAPI.Helpers;
using Nhom2__WebAPI.Models;
using Nhom2__WebAPI.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Lắng nghe trên cả 2 cổng 5145 và 5000 cho mọi nguồn
builder.WebHost.UseUrls("http://0.0.0.0:5145", "http://0.0.0.0:5000");

// 1. Cấu hình: Dùng SQL Server làm hệ quản trị CSDL cho ứng dụng
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 2. Cấu hình Identity
builder.Services.AddIdentity<User, IdentityRole>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
    options.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// 3. Đăng ký các Repositories (Repository Pattern theo hướng dẫn giảng viên)
builder.Services.AddScoped<ISinhVienRepository, SinhVienRepository>();
builder.Services.AddScoped<IMonHocRepository, MonHocRepository>();
builder.Services.AddScoped<ILopHocPhanRepository, LopHocPhanRepository>();
builder.Services.AddScoped<IBuoiDiemDanhRepository, BuoiDiemDanhRepository>();
builder.Services.AddScoped<IDiemDanhRepository, DiemDanhRepository>();
builder.Services.AddScoped<IDonNghiPhepRepository, DonNghiPhepRepository>();

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });

// 4. Cấu hình CORS mở cho tất cả nguồn (hỗ trợ Conveyor, điện thoại, máy ảo, web)
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "MyAllowOrigins", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// 5. Cấu hình xác thực JWT Authentication
var jwtSettings = builder.Configuration.GetSection("JWTKey");
var key = Encoding.UTF8.GetBytes(jwtSettings["Secret"]!);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["ValidIssuer"],
        ValidAudience = jwtSettings["ValidAudience"],
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };
});

builder.Services.AddAuthorization();

// 6. Cấu hình Swagger với hỗ trợ xác thực JWT Bearer
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Nhom2_WebAPI - Hệ Thống Điểm Danh Sinh Viên",
        Version = "v1",
        Description = "API phục vụ ứng dụng di động điểm danh sinh viên bằng mã QR, vị trí GPS và nhận diện sinh trắc học."
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Nhập token JWT theo định dạng: Bearer {token}",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

// Hỗ trợ Reverse Proxy / Conveyor khi truy cập từ ngoài Internet hoặc mạng LAN
app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto | ForwardedHeaders.XForwardedHost
});

// Khởi tạo và nạp dữ liệu mẫu ban đầu (Roles, Admin, Môn học, Lớp học phần, Sinh viên)
using (var scope = app.Services.CreateScope())
{
    try
    {
        await DbInitializer.SeedAsync(app.Services);
    }
    catch (Exception ex)
    {
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Lỗi khi tự động khởi tạo dữ liệu mẫu ban đầu.");
    }
}

// Luôn bật Swagger UI để truy cập được cả trên máy cá nhân, Conveyor và điện thoại
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Nhom2_WebAPI v1");
    c.RoutePrefix = string.Empty; // Mở Swagger ngay tại trang chủ (http://localhost:5145/)
});

// Tự động chuyển hướng từ /swagger về trang chủ Swagger UI
app.MapGet("/swagger", () => Results.Redirect("/"));

// LƯU Ý KHI DÙNG CONVEYOR / PUBLIC IP: 
// Không bật UseHttpsRedirection() vì Conveyor chuyển tiếp HTTP, nếu ép HTTPS sẽ bị lỗi Connection Refused trên điện thoại
// app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseCors("MyAllowOrigins");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
