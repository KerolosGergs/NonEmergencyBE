using Microsoft.AspNetCore.Mvc;
using ServiceAbstraction;
using System;
using System.Threading.Tasks;
using Shared.DTOS.Registeration;
using Shared.DTOS.Driver;
using DomainLayer.Models;

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
        public async Task<IActionResult> Update(int id, [FromBody] DriverDTO dto)
        {
            var response = new GeneralResponse();
            try
            {
                var updated = await driverService.UpdateDriverAsync(id, dto);
                if (updated == null)
                {
                    response.Success = false;
                    response.Message = "Driver not found.";
                    return NotFound(response);
                }

                response.Data = updated;
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
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }

            return Ok(response);
        }

        [HttpPatch("{id}/toggle-availability")]
        public async Task<IActionResult> ToggleAvailability(int id, [FromBody] bool isAvailable)
        {
            var response = new GeneralResponse();
            try
            {
                await driverService.ToggleAvailabilityAsync(id, isAvailable);
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
