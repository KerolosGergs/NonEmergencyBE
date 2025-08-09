using System;
using Shared.DTOS.TripDTOs;

namespace Shared.DTOS.ProfitDTOS
{
    public class ProfitDistributionDTO
    {
        public int Id { get; set; }
        public int TripId { get; set; }
        public TripDTO Trip { get; set; }
        public decimal TotalTripPrice { get; set; }
        
        // أرباح السائق
        public decimal? DriverProfit { get; set; }
        public string? DriverId { get; set; }
        public string? DriverName { get; set; }
        
        // أرباح الممرضة
        public decimal? NurseProfit { get; set; }
        public string? NurseId { get; set; }
        public string? NurseName { get; set; }
        
        // أرباح الموقع
        public decimal? PlatformProfit { get; set; }
        
        public DateTime DistributionDate { get; set; }
        
        // نسب التوزيع
        public decimal DriverPercentage { get; set; }
        public decimal NursePercentage { get; set; }
        public decimal PlatformPercentage { get; set; }
    }
    
    public class UserBalanceDTO
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string UserType { get; set; } // Driver, Nurse, Admin
        public decimal CurrentBalance { get; set; }
        public decimal TotalEarnings { get; set; }
        public decimal TotalWithdrawn { get; set; }
        public decimal PendingWithdrawals { get; set; }
    }
    
    public class ProfitSummaryDTO
    {
        public decimal TotalPlatformRevenue { get; set; }
        public decimal TotalDriverPayments { get; set; }
        public decimal TotalNursePayments { get; set; }
        public decimal TotalPlatformProfit { get; set; }
        public int TotalTrips { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
    }
} 