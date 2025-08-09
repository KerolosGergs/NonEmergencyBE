using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DomainLayer.Contracts;
using DomainLayer.Models.Identity_Module;
using DomainLayer.Models.Trip_Module;
using DomainLayer.Models.Withdrawal_Module;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ServiceAbstraction;
using Shared.DTOS.ProfitDTOS;
using Shared.DTOS.TripDTOs;

namespace Core.Service
{
    public class ProfitDistributionService : IProfitDistributionService
    {
        private readonly IProfitDistributionRepository _profitDistributionRepository;
        private readonly ITripRepository _tripRepository;
        private readonly IWithdrawalRequestRepository _withdrawalRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        public ProfitDistributionService(
            IProfitDistributionRepository profitDistributionRepository,
            ITripRepository tripRepository,
            IWithdrawalRequestRepository withdrawalRepository,
            UserManager<ApplicationUser> userManager)
        {
            _profitDistributionRepository = profitDistributionRepository;
            _tripRepository = tripRepository;
            _withdrawalRepository = withdrawalRepository;
            _userManager = userManager;
        }

        public async Task<ProfitDistributionDTO> DistributeTripProfitsAsync(int tripId)
        {
            // الحصول على الرحلة
            var trip = await _tripRepository.GetByIdAsync(tripId);
            if (trip == null)
                throw new DomainLayer.Exceptions.NotFoundException($"الرحلة رقم {tripId} غير موجودة");

            if (trip.TripStatus != TripStatus.Completed)
                throw new ArgumentException("لا يمكن توزيع الأرباح إلا للرحلات المكتملة");


            // التحقق من عدم توزيع الأرباح مسبقاً
            var existingDistribution = await _profitDistributionRepository.GetByTripIdAsync(tripId);
            if (existingDistribution != null)
                throw new ArgumentException("تم توزيع الأرباح لهذه الرحلة مسبقاً");
            // الحصول على معلومات السائق والممرضة
            var driver = await _userManager.FindByIdAsync(trip.Driver.UserId);
            var nurse = trip.NurseId.HasValue ? await _userManager.FindByIdAsync(trip.Nurse.UserId) : null;

            // حساب الأرباح
            var driverProfit = trip.Price * 0.40m;
            var nurseProfit = trip.Price * 0.30m;
            var platformProfit = trip.Price * 0.30m;

            // إنشاء توزيع الأرباح
            var profitDistribution = new ProfitDistribution
            {
                TripId = tripId,
                TotalTripPrice = trip.Price,
                DriverProfit = driverProfit,
                DriverId = driver.Id,
                NurseProfit = nurse != null ? nurseProfit : null,
                NurseId = nurse?.Id,
                PlatformProfit = platformProfit,
                DistributionDate = DateTime.UtcNow,
                DriverPercentage = 0.40m,
                NursePercentage = 0.30m,
                PlatformPercentage = 0.30m
            };

            // حفظ توزيع الأرباح
            await _profitDistributionRepository.AddAsync(profitDistribution);

            // تحديث رصيد السائق
            driver.Balance += driverProfit;
            await _userManager.UpdateAsync(driver);

            // تحديث رصيد الممرضة إذا كانت موجودة
            if (nurse != null)
            {
                nurse.Balance += nurseProfit;
                await _userManager.UpdateAsync(nurse);
            }

            // تحويل إلى DTO
            return new ProfitDistributionDTO
            {
                Id = profitDistribution.Id,
                TripId = tripId,
                Trip = new TripDTO
                {
                    TripId = trip.TripId,
                    StartTime = trip.StartTime,
                    EndTime = trip.EndTime,
                    DistanceKM = trip.DistanceKM,
                    TripStatus = trip.TripStatus,
                    Price = trip.Price
                },
                TotalTripPrice = trip.Price,
                DriverProfit = driverProfit,
                DriverId = driver.Id,
                DriverName = driver.FullName,
                NurseProfit = nurse != null ? nurseProfit : null,
                NurseId = nurse?.Id,
                NurseName = nurse?.FullName,
                PlatformProfit = platformProfit,
                DistributionDate = profitDistribution.DistributionDate,
                DriverPercentage = 0.40m,
                NursePercentage = 0.30m,
                PlatformPercentage = 0.30m
            };
        }

        public async Task<decimal> GetUserBalanceAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                throw new DomainLayer.Exceptions.NotFoundException("المستخدم غير موجود");

            return user.Balance;
        }

        public async Task<List<ProfitDistributionDTO>> GetUserProfitHistoryAsync(string userId)
        {
            var distributions = await _profitDistributionRepository.GetUserProfitHistoryAsync(userId);
            
            return distributions.Select(d => new ProfitDistributionDTO
            {
                Id = d.Id,
                TripId = d.TripId,
                TotalTripPrice = d.TotalTripPrice,
                DriverProfit = d.DriverProfit,
                DriverId = d.DriverId,
                DriverName = d.Driver.FullName,
                NurseProfit = d.NurseProfit,
                NurseId = d.NurseId,
                NurseName = d.Nurse?.FullName,
                PlatformProfit = d.PlatformProfit,
                DistributionDate = d.DistributionDate,
                DriverPercentage = d.DriverPercentage,
                NursePercentage = d.NursePercentage,
                PlatformPercentage = d.PlatformPercentage
            }).ToList();
        }

        public async Task<List<ProfitDistributionDTO>> GetAllProfitDistributionsAsync()
        {
            var distributions = await _profitDistributionRepository.GetAllProfitDistributionsAsync();
            
            return distributions.Select(d => new ProfitDistributionDTO
            {
                Id = d.Id,
                TripId = d.TripId,
                TotalTripPrice = d.TotalTripPrice,
                DriverProfit = d.DriverProfit,
                DriverId = d.DriverId,
                DriverName = d.Driver.FullName,
                NurseProfit = d.NurseProfit,
                NurseId = d.NurseId,
                NurseName = d.Nurse?.FullName,
                PlatformProfit = d.PlatformProfit,
                DistributionDate = d.DistributionDate,
                DriverPercentage = d.DriverPercentage,
                NursePercentage = d.NursePercentage,
                PlatformPercentage = d.PlatformPercentage
            }).ToList();
        }

        public async Task<ProfitDistributionDTO> GetProfitDistributionByTripAsync(int tripId)
        {
            var distribution = await _profitDistributionRepository.GetByTripIdAsync(tripId);
            if (distribution == null)
                return null;

            return new ProfitDistributionDTO
            {
                Id = distribution.Id,
                TripId = distribution.TripId,
                TotalTripPrice = distribution.TotalTripPrice,
                DriverProfit = distribution.DriverProfit,
                DriverId = distribution.DriverId,
                DriverName = distribution.Driver.FullName,
                NurseProfit = distribution.NurseProfit,
                NurseId = distribution.NurseId,
                NurseName = distribution.Nurse?.FullName,
                PlatformProfit = distribution.PlatformProfit,
                DistributionDate = distribution.DistributionDate,
                DriverPercentage = distribution.DriverPercentage,
                NursePercentage = distribution.NursePercentage,
                PlatformPercentage = distribution.PlatformPercentage
            };
        }
    }
} 