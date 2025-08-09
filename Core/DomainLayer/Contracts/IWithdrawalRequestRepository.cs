using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DomainLayer.Models.Withdrawal_Module;
using Shared.DTOS.WithdrawalDTOS;

namespace DomainLayer.Contracts
{
    public interface IWithdrawalRequestRepository : IGenericRepository<WithdrawalRequest>
    {
        Task<List<WithdrawalRequest>> GetUserWithdrawalRequestsAsync(string userId);
        Task<List<WithdrawalRequest>> GetPendingWithdrawalRequestsAsync();
        Task<List<WithdrawalRequest>> GetWithdrawalRequestsByStatusAsync(WithdrawalStatus status);
        Task<decimal> GetUserPendingWithdrawalAmountAsync(string userId);
        Task<decimal> GetUserTotalWithdrawnAmountAsync(string userId);
    }
} 