using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLayer.Models
{
    public class GeneralResponse
    {
        public Boolean Success { get; set; }
        public string Message { get; set; }
        public dynamic Data { get; set; }

    }
}
