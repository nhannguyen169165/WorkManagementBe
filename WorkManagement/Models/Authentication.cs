using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WorkManagement.Models
{
    public partial class Authentication 
    {
        public int Id { get; set; }
        public int Admin_id { get; set; }
        public int User_id { get; set; }
    }
}
