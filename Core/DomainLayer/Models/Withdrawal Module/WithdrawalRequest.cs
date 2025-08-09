using System;
using System.ComponentModel.DataAnnotations;
using DomainLayer.Models.Identity_Module;
using Shared.DTOS.WithdrawalDTOS;

namespace DomainLayer.Models.Withdrawal_Module
{
    public class WithdrawalRequest
    {
        public int Id { get; set; }
        
        [Required]
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        
        [Required]
        [Range(1, double.MaxValue, ErrorMessage = "المبلغ يجب أن يكون أكبر من صفر")]
        public decimal Amount { get; set; }
        
        public DateTime RequestDate { get; set; } = DateTime.UtcNow;
        
        public WithdrawalStatus Status { get; set; } = WithdrawalStatus.Pending;
        
        public DateTime? ProcessedDate { get; set; }
        
        public string? AdminNotes { get; set; }
        
        public string? ProcessedByAdminId { get; set; }
        public ApplicationUser ProcessedByAdmin { get; set; }
    }

} 