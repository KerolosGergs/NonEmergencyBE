using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DTOS.Nurse
{
    public class UpdateNurseDto
    {
        public string Certification { get; set; }
        public bool IsAvailable { get; set; }
        public string PhoneNumber { get; set; }
        public string FullName { get; set; }
    }
}
