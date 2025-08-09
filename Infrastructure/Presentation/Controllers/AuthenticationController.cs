using System;
using System.Threading.Tasks;
using DomainLayer.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServiceAbstraction;
using Shared.DTOS.ApiResponse;
using Shared.DTOS.Registeration;

namespace Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthenticationController(IAuthenticationService _authService) : ControllerBase
    {
        [HttpPost("register/driver")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> RegisterDriver([FromForm] DriverRegisterDTO driverDto)
        {
            var response = new GeneralResponse();
            try
            {
                response.Data = await _authService.DriverRegisterAsync(driverDto);
                response.Success = true;
                response.Message = "Driver registered successfully.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
                response.Data = null;
            }

            return Ok(response);
        }

        [HttpPost("register/nurse")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> RegisterNurse([FromForm] NurseRegisterDTO nurseDto)
        {
            var response = new GeneralResponse();
            try
            {
                response.Data = await _authService.NurseRegisterAsync(nurseDto);
                response.Success = true;
                response.Message = "Nurse registered successfully.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
                response.Data = null;
            }

            return Ok(response);
        }

        [HttpPost("register/patient")]
        [AllowAnonymous]
        public async Task<IActionResult> RegisterPatient([FromForm] PatientRegisterDTO patientDto)
        {

            var response = new GeneralResponse();
            try
            {
                response.Data = await _authService.PatientRegisterAsync(patientDto);
                response.Success = true;
                response.Message = "Patient registered successfully.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
                response.Data = null;
            }

            return Ok(response);
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginDTO loginDto)
        {
            var response = new GeneralResponse();
            try
            {
                response.Data = await _authService.LoginAsync(loginDto);
                response.Success = true;
                response.Message = "Login successful.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
                response.Data = null;
            }

            return Ok(response);
        }

        [HttpGet("MyProfile")]
        [Authorize]
        public async Task<IActionResult> GetCurrentUser()
        {
            var response = new GeneralResponse();
            try
            {
                var email = User.Identity?.Name;
                if (string.IsNullOrEmpty(email))
                {
                    response.Success = false;
                    response.Message = "Unauthorized";
                    return StatusCode(401, response);
                }

                response.Data = await _authService.GetCurrentUserAsync(email);
                response.Success = true;
                response.Message = "User profile retrieved.";
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
