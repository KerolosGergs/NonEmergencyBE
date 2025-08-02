using DomainLayer.Models;
using Microsoft.AspNetCore.Mvc;
using ServiceAbstraction;
using Shared.DTOS.Registeration;
using System;
using System.Threading.Tasks;

namespace Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PatientController : ControllerBase
    {
        private readonly IPatientService _patientService;
        public PatientController(IPatientService patientService)
        {
            _patientService = patientService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var response = new GeneralResponse();
            try
            {
                response.Data = await _patientService.GetAllPatientsAsync();
                response.Success = true;
                response.Message = "Patients fetched successfully.";
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
                var patient = await _patientService.GetPatientByIdAsync(id);
                if (patient == null)
                {
                    response.Success = false;
                    response.Message = "Patient not found.";
                    return NotFound(response);
                }

                response.Data = patient;
                response.Success = true;
                response.Message = "Patient retrieved successfully.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return Ok(response);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] PatientDTO dto)
        {
            var response = new GeneralResponse();
            try
            {
                var updated = await _patientService.UpdatePatientAsync(id, dto);
                if (updated == null)
                {
                    response.Success = false;
                    response.Message = "Patient not found.";
                    return NotFound(response);
                }

                response.Data = updated;
                response.Success = true;
                response.Message = "Patient updated successfully.";
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
                var deleted = await _patientService.DeletePatientAsync(id);
                if (!deleted)
                {
                    response.Success = false;
                    response.Message = "Patient not found.";
                    return NotFound(response);
                }

                response.Success = true;
                response.Message = "Patient deleted successfully.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return Ok(response);
        }

        [HttpGet("{id}/requests")]
        public async Task<IActionResult> GetRequests(int id)
        {
            var response = new GeneralResponse();
            try
            {
                response.Data = await _patientService.GetRequestsForPatientAsync(id);
                response.Success = true;
                response.Message = "Requests fetched successfully.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return Ok(response);
        }

        [HttpGet("{id}/trips")]
        public async Task<IActionResult> GetTrips(int id)
        {
            var response = new GeneralResponse();
            try
            {
                response.Data = await _patientService.GetTripsForPatientAsync(id);
                response.Success = true;
                response.Message = "Trips fetched successfully.";
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
