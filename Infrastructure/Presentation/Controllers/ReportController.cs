using DomainLayer.Models;
using Microsoft.AspNetCore.Mvc;
using ServiceAbstraction;
using System;
using System.Threading.Tasks;

namespace Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReportController : ControllerBase
    {
        private readonly IReportService _reportService;
        public ReportController(IReportService reportService)
        {
            _reportService = reportService;
        }

        [HttpGet("totals")]
        public async Task<IActionResult> GetTotals()
        {
            var response = new GeneralResponse();
            try
            {
                var totalRequests = await _reportService.GetTotalRequestsAsync();
                var totalTrips = await _reportService.GetTotalTripsAsync();
                var totalRevenue = await _reportService.GetTotalRevenueAsync();

                response.Success = true;
                response.Data = new
                {
                    totalRequests,
                    totalTrips,
                    totalRevenue
                };
                response.Message = "Report totals fetched successfully.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }

            return Ok(response);
        }

        [HttpGet("most-active-drivers")]
        public async Task<IActionResult> GetMostActiveDrivers()
        {
            var response = new GeneralResponse();
            try
            {
                response.Data = await _reportService.GetMostActiveDriversAsync();
                response.Success = true;
                response.Message = "Most active drivers fetched successfully.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }

            return Ok(response);
        }

        [HttpGet("most-active-nurses")]
        public async Task<IActionResult> GetMostActiveNurses()
        {
            var response = new GeneralResponse();
            try
            {
                response.Data = await _reportService.GetMostActiveNursesAsync();
                response.Success = true;
                response.Message = "Most active nurses fetched successfully.";
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
