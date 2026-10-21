using Microsoft.AspNetCore.Mvc;
using Nhom2__WebAPI.Models;
using Nhom2__WebAPI.Repositories;

namespace Nhom2__WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DonNghiPhepController : ControllerBase
    {
        private readonly IDonNghiPhepRepository _donNghiPhepRepository;

        public DonNghiPhepController(IDonNghiPhepRepository donNghiPhepRepository)
        {
            _donNghiPhepRepository = donNghiPhepRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetDonNghiPheps()
        {
            try
            {
                var dons = await _donNghiPhepRepository.GetDonNghiPhepsAsync();
                return Ok(dons);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Lỗi máy chủ", Error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetDonNghiPhepById(int id)
        {
            try
            {
                var don = await _donNghiPhepRepository.GetDonNghiPhepByIdAsync(id);
                if (don == null)
                    return NotFound(new { Message = $"Không tìm thấy đơn nghỉ phép Id {id}" });
                return Ok(don);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Lỗi máy chủ", Error = ex.Message });
            }
        }

        [HttpGet("by-sinh-vien/{sinhVienId}")]
        public async Task<IActionResult> GetBySinhVien(int sinhVienId)
        {
            try
            {
                var dons = await _donNghiPhepRepository.GetDonNghiPhepBySinhVienAsync(sinhVienId);
                return Ok(dons);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Lỗi máy chủ", Error = ex.Message });
            }
        }

        /// <summary>
        /// Sinh viên nộp đơn xin nghỉ phép kèm minh chứng
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> AddDonNghiPhep([FromBody] DonNghiPhep donNghiPhep)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);

                donNghiPhep.NgayGui = DateTime.UtcNow;
                donNghiPhep.TrangThai = "ChoDuyet";

                await _donNghiPhepRepository.AddDonNghiPhepAsync(donNghiPhep);
                return CreatedAtAction(nameof(GetDonNghiPhepById), new { id = donNghiPhep.Id }, donNghiPhep);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Lỗi máy chủ", Error = ex.Message });
            }
        }

        /// <summary>
        /// Giảng viên / Admin duyệt hoặc từ chối đơn nghỉ phép
        /// </summary>
        [HttpPut("{id}/duyet")]
        public async Task<IActionResult> DuyetDon(int id, [FromBody] DuyetDonModel model)
        {
            try
            {
                var success = await _donNghiPhepRepository.DuyetDonNghiPhepAsync(id, model.TrangThai, model.PhanHoi);
                if (!success)
                    return NotFound(new { Message = $"Không tìm thấy đơn nghỉ phép Id {id}" });

                return Ok(new { Message = $"Đã cập nhật trạng thái đơn thành: {model.TrangThai}" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Lỗi máy chủ", Error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDonNghiPhep(int id)
        {
            try
            {
                var existing = await _donNghiPhepRepository.GetDonNghiPhepByIdAsync(id);
                if (existing == null)
                    return NotFound(new { Message = $"Không tìm thấy đơn nghỉ phép Id {id}" });

                await _donNghiPhepRepository.DeleteDonNghiPhepAsync(id);
                return Ok(new { Message = "Xóa đơn nghỉ phép thành công." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Lỗi máy chủ", Error = ex.Message });
            }
        }
    }

    public class DuyetDonModel
    {
        // "DaDuyet", "TuChoi"
        public string TrangThai { get; set; } = "DaDuyet";
        public string? PhanHoi { get; set; }
    }
}
