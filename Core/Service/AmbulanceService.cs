using DomainLayer.Contracts;
using DomainLayer.Models.Ambulance_Module;
using ServiceAbstraction;
using Shared.DTOS;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Service.Mapping;
using Shared.DTOS.AmbulanceDTOS;

namespace Service
{
    public class AmbulanceService : IAmbulanceService
    {
        private readonly IAmbulanceRepository _ambulanceRepository;
        public AmbulanceService(IAmbulanceRepository ambulanceRepository)
        {
            _ambulanceRepository = ambulanceRepository;
        }

        public async Task<IEnumerable<AmbulanceGetDTO>> GetAllAmbulancesAsync()
        {
            var ambulances = await _ambulanceRepository.GetAllWithRelatedData();
            return ambulances.Select(a => a.ToAmbulanceGetDTO());
        }

        public async Task<AmbulanceDTO> GetAmbulanceByIdAsync(int id)
        {
            var ambulance = await _ambulanceRepository.GetByIdWithRelatedData(id);
            return ambulance?.ToAmbulanceDTO();
        }

        public async Task<AmbulanceDTO> CreateAmbulanceAsync(AmbulanceDTO dto)
        {
            if (dto.DriverId == 0)
                throw new Exception("Driver Not Found");
            var ambulance = new Ambulance
            {
                PlateNumber = dto.PlateNumber,
                CurrentLocation = dto.CurrentLocation,
                Status = dto.Status,
                Type = dto.Type,
                DriverId = dto.DriverId
            };

            await _ambulanceRepository.AddAsync(ambulance);
            await _ambulanceRepository.SaveChangesAsync();
            return ambulance.ToAmbulanceDTO();
        }

        public async Task<AmbulanceDTO> UpdateAmbulanceAsync(int id, AmbulanceDTO dto)
        {
            var ambulance = await _ambulanceRepository.GetByIdAsync(id);
            if (ambulance == null) return null;
            ambulance.PlateNumber = dto.PlateNumber;
            ambulance.CurrentLocation = dto.CurrentLocation;
            ambulance.Status = dto.Status;
            ambulance.Type = dto.Type;
            ambulance.DriverId = dto.DriverId;
            _ambulanceRepository.Update(ambulance);
            await _ambulanceRepository.SaveChangesAsync();

            return ambulance.ToAmbulanceDTO();
        }

        public async Task<bool> DeleteAmbulanceAsync(int id)
        {
            var ambulance = await _ambulanceRepository.GetByIdAsync(id);
            if (ambulance == null) return false;
            _ambulanceRepository.Delete(ambulance);
            await _ambulanceRepository.SaveChangesAsync();
            return true;
        }

        public async Task AssignDriverAsync(int ambulanceId, int driverId)
        {
            await _ambulanceRepository.AssignDriverAsync(ambulanceId, driverId);
        }
    }
} 