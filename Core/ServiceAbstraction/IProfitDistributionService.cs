using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Shared.DTOS.ProfitDTOS;

namespace ServiceAbstraction
{
    public interface IProfitDistributionService
    {
        Task<ProfitDistributionDTO> DistributeTripProfitsAsync(int tripId);
        Task<decimal> GetUserBalanceAsync(string userId);
        Task<List<ProfitDistributionDTO>> GetUserProfitHistoryAsync(string userId);
        Task<List<ProfitDistributionDTO>> GetAllProfitDistributionsAsync();
        Task<ProfitDistributionDTO> GetProfitDistributionByTripAsync(int tripId);
    }
} 