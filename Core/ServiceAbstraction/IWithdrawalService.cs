using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Shared.DTOS.WithdrawalDTOS;

namespace ServiceAbstraction
{
    public interface IWithdrawalService
    {
        Task<WithdrawalRequestDTO> CreateWithdrawalRequestAsync(CreateWithdrawalRequestDTO request);
        Task<WithdrawalRequestDTO> GetWithdrawalRequestAsync(int requestId);
        Task<List<WithdrawalRequestDTO>> GetUserWithdrawalRequestsAsync(string userId);
        Task<List<WithdrawalRequestDTO>> GetAllWithdrawalRequestsAsync();
        Task<List<WithdrawalRequestDTO>> GetPendingWithdrawalRequestsAsync();
        Task<WithdrawalRequestDTO> ApproveWithdrawalRequestAsync(int requestId, string adminId, string? notes = null);
        Task<WithdrawalRequestDTO> RejectWithdrawalRequestAsync(int requestId, string adminId, string? notes = null);
        Task<WithdrawalRequestDTO> CompleteWithdrawalRequestAsync(int requestId, string adminId);
        Task<bool> CancelWithdrawalRequestAsync(int requestId, string userId);
    }
} 