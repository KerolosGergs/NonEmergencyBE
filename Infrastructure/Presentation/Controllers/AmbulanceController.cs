using DomainLayer.Models;
using Infrastructure.Presentation;
using Microsoft.AspNetCore.Mvc;
using ServiceAbstraction;
using Shared.DTOS;
using Shared.DTOS.AmbulanceDTOS;
using System;
using System.Threading.Tasks;

namespace Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AmbulanceController : ControllerBase
    {
        private readonly IAmbulanceService _ambulanceService;

        public AmbulanceController(IAmbulanceService ambulanceService)
        {
            _ambulanceService = ambulanceService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var response = new GeneralResponse();
            try
            {
                response.Success = true;
                response.Data = await _ambulanceService.GetAllAmbulancesAsync();
                response.Message = "Ambulances fetched successfully.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
                response.Data = null;
            }

            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var response = new GeneralResponse();
            try
            {
                var ambulance = await _ambulanceService.GetAmbulanceByIdAsync(id);
                if (ambulance == null)
                {
                    response.Success = false;
                    response.Message = "Ambulance not found.";
                    return NotFound(response);
                }

                response.Success = true;
                response.Data = ambulance;
                response.Message = "Ambulance retrieved successfully.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
                response.Data = null;
            }

            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] AmbulanceDTO dto)
        {
            var response = new GeneralResponse();
            try
            {
                response.Data = await _ambulanceService.CreateAmbulanceAsync(dto);
                response.Success = true;
                response.Message = "Ambulance created successfully.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
                response.Data = null;
            }

            return Ok(response);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] AmbulanceDTO dto)
        {
            var response = new GeneralResponse();
            try
            {
                var updated = await _ambulanceService.UpdateAmbulanceAsync(id, dto);
                if (updated == null)
                {
                    response.Success = false;
                    response.Message = "Ambulance not found.";
                    return NotFound(response);
                }

                response.Success = true;
                response.Data = updated;
                response.Message = "Ambulance updated successfully.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
                response.Data = null;
            }

            return Ok(response);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var response = new GeneralResponse();
            try
            {
                var deleted = await _ambulanceService.DeleteAmbulanceAsync(id);
                if (!deleted)
                {
                    response.Success = false;
                    response.Message = "Ambulance not found or already deleted.";
                    return NotFound(response);
                }

                response.Success = true;
                response.Message = "Ambulance deleted successfully.";
                response.Data = null;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
                response.Data = null;
            }

            return Ok(response);
        }

        [HttpPost("assign-driver")]
        public async Task<IActionResult> AssignDriver([FromQuery] int ambulanceId, [FromQuery] int driverId)
        {
            var response = new GeneralResponse();
            try
            {
                await _ambulanceService.AssignDriverAsync(ambulanceId, driverId);
                response.Success = true;
                response.Message = "Driver assigned to ambulance successfully.";
                response.Data = null;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
                response.Data = null;
            }

            return Ok(response);
        }
    }
}
