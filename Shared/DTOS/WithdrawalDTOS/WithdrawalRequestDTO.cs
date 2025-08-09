using System;
using System.ComponentModel.DataAnnotations;

namespace Shared.DTOS.WithdrawalDTOS
{
    public class WithdrawalRequestDTO
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string UserType { get; set; } // Driver, Nurse
        public decimal Amount { get; set; }
        public DateTime RequestDate { get; set; }
        public WithdrawalStatus Status { get; set; }
        public DateTime? ProcessedDate { get; set; }
        public string? AdminNotes { get; set; }
        public string? ProcessedByAdminId { get; set; }
        public string? ProcessedByAdminName { get; set; }
    }
    
    public class CreateWithdrawalRequestDTO
    {
        [Required]
        public string UserId { get; set; }
        
        [Required]
        [Range(1, double.MaxValue, ErrorMessage = "المبلغ يجب أن يكون أكبر من صفر")]
        public decimal Amount { get; set; }
    }
    
    public class UpdateWithdrawalRequestDTO
    {
        [Required]
        public int RequestId { get; set; }
        
        [Required]
        public WithdrawalStatus Status { get; set; }
        
        public string? AdminNotes { get; set; }
    }
    
    public class WithdrawalSummaryDTO
    {
        public int TotalRequests { get; set; }
        public int PendingRequests { get; set; }
        public int ApprovedRequests { get; set; }
        public int RejectedRequests { get; set; }
        public int CompletedRequests { get; set; }
        public decimal TotalRequestedAmount { get; set; }
        public decimal TotalApprovedAmount { get; set; }
        public decimal TotalCompletedAmount { get; set; }
    }
    
    public enum WithdrawalStatus
    {
        Pending = 0,    // في الانتظار
        Approved = 1,   // موافق عليه
        Rejected = 2,   // مرفوض
        Completed = 3   // مكتمل
    }
} 