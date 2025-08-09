using System;
using System.ComponentModel.DataAnnotations;
using DomainLayer.Models.Identity_Module;
using DomainLayer.Models.Trip_Module;

namespace DomainLayer.Models.Withdrawal_Module
{
    public class ProfitDistribution
    {
        public int Id { get; set; }
        
        [Required]
        public int TripId { get; set; }
        public Trip Trip { get; set; }
        
        [Required]
        public decimal TotalTripPrice { get; set; }
        
        public decimal? DriverProfit { get; set; }
        public string? DriverId { get; set; }
        public ApplicationUser? Driver { get; set; }
        

        public decimal? NurseProfit { get; set; }
        public string? NurseId { get; set; }
        public ApplicationUser? Nurse { get; set; }
        

        public decimal? PlatformProfit { get; set; }
        public DateTime DistributionDate { get; set; } = DateTime.UtcNow;
        
        // نسبة التوزيع المستخدمة
        public decimal DriverPercentage { get; set; } = 0.40m; // 40%
        public decimal NursePercentage { get; set; } = 0.30m;  // 30%
        public decimal PlatformPercentage { get; set; } = 0.30m; // 30%
    }
} 