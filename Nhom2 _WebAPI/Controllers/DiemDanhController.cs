using Microsoft.AspNetCore.Mvc;
using Nhom2__WebAPI.Models;
using Nhom2__WebAPI.Models.DTOs;
using Nhom2__WebAPI.Repositories;

namespace Nhom2__WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DiemDanhController : ControllerBase
    {
        private readonly IDiemDanhRepository _diemDanhRepository;

        public DiemDanhController(IDiemDanhRepository diemDanhRepository)
        {
            _diemDanhRepository = diemDanhRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetDiemDanhs()
        {
            try
            {
                var records = await _diemDanhRepository.GetDiemDanhsAsync();
                return Ok(records);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Lỗi máy chủ", Error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetDiemDanhById(int id)
        {
            try
            {
                var record = await _diemDanhRepository.GetDiemDanhByIdAsync(id);
                if (record == null)
                    return NotFound(new { Message = $"Không tìm thấy bản ghi điểm danh Id {id}" });
                return Ok(record);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Lỗi máy chủ", Error = ex.Message });
            }
        }

        /// <summary>
        /// Lấy toàn bộ danh sách điểm danh của một buổi học
        /// </summary>
        [HttpGet("by-buoi/{buoiDiemDanhId}")]
        public async Task<IActionResult> GetByBuoi(int buoiDiemDanhId)
        {
            try
            {
                var records = await _diemDanhRepository.GetDiemDanhByBuoiAsync(buoiDiemDanhId);
                return Ok(records);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Lỗi máy chủ", Error = ex.Message });
            }
        }

        /// <summary>
        /// Sinh viên xem lịch sử điểm danh của mình
        /// </summary>
        [HttpGet("by-sinh-vien/{sinhVienId}")]
        public async Task<IActionResult> GetBySinhVien(int sinhVienId)
        {
            try
            {
                var records = await _diemDanhRepository.GetDiemDanhBySinhVienAsync(sinhVienId);
                return Ok(records);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Lỗi máy chủ", Error = ex.Message });
            }
        }

        /// <summary>
        /// Chức năng Check-in Điểm danh (Hỗ trợ QR Code, GPS Vị trí - 5b, Vân tay - 5e)
        /// </summary>
        [HttpPost("check-in")]
        public async Task<IActionResult> CheckIn([FromBody] CheckInRequestDto request)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);

                var result = await _diemDanhRepository.CheckInAsync(request);
                if (!result.ThanhCong)
                {
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Lỗi máy chủ khi điểm danh", Error = ex.Message });
            }
        }

        /// <summary>
        /// Giảng viên / Admin cập nhật trạng thái điểm danh thủ công (Có mặt, Vắng, Có phép, Trễ)
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDiemDanh(int id, [FromBody] DiemDanh diemDanh)
        {
            try
            {
                if (id != diemDanh.Id)
                    return BadRequest(new { Message = "Id không khớp với bản ghi cần sửa." });

                var existing = await _diemDanhRepository.GetDiemDanhByIdAsync(id);
                if (existing == null)
                    return NotFound(new { Message = $"Không tìm thấy bản ghi điểm danh Id {id}" });

                await _diemDanhRepository.UpdateDiemDanhAsync(diemDanh);
                return Ok(new { Message = "Cập nhật bản ghi điểm danh thành công", Data = diemDanh });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Lỗi máy chủ", Error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDiemDanh(int id)
        {
            try
            {
                var existing = await _diemDanhRepository.GetDiemDanhByIdAsync(id);
                if (existing == null)
                    return NotFound(new { Message = $"Không tìm thấy bản ghi điểm danh Id {id}" });

                await _diemDanhRepository.DeleteDiemDanhAsync(id);
                return Ok(new { Message = "Xóa bản ghi điểm danh thành công." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Lỗi máy chủ", Error = ex.Message });
            }
        }
    }
}
