using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DomainLayer.Contracts;
using DomainLayer.Models.Identity_Module;
using DomainLayer.Models.Request_Module;
using DomainLayer.Models.Withdrawal_Module;
using Microsoft.AspNetCore.Identity;
using ServiceAbstraction;
using Shared.DTOS.WithdrawalDTOS;

namespace Core.Service
{
    public class WithdrawalService : IWithdrawalService
    {
        private readonly IWithdrawalRequestRepository _withdrawalRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        public WithdrawalService(
            IWithdrawalRequestRepository withdrawalRepository,

            UserManager<ApplicationUser> userManager)
        {
            _withdrawalRepository = withdrawalRepository;
            _userManager = userManager;
        }

        public async Task<WithdrawalRequestDTO> CreateWithdrawalRequestAsync(CreateWithdrawalRequestDTO request)
        {
            var user = await _userManager.FindByIdAsync(request.UserId);
            if (user == null)
                throw new InvalidOperationException("المستخدم غير موجود");

            if (user.Balance < request.Amount)
                throw new InvalidOperationException("الرصيد غير كافي لطلب السحب");

            var pendingAmount = await _withdrawalRepository.GetUserPendingWithdrawalAmountAsync(request.UserId);
            if (user.Balance - pendingAmount < request.Amount)
                throw new InvalidOperationException("الرصيد المتاح بعد خصم الطلبات المعلقة غير كافي");

            var withdrawalRequest = new WithdrawalRequest
            {
                UserId = request.UserId,
                Amount = request.Amount,
                RequestDate = DateTime.UtcNow,
                Status = WithdrawalStatus.Pending
            };

            await _withdrawalRepository.AddAsync(withdrawalRequest);
            user.Balance -= request.Amount;
            await _userManager.UpdateAsync(user);

            return new WithdrawalRequestDTO
            {
                Id = withdrawalRequest.Id,
                UserId = withdrawalRequest.UserId,
                UserName = user.FullName,
                UserType = GetUserType(user),
                Amount = withdrawalRequest.Amount,
                RequestDate = withdrawalRequest.RequestDate,
                Status = withdrawalRequest.Status
            };
        }

        public async Task<WithdrawalRequestDTO> GetWithdrawalRequestAsync(int requestId)
        {
            var request = await _withdrawalRepository.GetByIdAsync(requestId);
            if (request == null) return null;

            var user = await _userManager.FindByIdAsync(request.UserId);
            var admin = request.ProcessedByAdminId != null ? await _userManager.FindByIdAsync(request.ProcessedByAdminId) : null;

            return new WithdrawalRequestDTO
            {
                Id = request.Id,
                UserId = request.UserId,
                UserName = user?.FullName,
                UserType = user != null ? GetUserType(user) : null,
                Amount = request.Amount,
                RequestDate = request.RequestDate,
                Status = request.Status,
                ProcessedDate = request.ProcessedDate,
                AdminNotes = request.AdminNotes,
                ProcessedByAdminId = request.ProcessedByAdminId,
                ProcessedByAdminName = admin?.FullName
            };
        }

        public async Task<List<WithdrawalRequestDTO>> GetUserWithdrawalRequestsAsync(string userId)
        {
            var requests = await _withdrawalRepository.GetUserWithdrawalRequestsAsync(userId);
            var user = await _userManager.FindByIdAsync(userId);

            return requests.Select(r => new WithdrawalRequestDTO
            {
                Id = r.Id,
                UserId = r.UserId,
                UserName = user?.FullName,
                UserType = user != null ? GetUserType(user) : null,
                Amount = r.Amount,
                RequestDate = r.RequestDate,
                Status = r.Status,
                ProcessedDate = r.ProcessedDate,
                AdminNotes = r.AdminNotes,
                ProcessedByAdminId = r.ProcessedByAdminId
            }).ToList();
        }

        public async Task<List<WithdrawalRequestDTO>> GetAllWithdrawalRequestsAsync()
        {
            var requests = await _withdrawalRepository.GetAllAsync();
            var result = new List<WithdrawalRequestDTO>();

            foreach (var request in requests)
            {
                var user = await _userManager.FindByIdAsync(request.UserId);
                var admin = request.ProcessedByAdminId != null ? await _userManager.FindByIdAsync(request.ProcessedByAdminId) : null;

                result.Add(new WithdrawalRequestDTO
                {
                    Id = request.Id,
                    UserId = request.UserId,
                    UserName = user?.FullName,
                    UserType = user != null ? GetUserType(user) : null,
                    Amount = request.Amount,
                    RequestDate = request.RequestDate,
                    Status = request.Status,
                    ProcessedDate = request.ProcessedDate,
                    AdminNotes = request.AdminNotes,
                    ProcessedByAdminId = request.ProcessedByAdminId,
                    ProcessedByAdminName = admin?.FullName
                });
            }

            return result;
        }

        public async Task<List<WithdrawalRequestDTO>> GetPendingWithdrawalRequestsAsync()
        {
            var requests = await _withdrawalRepository.GetPendingWithdrawalRequestsAsync();
            var result = new List<WithdrawalRequestDTO>();

            foreach (var request in requests)
            {
                var user = await _userManager.FindByIdAsync(request.UserId);

                result.Add(new WithdrawalRequestDTO
                {
                    Id = request.Id,
                    UserId = request.UserId,
                    UserName = user?.FullName,
                    UserType = user != null ? GetUserType(user) : null,
                    Amount = request.Amount,
                    RequestDate = request.RequestDate,
                    Status = request.Status,
                    ProcessedDate = request.ProcessedDate,
                    AdminNotes = request.AdminNotes,
                    ProcessedByAdminId = request.ProcessedByAdminId
                });
            }

            return result;
        }

