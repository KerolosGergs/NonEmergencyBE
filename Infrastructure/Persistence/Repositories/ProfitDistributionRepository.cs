using DomainLayer.Contracts;
using DomainLayer.Models.Withdrawal_Module;
using Microsoft.EntityFrameworkCore;
using Persistence.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Persistence.Repositories
{
    public class ProfitDistributionRepository : GenericRepository<ProfitDistribution>, IProfitDistributionRepository
    {
        private readonly ApplicationDbContext _context;

        public ProfitDistributionRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<ProfitDistribution> GetByTripIdAsync(int tripId)
        {
            return await _context.ProfitDistributions
                .Include(p => p.Driver)
                .Include(p => p.Nurse)
                .FirstOrDefaultAsync(p => p.TripId == tripId);
        }

        public async Task<List<ProfitDistribution>> GetUserProfitHistoryAsync(string userId)
        {
            return await _context.ProfitDistributions
                .Include(p => p.Driver)
                .Include(p => p.Nurse)
                .Where(p => p.DriverId == userId || p.NurseId == userId)
                .OrderByDescending(p => p.DistributionDate)
                .ToListAsync();
        }

        public async Task<List<ProfitDistribution>> GetAllProfitDistributionsAsync()
        {
            return await _context.ProfitDistributions
                .Include(p => p.Driver)
                .Include(p => p.Nurse)
                .OrderByDescending(p => p.DistributionDate)
                .ToListAsync();
        }

        public async Task<decimal> GetTotalUserEarningsAsync(string userId)
        {
            return await _context.ProfitDistributions
                .Where(p => p.DriverId == userId || p.NurseId == userId)
                .SumAsync(p =>
                    (p.DriverId == userId ? p.DriverProfit ?? 0 : 0) +
                    (p.NurseId == userId ? p.NurseProfit ?? 0 : 0)
                );
        }

        public async Task<decimal> GetTotalPlatformProfitAsync()
        {
            return await _context.ProfitDistributions.SumAsync(p => p.PlatformProfit ?? 0);
        }

        public async Task<decimal> GetTotalDriverPaymentsAsync()
        {
            return await _context.ProfitDistributions.SumAsync(p => p.DriverProfit ?? 0);
        }

        public async Task<decimal> GetTotalNursePaymentsAsync()
        {
            return await _context.ProfitDistributions.SumAsync(p => p.NurseProfit ?? 0);
        }
    }
}
