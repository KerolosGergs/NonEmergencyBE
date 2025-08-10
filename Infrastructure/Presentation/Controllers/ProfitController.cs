using DomainLayer.Models;
using Microsoft.AspNetCore.Mvc;
using ServiceAbstraction;
using Shared.DTOS.WithdrawalDTOS;

namespace Infrastructure.Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
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

        [HttpGet("balance/{userId}")]
        public async Task<IActionResult> GetUserBalance(string userId)
        {
            var response = new GeneralResponse();
            try
            {
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

        [HttpGet("history/{userId}")]
        public async Task<IActionResult> GetUserProfitHistory(string userId)
        {
            var response = new GeneralResponse();
            try
            {
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

        [HttpPost("withdrawal/request/{userId}")]
        public async Task<IActionResult> CreateWithdrawalRequest(string userId, [FromBody] CreateWithdrawalRequestDTO request)
        {
            var response = new GeneralResponse();
            try
            {
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

        [HttpGet("withdrawal/requests/{userId}")]
        public async Task<IActionResult> GetUserWithdrawalRequests(string userId)
        {
            var response = new GeneralResponse();
            try
            {
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

        [HttpDelete("withdrawal/request/{requestId}/{userId}")]
        public async Task<IActionResult> CancelWithdrawalRequest(int requestId, string userId)
        {
            var response = new GeneralResponse();
            try
            {
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
