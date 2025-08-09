using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DomainLayer.Models.Withdrawal_Module;

namespace DomainLayer.Contracts
{
    public interface IProfitDistributionRepository : IGenericRepository<ProfitDistribution>
    {
        Task<ProfitDistribution> GetByTripIdAsync(int tripId);
        Task<List<ProfitDistribution>> GetUserProfitHistoryAsync(string userId);
        Task<List<ProfitDistribution>> GetAllProfitDistributionsAsync();
        Task<decimal> GetTotalUserEarningsAsync(string userId);
        Task<decimal> GetTotalPlatformProfitAsync();
        Task<decimal> GetTotalDriverPaymentsAsync();
        Task<decimal> GetTotalNursePaymentsAsync();
    }
} 