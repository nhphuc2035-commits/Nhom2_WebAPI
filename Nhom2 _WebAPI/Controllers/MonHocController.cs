using Microsoft.AspNetCore.Mvc;
using Nhom2__WebAPI.Models;
using Nhom2__WebAPI.Repositories;

namespace Nhom2__WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MonHocController : ControllerBase
    {
        private readonly IMonHocRepository _monHocRepository;

        public MonHocController(IMonHocRepository monHocRepository)
        {
            _monHocRepository = monHocRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetMonHocs()
        {
            try
            {
                var monHocs = await _monHocRepository.GetMonHocsAsync();
                return Ok(monHocs);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Lỗi máy chủ", Error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetMonHocById(int id)
        {
            try
            {
                var monHoc = await _monHocRepository.GetMonHocByIdAsync(id);
                if (monHoc == null)
                    return NotFound(new { Message = $"Không tìm thấy môn học với Id {id}" });
                return Ok(monHoc);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Lỗi máy chủ", Error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddMonHoc([FromBody] MonHoc monHoc)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);

                var exists = await _monHocRepository.GetMonHocByMaMonAsync(monHoc.MaMon);
                if (exists != null)
                {
                    return BadRequest(new { Message = $"Mã môn học {monHoc.MaMon} đã tồn tại." });
                }

                await _monHocRepository.AddMonHocAsync(monHoc);
                return CreatedAtAction(nameof(GetMonHocById), new { id = monHoc.Id }, monHoc);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Lỗi máy chủ", Error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMonHoc(int id, [FromBody] MonHoc monHoc)
        {
            try
            {
                if (id != monHoc.Id)
                    return BadRequest(new { Message = "Id không khớp với môn học cần sửa." });

                var existing = await _monHocRepository.GetMonHocByIdAsync(id);
                if (existing == null)
                    return NotFound(new { Message = $"Không tìm thấy môn học có Id {id}" });

                await _monHocRepository.UpdateMonHocAsync(monHoc);
                return Ok(new { Message = "Cập nhật môn học thành công", Data = monHoc });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Lỗi máy chủ", Error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMonHoc(int id)
        {
            try
            {
                var existing = await _monHocRepository.GetMonHocByIdAsync(id);
                if (existing == null)
                    return NotFound(new { Message = $"Không tìm thấy môn học có Id {id}" });

                await _monHocRepository.DeleteMonHocAsync(id);
                return Ok(new { Message = "Xóa môn học thành công." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Lỗi máy chủ", Error = ex.Message });
            }
        }
    }
}
