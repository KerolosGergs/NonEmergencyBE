using DomainLayer.Models;
using Microsoft.AspNetCore.Mvc;
using ServiceAbstraction;
using System.Threading.Tasks;

namespace Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdminController : ControllerBase
    {
        private readonly IDriverService _driverService;
        private readonly INurseService _nurseService;
        private readonly IAmbulanceService _ambulanceService;
        private readonly IPatientService _patientService;
        private readonly IRequestService _requestService;
        private readonly ITripService _tripService;

        public AdminController(
            IDriverService driverService,
            INurseService nurseService,
            IAmbulanceService ambulanceService,
            IPatientService patientService,
            IRequestService requestService,
            ITripService tripService)
        {
            _driverService = driverService;
            _nurseService = nurseService;
            _ambulanceService = ambulanceService;
            _patientService = patientService;
            _requestService = requestService;
            _tripService = tripService;
        }

        [HttpGet("drivers")]
        public async Task<IActionResult> GetAllDrivers()
        {
            GeneralResponse generalResponse = new GeneralResponse();
            try
            {
                generalResponse.Success = true;
                generalResponse.Data = await _driverService.GetAllDriversAsync();
                generalResponse.Message = "Get Data Succeded";

            }
            catch (Exception ex)
            {
                generalResponse.Success = false;
                generalResponse.Message = ex.Message;
                generalResponse.Data = ex.Message;
            }
            return Ok(generalResponse);
        }

        [HttpGet("nurses")]
        public async Task<IActionResult> GetAllNurses()
        {
            GeneralResponse generalResponse = new GeneralResponse();
            try
            {
                generalResponse.Success = true;
                generalResponse.Message = "Get Data Succeded";
                generalResponse.Data = await _nurseService.GetAllNursesAsync();
            }
            catch (Exception ex)
            {
                generalResponse.Success = false;
                generalResponse.Message = ex.Message;
                generalResponse.Data = ex.Message;

            }
            return Ok(generalResponse);
        }
        [HttpGet("ambulances")]
        public async Task<IActionResult> GetAllAmbulances()
        {
            GeneralResponse generalResponse = new GeneralResponse();
            try
            {
                generalResponse.Success = true;
                generalResponse.Message = "Get Data Succeded";
                generalResponse.Data = await _ambulanceService.GetAllAmbulancesAsync();

            }
            catch (Exception ex)
            {
                generalResponse.Success = false;
                generalResponse.Message = ex.Message;
                generalResponse.Data = ex.Message;
            }

            return Ok(generalResponse); ;
        }
        [HttpGet("patients")]
        public async Task<IActionResult> GetAllPatients()
        {

            GeneralResponse generalResponse = new GeneralResponse();
            try
            {
                generalResponse.Success = true;
                generalResponse.Data = await _patientService.GetAllPatientsAsync();
                generalResponse.Message = "Get Data Succeded";
            }
            catch (Exception ex)
            {
                generalResponse.Success = false;
                generalResponse.Message = ex.Message;
                generalResponse.Data = ex.Message;
            }
            return Ok(generalResponse);
        }

        [HttpGet("requests")]
        public async Task<IActionResult> GetAllRequests()
        {
            GeneralResponse generalResponse = new GeneralResponse();
            try
            {
                generalResponse.Success = true;
                generalResponse.Data = await _requestService.GetAllRequestsWithRelatedData();
                generalResponse.Message = "Get Data Succeded";
            }
            catch (Exception ex)
            {
                generalResponse.Success = false;
                generalResponse.Message = ex.Message;
                generalResponse.Data = ex.Message;
            }
            return Ok(generalResponse);
        }
        [HttpGet("trips")]
        public async Task<IActionResult> GetAllTrips()
        {
            GeneralResponse generalResponse = new GeneralResponse();
            try
            {
                generalResponse.Success = true;
                generalResponse.Data = await _tripService.GetTripsForDriverAsync(0);
                generalResponse.Message = "Get Data Succeded";

            }
            catch(Exception ex)
            {
                generalResponse.Success = false;
                generalResponse.Message = ex.Message;
                generalResponse.Data = ex.Message;
            }
            return Ok(generalResponse);

        }
    }
} 