using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainLayer.Contracts;
using DomainLayer.Models.Withdrawal_Module;
using Microsoft.EntityFrameworkCore;
using Persistence.Data;
using Shared.DTOS.WithdrawalDTOS;

namespace Persistence.Repositories
{
    public class WithdrawalRequestRepository :GenericRepository<WithdrawalRequest>, IWithdrawalRequestRepository
    {
        private readonly ApplicationDbContext _context;
        public WithdrawalRequestRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
        public async Task<List<WithdrawalRequest>> GetUserWithdrawalRequestsAsync(string userId)
        {
            return await _context.WithdrawalRequests
                .Where(w => w.UserId == userId)
                .OrderByDescending(w => w.RequestDate)
                .ToListAsync();
        }
        public async Task<List<WithdrawalRequest>> GetPendingWithdrawalRequestsAsync()
        {
            return await _context.WithdrawalRequests
                .Where(w => w.Status == WithdrawalStatus.Pending)
                .OrderByDescending(w => w.RequestDate)
                .ToListAsync();
        }
        public async Task<List<WithdrawalRequest>> GetWithdrawalRequestsByStatusAsync(WithdrawalStatus status)
        {
            return await _context.WithdrawalRequests
                .Where(w => w.Status == status)
                .OrderByDescending(w => w.RequestDate)
                .ToListAsync();
        }
        public async Task<decimal> GetUserPendingWithdrawalAmountAsync(string userId)
        {
            return await _context.WithdrawalRequests
                .Where(w => w.UserId == userId && w.Status == WithdrawalStatus.Pending)
                .SumAsync(w => w.Amount);
        }
        public async Task<decimal> GetUserTotalWithdrawnAmountAsync(string userId)
        {
            return await _context.WithdrawalRequests
                .Where(w => w.UserId == userId && w.Status == WithdrawalStatus.Completed)
                .SumAsync(w => w.Amount);
        }
    }
}
