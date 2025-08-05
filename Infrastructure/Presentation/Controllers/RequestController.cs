using System;
using System.Linq;
using System.Threading.Tasks;
using DomainLayer.Models;
using Microsoft.AspNetCore.Mvc;
using ServiceAbstraction;
using Shared.DTOS.RequestDTOS;

namespace Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RequestController(IRequestService _requestService, IDistanceService _distanceService) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var response = new GeneralResponse();
            try
            {
                response.Data = await _requestService.GetAllRequestsWithRelatedData();
                response.Success = true;
                response.Message = "Requests retrieved successfully.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return Ok(response);
        }

        [HttpGet("available-for-driver")]
        public async Task<IActionResult> GetPendingRequestsForDrivers()
        {
            var response = new GeneralResponse();
            try
            {
                response.Data = await _requestService.GetAvailableRequestsForDriverAsync();
                response.Success = true;
                response.Message = "Available requests for drivers retrieved successfully.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return Ok(response);
        }

        [HttpGet("available-for-nurse")]
        public async Task<IActionResult> GetPendingRequestsForNurses()
        {
            var response = new GeneralResponse();
            try
            {
                response.Data = await _requestService.GetAvailableRequestsForNurseAsync();
                response.Success = true;
                response.Message = "Available requests for nurses retrieved successfully.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> CreateRequest([FromQuery] string userId, [FromBody] CreateRequestDTO dto)
        {
            var response = new GeneralResponse();
            try
            {
                response.Data = await _requestService.AddNewRequest(userId, dto);
                response.Success = true;
                response.Message = "Request created successfully.";
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
                var dto = new RequestDTO { RequestId = id };
                response.Data = await _requestService.GetRequestById(dto);
                response.Success = response.Data != null;
                response.Message = response.Data != null ? "Request found." : "Request not found.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return Ok(response);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateRequest(int id, [FromBody] UpdateRequestDTO dto)
        {
            var response = new GeneralResponse();
            try
            {
                if (id != dto.RequestId)
                {
                    response.Success = false;
                    response.Message = "Id mismatch";
                    return BadRequest(response);
                }

                response.Data = await _requestService.UpdateRequest(dto);
                response.Success = true;
                response.Message = "Request updated successfully.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return Ok(response);
        }

        [HttpPut("assign-driver")]
        public async Task<IActionResult> AssignDriverToRequest([FromBody] AssignDriverDTO dto)
        {
            var response = new GeneralResponse();
            try
            {
                response.Data = await _requestService.AssignDriverAsync(dto);
                if (response.Data == null)
                {
                    response.Success = false;
                    response.Message = "Driver is already assigned.";
                    return Conflict(response);
                }
                response.Success = true;
                response.Message = "Driver assigned successfully.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return Ok(response);
        }

        [HttpPut("assign-nurse")]
        public async Task<IActionResult> AssignNurseToRequest([FromBody] AssignNurseDTO dto)
        {
            var response = new GeneralResponse();
            try
            {
                response.Data = await _requestService.AssignNurseAsync(dto);
                if (response.Data == null)
                {
                    response.Success = false;
                    response.Message = "Nurse is already assigned.";
                    return Conflict(response);
                }
                response.Success = true;
                response.Message = "Nurse assigned successfully.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return Ok(response);
        }

        [HttpPut("update-status")]
        public async Task<IActionResult> UpdateStatus([FromBody] UpdateRequestStatusDTO dto)
        {
            var response = new GeneralResponse();
            try
            {
                response.Data = await _requestService.UpdateStatusAsync(dto);
                if (response.Data == null)
                {
                    response.Success = false;
                    response.Message = "Request not found.";
                    return NotFound(response);
                }
                response.Success = true;
                response.Message = "Status updated successfully.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return Ok(response);
        }

        [HttpPut("cancel/{id}")]
        public async Task<IActionResult> Cancel(int id)
        {
            var response = new GeneralResponse();
            try
            {
                var result = await _requestService.CancelRequestAsync(id);
                response.Success = result;
                response.Message = result ? "Request cancelled successfully." : "Request not found.";
                if (!result) return NotFound(response);
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return Ok(response);
        }

        [HttpPost("confirm-request/{requestId}")]
        public async Task<IActionResult> ConfirmRequestByPatient(int requestId)
        {
            var response = new GeneralResponse();
            try
            {
                response.Data = await _requestService.ConfirmPatientAsync(requestId);
                if (response.Data == null)
                {
                    response.Success = false;
                    response.Message = "Request not found.";
                    return NotFound(response);
                }
                response.Success = true;
                response.Message = "Request confirmed successfully.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return Ok(response);
        }

        [HttpGet("distance")]
        public async Task<IActionResult> GetDistance([FromQuery] string from, [FromQuery] string to)
        {
            var response = new GeneralResponse();
            try
            {
                var km = await _distanceService.CalculateKMAsync(from, to);
                response.Data = new { Distance = km };
                response.Success = true;
                response.Message = "Distance calculated successfully.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return Ok(response);
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetRequestsByUserId(string userId)
        {
            var response = new GeneralResponse();
            try
            {
                response.Data = await _requestService.GetRequestsByUserIdAsync(userId);
                if (response.Data == null || !response.Data.Any())
                {
                    response.Success = false;
                    response.Message = "No requests found for this user.";
                    return NotFound(response);
                }
                response.Success = true;
                response.Message = "Requests retrieved successfully.";
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
