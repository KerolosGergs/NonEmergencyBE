using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DTOS.Driver
{
    public class UpdateDriverDto
    {
        public string LicenseNumber { get; set; }
        public string PhoneNumber { get; set; }
        public bool IsAvailable { get; set; }
        public string UserFullName { get; set; }
    }
}
