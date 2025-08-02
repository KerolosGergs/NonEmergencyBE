using DomainLayer.Models;
using Microsoft.AspNetCore.Mvc;
using ServiceAbstraction;
using Shared.DTOS;
using Shared.DTOS.Nurse;
using Shared.DTOS.Registeration;
using System;
using System.Threading.Tasks;

namespace Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NurseController : ControllerBase
    {
        private readonly INurseService nurseService;
        private readonly IAuthenticationService _authService;

        public NurseController(INurseService nurseService, IAuthenticationService authService)
        {
            this.nurseService = nurseService;
            _authService = authService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var response = new GeneralResponse();
            try
            {
                response.Data = await nurseService.GetAllNursesAsync();
                response.Success = true;
                response.Message = "Nurses fetched successfully.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var response = new GeneralResponse();
            try
            {
                var nurse = await nurseService.GetNurseByIdAsync(id);
                if (nurse == null)
                {
                    response.Success = false;
                    response.Message = "Nurse not found.";
                    return NotFound(response);
                }

                response.Data = nurse;
                response.Success = true;
                response.Message = "Nurse retrieved successfully.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }

            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] NurseRegisterDTO dto)
        {
            var response = new GeneralResponse();
            try
            {
                response.Data = await _authService.NurseRegisterAsync(dto);
                response.Success = true;
                response.Message = "Nurse registered successfully.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }

            return Ok(response);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateNurse(int id, [FromBody] NurseDto dto)
        {
            var response = new GeneralResponse();
            try
            {
                await nurseService.UpdateNurseAsync(id, dto);
                response.Success = true;
                response.Message = "Nurse updated successfully.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }

            return Ok(response);
        }

        [HttpGet("available")]
        public async Task<IActionResult> GetAvailableNurses()
        {
            var response = new GeneralResponse();
            try
            {
                response.Data = await nurseService.GetAvailableNursesAsync();
                response.Success = true;
                response.Message = "Available nurses fetched successfully.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }

            return Ok(response);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteNurse(int id)
        {
            var response = new GeneralResponse();
            try
            {
                bool deleted = await nurseService.DeleteNurseAsync(id);
                if (!deleted)
                {
                    response.Success = false;
                    response.Message = "Nurse not found.";
                    return NotFound(response);
                }

                response.Success = true;
                response.Message = "Nurse deleted successfully.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }

            return Ok(response);
        }

        [HttpPatch("{id}/toggle-availability")]
        public async Task<IActionResult> ToggleAvailability(int id, [FromBody] ToggleAvailabilityDto dto)
        {
            var response = new GeneralResponse();
            try
            {
                await nurseService.ToggleAvailabilityAsync(id, dto.IsAvailable);
                response.Success = true;
                response.Message = "Availability updated successfully.";
            }
            catch (KeyNotFoundException ex)
            {
                response.Success = false;
                response.Message = ex.Message;
                return NotFound(response);
            }
            catch (Exception)
            {
                response.Success = false;
                response.Message = "An unexpected error occurred.";
                return StatusCode(500, response);
            }

            return Ok(response);
        }
    }
}
