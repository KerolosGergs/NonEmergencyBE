using Microsoft.AspNetCore.Mvc;
using ServiceAbstraction;
using System;
using System.Threading.Tasks;
using Shared.DTOS.Registeration;
using Shared.DTOS.Driver;
using DomainLayer.Models;
using Shared.DTOS.Nurse;

namespace Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DriverController : ControllerBase
    {
        private readonly IDriverService driverService;
        private readonly IAuthenticationService _authService;

        public DriverController(IDriverService driverService, IAuthenticationService authService)
        {
            this.driverService = driverService;
            _authService = authService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var response = new GeneralResponse();
            try
            {
                response.Data = await driverService.GetAllDriversAsync();
                response.Success = true;
                response.Message = "Drivers fetched successfully.";
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
                var driver = await driverService.GetDriverByIdAsync(id);
                if (driver == null)
                {
                    response.Success = false;
                    response.Message = "Driver not found.";
                    return NotFound(response);
                }

                response.Data = driver;
                response.Success = true;
                response.Message = "Driver fetched successfully.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }

            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] DriverRegisterDTO dto)
        {
            var response = new GeneralResponse();
            try
            {
                response.Data = await _authService.DriverRegisterAsync(dto);
                response.Success = true;
                response.Message = "Driver created successfully.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }

            return Ok(response);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDriver(int id, [FromBody] UpdateDriverDto dto)
        {
            var response = new GeneralResponse();
            try
            {
                await driverService.UpdateDriverAsync(id, dto);
                response.Success = true;
                response.Message = "Driver updated successfully.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }

            return Ok(response);
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var response = new GeneralResponse();

            try
            {
                var deleted = await driverService.DeleteDriverAsync(id);
                if (!deleted)
                {
                    response.Success = false;
                    response.Message = "Driver not found or already deleted.";
                    return NotFound(response);
                }

                response.Success = true;
                response.Message = "Driver deleted successfully.";
                return Ok(response);
            }
            catch (InvalidOperationException ex)
            {
                response.Success = false;
                response.Message = ex.Message;
                return BadRequest(response);
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "An error occurred while deleting the driver.";
                return StatusCode(500, response);
            }
        }

        [HttpPatch("{id}/toggle-availability")]
        public async Task<IActionResult> ToggleAvailability(int id, [FromBody] ToggleAvailabilityDto dto)
        {
            var response = new GeneralResponse();
            try
            {
                await driverService.ToggleAvailabilityAsync(id, dto.IsAvailable);
                response.Success = true;
                response.Message = "Driver availability updated successfully.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }

            return Ok(response);
        }
    }
}
