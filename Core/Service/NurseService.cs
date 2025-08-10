using DomainLayer.Contracts;
using DomainLayer.Models.Identity_Module;
using Microsoft.EntityFrameworkCore;
using ServiceAbstraction;
using Shared;
using Shared.DTOS.Nurse;
using Shared.DTOS.TripDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class NurseService : INurseService
    {
        private readonly INurseRepository nurseRepository;

        public NurseService(INurseRepository nurseRepository)
        {
            this.nurseRepository = nurseRepository;
        }
        public async Task<IEnumerable<NurseDetailsDto>> GetAllNursesAsync()
        {
            var nurses = await nurseRepository.GetAllWithRelatedData();

            return nurses.Select(n => new NurseDetailsDto
            {
                Id = n.Id,
                Certification = n.Certification,
                IsAvailable = n.IsAvailable,
                PhoneNumber = n.PhoneNumber,
                UserId = n.UserId,
                FullName = n.User.FullName
            });
        }

        public async Task<NurseDetailsDto> GetNurseByIdAsync(int id)
        {
            var nurse = await nurseRepository.GetByIdWithRelatedData(id);
            if (nurse == null)
                return null;


            return new NurseDetailsDto
            {
                Id = nurse.Id,
                Certification = nurse.Certification,
                IsAvailable = nurse.IsAvailable,
                PhoneNumber = nurse.PhoneNumber,
                UserId = nurse.UserId,
                FullName = nurse.User.FullName
            };

        }
        public async Task<IEnumerable<NurseDetailsDto>> GetAvailableNursesAsync()
        {
            var nurses = await nurseRepository.GetAvailableAsync();
            return nurses.Select(n => new NurseDetailsDto
            {
                Id = n.Id,
                Certification = n.Certification,
                IsAvailable = n.IsAvailable,
                PhoneNumber = n.PhoneNumber,
                FullName = n.User.FullName,
                UserId = n.UserId
            });
        }



        public async Task UpdateNurseAsync(int id, UpdateNurseDto dto)
        {
            var nurse = await nurseRepository.GetByIdWithRelatedData(id);
            if (nurse == null)
                throw new KeyNotFoundException($"Nurse with ID {id} not found.");

            // Update nurse properties
            nurse.Certification = dto.Certification;
            nurse.IsAvailable = dto.IsAvailable;
            nurse.PhoneNumber = dto.PhoneNumber;

            // Ensure User is loaded and not null
            if (nurse.User == null)
                throw new Exception("Associated User not found.");

            nurse.User.FullName = dto.FullName;

            // Update entity (optional if EF is tracking)
            nurseRepository.Update(nurse); // You can remove this if using EF Core Change Tracker

            await nurseRepository.SaveChangesAsync();
        }




        public async Task<bool> DeleteNurseAsync(int nurseId)
        {
            var nurse = await nurseRepository.GetByIdWithRequestsAndTripsAsync(nurseId);
            if (nurse == null)
                return false;

            var ongoingTrips = nurse.Trips.Where(t => t.TripStatus == TripStatus.Ongoing).ToList();
            if (ongoingTrips.Any())
            {
                throw new InvalidOperationException("Cannot delete nurse with ongoing trips. Complete or cancel trips first.");
            }

            foreach (var trip in nurse.Trips.ToList())
            {
                if (trip.TripStatus == TripStatus.Assigned || trip.TripStatus == TripStatus.Pending)
                {
                    trip.TripStatus = TripStatus.Cancelled;
                    if (trip.Request != null)
                    {
                        trip.Request.Status = RequestStatus.Cancelled;
                        trip.Request.NurseId = null;
                        trip.Request.AssignedAmbulanceId = 0;
                        trip.Request.PatientConfirmed = false;
                    }
                }
            }

            foreach (var request in nurse.AssignedRequests.ToList())
            {
                if (request.Status == RequestStatus.InProgress ||
                    request.Status == RequestStatus.Accepted ||
                    request.Status == RequestStatus.Pending)
                {
                    request.Status = RequestStatus.Cancelled;
                    request.NurseId = null;
                    request.AssignedAmbulanceId = 0;
                    request.PatientConfirmed = false;
                }
            }

            nurseRepository.Delete(nurse);
            await nurseRepository.SaveChangesAsync();

            return true;
        }


        public async Task ToggleAvailabilityAsync(int id, bool isAvailable)
        {
            var nurse = await nurseRepository.GetByIdAsync(id);
            if (nurse == null) throw new KeyNotFoundException("Nurse not found");

            nurse.IsAvailable = isAvailable;
            await nurseRepository.SaveChangesAsync();
        }


    }
}
