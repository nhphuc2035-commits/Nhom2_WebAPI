using Microsoft.AspNetCore.Mvc;
using Nhom2__WebAPI.Models;
using Nhom2__WebAPI.Repositories;

namespace Nhom2__WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LopHocPhanController : ControllerBase
    {
        private readonly ILopHocPhanRepository _lopHocPhanRepository;

        public LopHocPhanController(ILopHocPhanRepository lopHocPhanRepository)
        {
            _lopHocPhanRepository = lopHocPhanRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetLopHocPhans()
        {
            try
            {
                var lopHocPhans = await _lopHocPhanRepository.GetLopHocPhansAsync();
                return Ok(lopHocPhans);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Lỗi máy chủ", Error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetLopHocPhanById(int id)
        {
            try
            {
                var lopHocPhan = await _lopHocPhanRepository.GetLopHocPhanByIdAsync(id);
                if (lopHocPhan == null)
                    return NotFound(new { Message = $"Không tìm thấy lớp học phần có Id {id}" });
                return Ok(lopHocPhan);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Lỗi máy chủ", Error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddLopHocPhan([FromBody] LopHocPhan lopHocPhan)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);

                await _lopHocPhanRepository.AddLopHocPhanAsync(lopHocPhan);
                return CreatedAtAction(nameof(GetLopHocPhanById), new { id = lopHocPhan.Id }, lopHocPhan);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Lỗi máy chủ", Error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateLopHocPhan(int id, [FromBody] LopHocPhan lopHocPhan)
        {
            try
            {
                if (id != lopHocPhan.Id)
                    return BadRequest(new { Message = "Id không khớp với lớp học phần cần sửa." });

                var existing = await _lopHocPhanRepository.GetLopHocPhanByIdAsync(id);
                if (existing == null)
                    return NotFound(new { Message = $"Không tìm thấy lớp học phần có Id {id}" });

                await _lopHocPhanRepository.UpdateLopHocPhanAsync(lopHocPhan);
                return Ok(new { Message = "Cập nhật lớp học phần thành công", Data = lopHocPhan });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Lỗi máy chủ", Error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLopHocPhan(int id)
        {
            try
            {
                var existing = await _lopHocPhanRepository.GetLopHocPhanByIdAsync(id);
                if (existing == null)
                    return NotFound(new { Message = $"Không tìm thấy lớp học phần có Id {id}" });

                await _lopHocPhanRepository.DeleteLopHocPhanAsync(id);
                return Ok(new { Message = "Xóa lớp học phần thành công." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Lỗi máy chủ", Error = ex.Message });
            }
        }

        /// <summary>
        /// Lấy danh sách sinh viên theo học lớp học phần
        /// </summary>
        [HttpGet("{id}/sinh-viens")]
        public async Task<IActionResult> GetSinhViensInLop(int id)
        {
            try
            {
                var sinhViens = await _lopHocPhanRepository.GetSinhViensInLopHocPhanAsync(id);
                return Ok(sinhViens);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Lỗi máy chủ", Error = ex.Message });
            }
        }

        /// <summary>
        /// Thêm sinh viên vào lớp học phần
        /// </summary>
        [HttpPost("{id}/them-sinh-vien/{sinhVienId}")]
        public async Task<IActionResult> AddSinhVienToLop(int id, int sinhVienId)
        {
            try
            {
                var success = await _lopHocPhanRepository.AddSinhVienToLopAsync(id, sinhVienId);
                if (!success)
                    return BadRequest(new { Message = "Sinh viên đã có trong lớp học phần hoặc lớp/sinh viên không hợp lệ." });

                return Ok(new { Message = "Đã thêm sinh viên vào lớp học phần thành công." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Lỗi máy chủ", Error = ex.Message });
            }
        }

        /// <summary>
        /// Xóa sinh viên khỏi lớp học phần
        /// </summary>
        [HttpDelete("{id}/xoa-sinh-vien/{sinhVienId}")]
        public async Task<IActionResult> RemoveSinhVienFromLop(int id, int sinhVienId)
        {
            try
            {
                var success = await _lopHocPhanRepository.RemoveSinhVienFromLopAsync(id, sinhVienId);
                if (!success)
                    return NotFound(new { Message = "Không tìm thấy sinh viên trong lớp học phần này." });

                return Ok(new { Message = "Đã xóa sinh viên khỏi lớp học phần." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Lỗi máy chủ", Error = ex.Message });
            }
        }
    }
}
