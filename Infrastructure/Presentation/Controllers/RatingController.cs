using DomainLayer.Models;
using Microsoft.AspNetCore.Mvc;
using ServiceAbstraction;
using Shared.DTOS.TripDTOs;
using System;
using System.Threading.Tasks;

namespace Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RatingController : ControllerBase
    {
        private readonly IRatingService _ratingService;
        public RatingController(IRatingService ratingService)
        {
            _ratingService = ratingService;
        }

        [HttpPost]
        public async Task<IActionResult> AddRating([FromBody] RatingDTO dto)
        {
            var response = new GeneralResponse();
            try
            {
                await _ratingService.AddRatingAsync(dto);
                response.Success = true;
                response.Message = "Rating added successfully.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return Ok(response);
        }

        [HttpGet("driver/{driverId}")]
        public async Task<IActionResult> GetRatingsForDriver(int driverId)
        {
            var response = new GeneralResponse();
            try
            {
                response.Data = await _ratingService.GetRatingsForDriverAsync(driverId);
                response.Success = true;
                response.Message = "Ratings for driver fetched successfully.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return Ok(response);
        }

        [HttpGet("nurse/{nurseId}")]
        public async Task<IActionResult> GetRatingsForNurse(int nurseId)
        {
            var response = new GeneralResponse();
            try
            {
                response.Data = await _ratingService.GetRatingsForNurseAsync(nurseId);
                response.Success = true;
                response.Message = "Ratings for nurse fetched successfully.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return Ok(response);
        }

        [HttpGet("driver/{driverId}/average")]
        public async Task<IActionResult> GetAverageRatingForDriver(int driverId)
        {
            var response = new GeneralResponse();
            try
            {
                response.Data = await _ratingService.GetAverageRatingForDriverAsync(driverId);
                response.Success = true;
                response.Message = "Average rating for driver fetched successfully.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return Ok(response);
        }

        [HttpGet("nurse/{nurseId}/average")]
        public async Task<IActionResult> GetAverageRatingForNurse(int nurseId)
        {
            var response = new GeneralResponse();
            try
            {
                response.Data = await _ratingService.GetAverageRatingForNurseAsync(nurseId);
                response.Success = true;
                response.Message = "Average rating for nurse fetched successfully.";
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
