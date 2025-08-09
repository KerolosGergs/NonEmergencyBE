using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace DomainLayer.Models.Identity_Module
{
    public class ApplicationUser :IdentityUser
    {
        public string FullName { get; set; }
        public decimal Balance { get; set; } = 0; // رصيد المستخدم المتاح للسحب
        public Patient PatientProfile { get; set; }
        public Driver DriverProfile { get; set; }
        public Nurse NurseProfile { get; set; }
        public Admin AdminProfile { get; set; }
    }
}
