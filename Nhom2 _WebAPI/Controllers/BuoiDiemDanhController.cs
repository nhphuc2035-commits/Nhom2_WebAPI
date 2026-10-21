using Microsoft.AspNetCore.Mvc;
using Nhom2__WebAPI.Models;
using Nhom2__WebAPI.Repositories;

namespace Nhom2__WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BuoiDiemDanhController : ControllerBase
    {
        private readonly IBuoiDiemDanhRepository _buoiDiemDanhRepository;

        public BuoiDiemDanhController(IBuoiDiemDanhRepository buoiDiemDanhRepository)
        {
            _buoiDiemDanhRepository = buoiDiemDanhRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetBuoiDiemDanhs()
        {
            try
            {
                var buois = await _buoiDiemDanhRepository.GetBuoiDiemDanhsAsync();
                return Ok(buois);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Lỗi máy chủ", Error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetBuoiDiemDanhById(int id)
        {
            try
            {
                var buoi = await _buoiDiemDanhRepository.GetBuoiDiemDanhByIdAsync(id);
                if (buoi == null)
                    return NotFound(new { Message = $"Không tìm thấy buổi điểm danh có Id {id}" });
                return Ok(buoi);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Lỗi máy chủ", Error = ex.Message });
            }
        }

        [HttpGet("by-lophocphan/{lopHocPhanId}")]
        public async Task<IActionResult> GetBuoiDiemDanhByLop(int lopHocPhanId)
        {
            try
            {
                var buois = await _buoiDiemDanhRepository.GetBuoiDiemDanhByLopHocPhanAsync(lopHocPhanId);
                return Ok(buois);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Lỗi máy chủ", Error = ex.Message });
            }
        }

        /// <summary>
        /// Tạo buổi điểm danh mới (kèm tọa độ GPS phòng học và tự sinh mã QR Code)
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> AddBuoiDiemDanh([FromBody] BuoiDiemDanh buoi)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);

                await _buoiDiemDanhRepository.AddBuoiDiemDanhAsync(buoi);
                return CreatedAtAction(nameof(GetBuoiDiemDanhById), new { id = buoi.Id }, buoi);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Lỗi máy chủ", Error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBuoiDiemDanh(int id, [FromBody] BuoiDiemDanh buoi)
        {
            try
            {
                if (id != buoi.Id)
                    return BadRequest(new { Message = "Id không khớp với buổi điểm danh cần cập nhật." });

                var existing = await _buoiDiemDanhRepository.GetBuoiDiemDanhByIdAsync(id);
                if (existing == null)
                    return NotFound(new { Message = $"Không tìm thấy buổi điểm danh có Id {id}" });

                await _buoiDiemDanhRepository.UpdateBuoiDiemDanhAsync(buoi);
                return Ok(new { Message = "Cập nhật buổi điểm danh thành công", Data = buoi });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Lỗi máy chủ", Error = ex.Message });
            }
        }

        /// <summary>
        /// Mở / Đóng buổi điểm danh (Chuyển đổi DangMo <=> DaDong)
        /// </summary>
        [HttpPut("{id}/dong-mo")]
        public async Task<IActionResult> ToggleTrangThai(int id)
        {
            try
            {
                var success = await _buoiDiemDanhRepository.ToggleTrangThaiAsync(id);
                if (!success)
                    return NotFound(new { Message = $"Không tìm thấy buổi điểm danh có Id {id}" });

                var updated = await _buoiDiemDanhRepository.GetBuoiDiemDanhByIdAsync(id);
                return Ok(new { Message = $"Đã cập nhật trạng thái buổi điểm danh sang: {updated?.TrangThai}", TrangThai = updated?.TrangThai });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Lỗi máy chủ", Error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBuoiDiemDanh(int id)
        {
            try
            {
                var existing = await _buoiDiemDanhRepository.GetBuoiDiemDanhByIdAsync(id);
                if (existing == null)
                    return NotFound(new { Message = $"Không tìm thấy buổi điểm danh có Id {id}" });

                await _buoiDiemDanhRepository.DeleteBuoiDiemDanhAsync(id);
                return Ok(new { Message = "Xóa buổi điểm danh thành công." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Lỗi máy chủ", Error = ex.Message });
            }
        }
    }
}
