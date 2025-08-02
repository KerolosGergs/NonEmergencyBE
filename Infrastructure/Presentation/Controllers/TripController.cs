using System;
using System.Threading.Tasks;
using DomainLayer.Models;
using Microsoft.AspNetCore.Mvc;
using ServiceAbstraction;

namespace Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TripController(ITripService _tripService) : ControllerBase
    {
        [HttpGet("driver/{driverId}")]
        public async Task<IActionResult> GetTripsForDriver(int driverId)
        {
            var response = new GeneralResponse();
            try
            {
                response.Data = await _tripService.GetTripsForDriverAsync(driverId);
                response.Success = true;
                response.Message = "Trips for driver retrieved successfully.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return Ok(response);
        }

        [HttpGet("nurse/{nurseId}")]
        public async Task<IActionResult> GetTripsForNurse(int nurseId)
        {
            var response = new GeneralResponse();
            try
            {
                response.Data = await _tripService.GetTripsForNurseAsync(nurseId);
                response.Success = true;
                response.Message = "Trips for nurse retrieved successfully.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return Ok(response);
        }

        [HttpGet("request/{requestId}")]
        public async Task<IActionResult> GetTripByRequestId(int requestId)
        {
            var response = new GeneralResponse();
            try
            {
                var trip = await _tripService.GetTripByRequestIdAsync(requestId);
                if (trip == null)
                {
                    response.Success = false;
                    response.Message = "Trip not found.";
                    return NotFound(response);
                }

                response.Data = trip;
                response.Success = true;
                response.Message = "Trip retrieved successfully.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return Ok(response);
        }

        [HttpPost("create/{requestId}")]
        public async Task<IActionResult> CreateTrip(int requestId)
        {
            var response = new GeneralResponse();
            try
            {
                response.Data = await _tripService.CreateTripFromRequestAsync(requestId);
                response.Success = true;
                response.Message = "Trip created successfully.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpPut("{tripId}/start")]
        public async Task<IActionResult> StartTrip(int tripId)
        {
            var response = new GeneralResponse();
            try
            {
                var result = await _tripService.ConfirmTripStartAsync(tripId);
                if (!result)
                {
                    response.Success = false;
                    response.Message = "Could not start trip.";
                    return BadRequest(response);
                }

                response.Success = true;
                response.Message = "Trip started successfully.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpPut("{tripId}/complete")]
        public async Task<IActionResult> CompleteTrip(int tripId)
        {
            var response = new GeneralResponse();
            try
            {
                var result = await _tripService.CompleteTripAsync(tripId);
                if (!result)
                {
                    response.Success = false;
                    response.Message = "Could not complete trip.";
                    return BadRequest(response);
                }

                response.Success = true;
                response.Message = "Trip completed successfully.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
                return BadRequest(response);
            }

            return Ok(response);
        }
    }
}