        public async Task<WithdrawalRequestDTO> ApproveWithdrawalRequestAsync(int requestId, string adminId, string? notes = null)
        {
            var request = await _withdrawalRepository.GetByIdAsync(requestId);
            if (request == null)
                throw new InvalidOperationException("طلب السحب غير موجود");

            if (request.Status != WithdrawalStatus.Pending)
                throw new InvalidOperationException("لا يمكن الموافقة على طلب غير معلق");

            request.Status = WithdrawalStatus.Approved;
            request.ProcessedDate = DateTime.UtcNow;
            request.ProcessedByAdminId = adminId;
            request.AdminNotes = notes;

            _withdrawalRepository.Update(request);

            var user = await _userManager.FindByIdAsync(request.UserId);
            var admin = await _userManager.FindByIdAsync(adminId);

            return new WithdrawalRequestDTO
            {
                Id = request.Id,
                UserId = request.UserId,
                UserName = user?.FullName,
                UserType = user != null ? GetUserType(user) : null,
                Amount = request.Amount,
                RequestDate = request.RequestDate,
                Status = request.Status,
                ProcessedDate = request.ProcessedDate,
                AdminNotes = request.AdminNotes,
                ProcessedByAdminId = request.ProcessedByAdminId,
                ProcessedByAdminName = admin?.FullName
            };
        }

        public async Task<WithdrawalRequestDTO> RejectWithdrawalRequestAsync(int requestId, string adminId, string? notes = null)
        {
            var request = await _withdrawalRepository.GetByIdAsync(requestId);
            if (request == null)
                throw new InvalidOperationException("طلب السحب غير موجود");

            if (request.Status != WithdrawalStatus.Pending)
                throw new InvalidOperationException("لا يمكن رفض طلب غير معلق");

            request.Status = WithdrawalStatus.Rejected;
            request.ProcessedDate = DateTime.UtcNow;
            request.ProcessedByAdminId = adminId;
            request.AdminNotes = notes;

            _withdrawalRepository.Update(request);

            var user = await _userManager.FindByIdAsync(request.UserId);
            if (user != null)
            {
                user.Balance += request.Amount;
                await _userManager.UpdateAsync(user);
            }

            var admin = await _userManager.FindByIdAsync(adminId);

            return new WithdrawalRequestDTO
            {
                Id = request.Id,
                UserId = request.UserId,
                UserName = user?.FullName,
                UserType = user != null ? GetUserType(user) : null,
                Amount = request.Amount,
                RequestDate = request.RequestDate,
                Status = request.Status,
                ProcessedDate = request.ProcessedDate,
                AdminNotes = request.AdminNotes,
                ProcessedByAdminId = request.ProcessedByAdminId,
                ProcessedByAdminName = admin?.FullName
            };
        }

        public async Task<WithdrawalRequestDTO> CompleteWithdrawalRequestAsync(int requestId, string adminId)
        {
            var request = await _withdrawalRepository.GetByIdAsync(requestId);
            if (request == null)
                throw new InvalidOperationException("طلب السحب غير موجود");

            if (request.Status != WithdrawalStatus.Approved)
                throw new InvalidOperationException("لا يمكن إكمال طلب غير موافق عليه");

            request.Status = WithdrawalStatus.Completed;
            request.ProcessedDate = DateTime.UtcNow;
            request.ProcessedByAdminId = adminId;

            _withdrawalRepository.Update(request);

            var user = await _userManager.FindByIdAsync(request.UserId);
            var admin = await _userManager.FindByIdAsync(adminId);

            return new WithdrawalRequestDTO
            {
                Id = request.Id,
                UserId = request.UserId,
                UserName = user?.FullName,
                UserType = user != null ? GetUserType(user) : null,
                Amount = request.Amount,
                RequestDate = request.RequestDate,
                Status = request.Status,
                ProcessedDate = request.ProcessedDate,
                AdminNotes = request.AdminNotes,
                ProcessedByAdminId = request.ProcessedByAdminId,
                ProcessedByAdminName = admin?.FullName
            };
        }

        public async Task<bool> CancelWithdrawalRequestAsync(int requestId, string userId)
        {
            var request = await _withdrawalRepository.GetByIdAsync(requestId);
            if (request == null)
                throw new InvalidOperationException("طلب السحب غير موجود");

            if (request.UserId != userId)
                throw new UnauthorizedAccessException("غير مصرح لك بإلغاء هذا الطلب");

            if (request.Status != WithdrawalStatus.Pending)
                throw new InvalidOperationException("لا يمكن إلغاء طلب غير معلق");

            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                user.Balance += request.Amount;
                await _userManager.UpdateAsync(user);
            }

            _withdrawalRepository.Delete(request);
            return true;
        }

        private string GetUserType(ApplicationUser user)
        {
            if (user.DriverProfile != null)
                return "Driver";
            if (user.NurseProfile != null)
                return "Nurse";
            if (user.AdminProfile != null)
                return "Admin";
            return "Patient";
        }
    }
}
