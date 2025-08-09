using DomainLayer.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServiceAbstraction;
using Shared.DTOS.ProfitDTOS;
using Shared.DTOS.WithdrawalDTOS;
using System.Security.Claims;

namespace Infrastructure.Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ProfitController : ControllerBase
    {
        private readonly IProfitDistributionService _profitService;
        private readonly IWithdrawalService _withdrawalService;

        public ProfitController(
            IProfitDistributionService profitService,
            IWithdrawalService withdrawalService)
        {
            _profitService = profitService;
            _withdrawalService = withdrawalService;
        }

        [HttpPost("distribute/{tripId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DistributeTripProfits(int tripId)
        {
            var response = new GeneralResponse();
            try
            {
                response.Data = await _profitService.DistributeTripProfitsAsync(tripId);
                response.Success = true;
                response.Message = "تم توزيع الأرباح بنجاح";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return Ok(response);
        }

        [HttpGet("balance")]
        public async Task<IActionResult> GetUserBalance()
        {
            var response = new GeneralResponse();
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                    return Unauthorized();

                response.Data = await _profitService.GetUserBalanceAsync(userId);
                response.Success = true;
                response.Message = "تم جلب الرصيد بنجاح";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return Ok(response);
        }

        [HttpGet("history")]
        public async Task<IActionResult> GetUserProfitHistory()
        {
            var response = new GeneralResponse();
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                    return Unauthorized();

                response.Data = await _profitService.GetUserProfitHistoryAsync(userId);
                response.Success = true;
                response.Message = "تم جلب سجل الأرباح بنجاح";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return Ok(response);
        }

        [HttpGet("all")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllProfitDistributions()
        {
            var response = new GeneralResponse();
            try
            {
                response.Data = await _profitService.GetAllProfitDistributionsAsync();
                response.Success = true;
                response.Message = "تم جلب جميع توزيعات الأرباح بنجاح";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return Ok(response);
        }

        [HttpGet("trip/{tripId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetProfitDistributionByTrip(int tripId)
        {
            var response = new GeneralResponse();
            try
            {
                var distribution = await _profitService.GetProfitDistributionByTripAsync(tripId);
                if (distribution == null)
                {
                    response.Success = false;
                    response.Message = "لم يتم العثور على توزيع أرباح لهذه الرحلة";
                    return NotFound(response);
                }

                response.Data = distribution;
                response.Success = true;
                response.Message = "تم جلب توزيع الأرباح بنجاح";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return Ok(response);
        }

        [HttpPost("withdrawal/request")]
        public async Task<IActionResult> CreateWithdrawalRequest([FromBody] CreateWithdrawalRequestDTO request)
        {
            var response = new GeneralResponse();
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                    return Unauthorized();

                request.UserId = userId;
                response.Data = await _withdrawalService.CreateWithdrawalRequestAsync(request);
                response.Success = true;
                response.Message = "تم إنشاء طلب السحب بنجاح";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return Ok(response);
        }

        [HttpGet("withdrawal/requests")]
        public async Task<IActionResult> GetUserWithdrawalRequests()
        {
            var response = new GeneralResponse();
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                    return Unauthorized();

                response.Data = await _withdrawalService.GetUserWithdrawalRequestsAsync(userId);
                response.Success = true;
                response.Message = "تم جلب طلبات السحب بنجاح";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return Ok(response);
        }

        [HttpDelete("withdrawal/request/{requestId}")]
        public async Task<IActionResult> CancelWithdrawalRequest(int requestId)
        {
            var response = new GeneralResponse();
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                    return Unauthorized();

                response.Data = await _withdrawalService.CancelWithdrawalRequestAsync(requestId, userId);
                response.Success = true;
                response.Message = "تم إلغاء طلب السحب بنجاح";
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