using Microsoft.AspNetCore.Mvc;
using Nhom2__WebAPI.Models;
using Nhom2__WebAPI.Repositories;

namespace Nhom2__WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SinhVienController : ControllerBase
    {
        private readonly ISinhVienRepository _sinhVienRepository;

        public SinhVienController(ISinhVienRepository sinhVienRepository)
        {
            _sinhVienRepository = sinhVienRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetSinhViens()
        {
            try
            {
                var sinhViens = await _sinhVienRepository.GetSinhViensAsync();
                return Ok(sinhViens);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Lỗi máy chủ", Error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetSinhVienById(int id)
        {
            try
            {
                var sinhVien = await _sinhVienRepository.GetSinhVienByIdAsync(id);
                if (sinhVien == null)
                    return NotFound(new { Message = $"Không tìm thấy sinh viên có Id {id}" });
                return Ok(sinhVien);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Lỗi máy chủ", Error = ex.Message });
            }
        }

        [HttpGet("by-masv/{maSv}")]
        public async Task<IActionResult> GetSinhVienByMaSV(string maSv)
        {
            try
            {
                var sinhVien = await _sinhVienRepository.GetSinhVienByMaSVAsync(maSv);
                if (sinhVien == null)
                    return NotFound(new { Message = $"Không tìm thấy sinh viên có MSSV {maSv}" });
                return Ok(sinhVien);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Lỗi máy chủ", Error = ex.Message });
            }
        }

        [HttpGet("by-user/{userId}")]
        public async Task<IActionResult> GetSinhVienByUserId(string userId)
        {
            try
            {
                var sinhVien = await _sinhVienRepository.GetSinhVienByUserIdAsync(userId);
                if (sinhVien == null)
                    return NotFound(new { Message = $"Không tìm thấy sinh viên liên kết với UserId {userId}" });
                return Ok(sinhVien);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Lỗi máy chủ", Error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddSinhVien([FromBody] SinhVien sinhVien)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);

                var exists = await _sinhVienRepository.GetSinhVienByMaSVAsync(sinhVien.MaSV);
                if (exists != null)
                {
                    return BadRequest(new { Message = $"Mã sinh viên {sinhVien.MaSV} đã tồn tại trong hệ thống." });
                }

                await _sinhVienRepository.AddSinhVienAsync(sinhVien);
                return CreatedAtAction(nameof(GetSinhVienById), new { id = sinhVien.Id }, sinhVien);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Lỗi máy chủ", Error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSinhVien(int id, [FromBody] SinhVien sinhVien)
        {
            try
            {
                if (id != sinhVien.Id)
                    return BadRequest(new { Message = "Id không khớp với đối tượng cần cập nhật." });

                var existing = await _sinhVienRepository.GetSinhVienByIdAsync(id);
                if (existing == null)
                    return NotFound(new { Message = $"Không tìm thấy sinh viên có Id {id}" });

                await _sinhVienRepository.UpdateSinhVienAsync(sinhVien);
                return Ok(new { Message = "Cập nhật sinh viên thành công", Data = sinhVien });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Lỗi máy chủ", Error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSinhVien(int id)
        {
            try
            {
                var existing = await _sinhVienRepository.GetSinhVienByIdAsync(id);
                if (existing == null)
                    return NotFound(new { Message = $"Không tìm thấy sinh viên có Id {id}" });

                await _sinhVienRepository.DeleteSinhVienAsync(id);
                return Ok(new { Message = "Xóa sinh viên thành công." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Lỗi máy chủ", Error = ex.Message });
            }
        }
    }
}
