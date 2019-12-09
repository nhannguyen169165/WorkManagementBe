using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WorkManagement.Models
{
    public partial class Admin
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Status { get; set; }
        public string Company { get; set; }
        public string Role { get; set; }
    }
}
