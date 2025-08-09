using DomainLayer.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServiceAbstraction;
using Shared.DTOS.WithdrawalDTOS;
using System.Security.Claims;

namespace Infrastructure.Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class WithdrawalController : ControllerBase
    {
        private readonly IWithdrawalService _withdrawalService;

        public WithdrawalController(IWithdrawalService withdrawalService)
        {
            _withdrawalService = withdrawalService;
        }

        [HttpGet("all")]
        public async Task<ActionResult<List<WithdrawalRequestDTO>>> GetAllWithdrawalRequests()
        {
            var response = new GeneralResponse();
            try
            {
                response.Data = await _withdrawalService.GetAllWithdrawalRequestsAsync();
                response.Success = true;
                response.Message = "تم جلب جميع طلبات السحب بنجاح";
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
                return BadRequest(response);
            }
        }

        [HttpGet("pending")]
        public async Task<ActionResult<List<WithdrawalRequestDTO>>> GetPendingWithdrawalRequests()
        {
            var response = new GeneralResponse();
            try
            {
                response.Data = await _withdrawalService.GetPendingWithdrawalRequestsAsync();
                response.Success = true;
                response.Message = "تم جلب طلبات السحب المعلقة بنجاح";
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
                return BadRequest(response);
            }
        }

        [HttpGet("{requestId}")]
        public async Task<ActionResult<WithdrawalRequestDTO>> GetWithdrawalRequest(int requestId)
        {
            var response = new GeneralResponse();
            try
            {
                var request = await _withdrawalService.GetWithdrawalRequestAsync(requestId);
                if (request == null)
                {
                    response.Success = false;
                    response.Message = "طلب السحب غير موجود";
                    return NotFound(response);
                }
                response.Data = request;
                response.Success = true;
                response.Message = "تم جلب طلب السحب بنجاح";
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
                return BadRequest(response);
            }
        }

        [HttpPost("approve/{requestId}")]
        public async Task<ActionResult<WithdrawalRequestDTO>> ApproveWithdrawalRequest(int requestId, [FromBody] string? notes = null)
        {
            var response = new GeneralResponse();
            try
            {
                var adminId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(adminId))
                    return Unauthorized();

                var result = await _withdrawalService.ApproveWithdrawalRequestAsync(requestId, adminId, notes);
                response.Data = result;
                response.Success = true;
                response.Message = "تم الموافقة على طلب السحب بنجاح";
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
                return BadRequest(response);
            }
        }

        [HttpPost("reject/{requestId}")]
        public async Task<ActionResult<WithdrawalRequestDTO>> RejectWithdrawalRequest(int requestId, [FromBody] string? notes = null)
        {
            var response = new GeneralResponse();
            try
            {
                var adminId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(adminId))
                    return Unauthorized();
                var result = await _withdrawalService.RejectWithdrawalRequestAsync(requestId, adminId, notes);
                response.Data = result;
                response.Success = true;
                response.Message = "تم رفض طلب السحب بنجاح";
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
                return BadRequest(response);
            }
        }

        [HttpPost("complete/{requestId}")]
        public async Task<ActionResult<WithdrawalRequestDTO>> CompleteWithdrawalRequest(int requestId)
        {
            var response = new GeneralResponse();
            try
            {
                var adminId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(adminId))
                    return Unauthorized();
                var result = await _withdrawalService.CompleteWithdrawalRequestAsync(requestId, adminId);
                response.Data = result;
                response.Success = true;
                response.Message = "تم إكمال طلب السحب بنجاح";
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
                return BadRequest(response);
            }
        }

        [HttpGet("summary")]
        public async Task<ActionResult<WithdrawalSummaryDTO>> GetWithdrawalSummary()
        {
            var response = new GeneralResponse();
            try
            {
                var allRequests = await _withdrawalService.GetAllWithdrawalRequestsAsync();
                
                var summary = new WithdrawalSummaryDTO
                {
                    TotalRequests = allRequests.Count,
                    PendingRequests = allRequests.Count(r => r.Status == WithdrawalStatus.Pending),
                    ApprovedRequests = allRequests.Count(r => r.Status == WithdrawalStatus.Approved),
                    RejectedRequests = allRequests.Count(r => r.Status == WithdrawalStatus.Rejected),
                    CompletedRequests = allRequests.Count(r => r.Status == WithdrawalStatus.Completed),
                    TotalRequestedAmount = allRequests.Sum(r => r.Amount),
                    TotalApprovedAmount = allRequests.Where(r => r.Status == WithdrawalStatus.Approved || r.Status == WithdrawalStatus.Completed).Sum(r => r.Amount),
                    TotalCompletedAmount = allRequests.Where(r => r.Status == WithdrawalStatus.Completed).Sum(r => r.Amount)
                };
                response.Data = summary;
                response.Success = true;
                response.Message = "تم جلب ملخص طلبات السحب بنجاح";
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
                return BadRequest(response);
            }
        }
    }
} 