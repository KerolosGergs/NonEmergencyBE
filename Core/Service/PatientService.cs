using DomainLayer.Contracts;
using DomainLayer.Models.Identity_Module;
using ServiceAbstraction;
using Shared.DTOS.Registeration;
using Shared.DTOS.RequestDTOS;
using Shared.DTOS.TripDTOs;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Service
{
    public class PatientService : IPatientService
    {
        private readonly IPatientRepository _patientRepository;
        public PatientService(IPatientRepository patientRepository)
        {
            _patientRepository = patientRepository;
        }

        public async Task<IEnumerable<PatientDTO>> GetAllPatientsAsync()
        {
            var patients = await _patientRepository.GetAllPatientWithRealtedData();
            return patients.Select(p => new PatientDTO
            {
                Id = p.Id,
                FullName = p.User.FullName,
                PhoneNumber = p.PhoneNumber,
                Address = p.Address,
                MedicalHistory = p.MedicalHistory,
                Gender = p.Gender,
                DateOfBirth = p.DateOfBirth,
                UserId = p.UserId
            });
        }

        public async Task<PatientDTO> GetPatientByIdAsync(int id)
        {
            var p = await _patientRepository.GetByIdAsync(id);
            if (p == null) return null;
            return new PatientDTO
            {
                Id = p.Id,
                FullName = p.User.FullName,
                PhoneNumber = p.PhoneNumber,
                Address = p.Address,
                MedicalHistory = p.MedicalHistory,
                Gender = p.Gender,
                DateOfBirth = p.DateOfBirth,
                UserId = p.UserId
            };
        }

        public async Task<PatientDTO> UpdatePatientAsync(int id, PatientDTO dto)
        {
            var p = await _patientRepository.GetByIdAsync(id);
            if (p == null) return null;
            p.PhoneNumber = dto.PhoneNumber;
            p.Address = dto.Address;
            p.MedicalHistory = dto.MedicalHistory;
            p.Gender = dto.Gender;
            p.DateOfBirth = dto.DateOfBirth;
            _patientRepository.Update(p);
            await _patientRepository.SaveChangesAsync();
            return await GetPatientByIdAsync(id);
        }

        public async Task<bool> DeletePatientAsync(int id)
        {
            var p = await _patientRepository.GetByIdAsync(id);
            if (p == null) return false;
            _patientRepository.Delete(p);
            await _patientRepository.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<RequestDTO>> GetRequestsForPatientAsync(int patientId)
        {
            var requests = await _patientRepository.GetRequestsByPatientIdAsync(patientId);
            return requests.Select(r => new RequestDTO
            {
                RequestId = r.RequestId,
                RequestDate = r.RequestDate,
                PickupAddress = r.PickupAddress,
                DropOffAddress = r.DropOffAddress,
                ScheduledDate = r.ScheduledDate,
                EmergencyType = r.EmergencyType,
                Status = r.Status,
                Notes = r.Notes,
                PatientId = r.PatientId,

                // Driver Info (Safe Access)
                DriverId = r.DriverId,
                DriverName = r.Driver != null && r.Driver.User != null ? r.Driver.User.FullName ?? "" : "",
                DriverPhone = r.Driver != null ? r.Driver.PhoneNumber ?? "" : "",
                DriverImg = r.Driver != null ? r.Driver.ImgUrl ?? "" : "",

                // Nurse Info (Safe Access)
                NurseId = r.NurseId,
                NurseName = r.Nurse != null && r.Nurse.User != null ? r.Nurse.User.FullName ?? "" : "",
                NursePhone = r.Nurse != null ? r.Nurse.PhoneNumber ?? "" : "",
                NurseImg = r.Nurse != null ? r.Nurse.ImgUrl ?? "" : ""
            });

        }

        public async Task<IEnumerable<TripDTO>> GetTripsForPatientAsync(int patientId)
        {
            var trips = await _patientRepository.GetTripsByPatientIdAsync(patientId);
            return trips.Select(t => new TripDTO
            {
                TripId = t.TripId,
                StartTime = t.StartTime,
                EndTime = t.EndTime,
                PickupAddress = t.Request?.PickupAddress,
                DropOffAddress = t.Request?.DropOffAddress,
                DriverName = t.Driver?.User?.FullName,
                NurseName = t.Nurse?.User?.FullName,
                DistanceKM = t.DistanceKM,
                Price = t.Price,
                TripStatus = t.TripStatus
            });
        }
    }
} 