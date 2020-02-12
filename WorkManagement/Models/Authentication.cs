using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WorkManagement.Models
{
    public partial class Authentication 
    {
        public int Id { get; set; }
        public int Admin_id { get; set; }
        public int User_id { get; set; }
        [ForeignKey("User_id")]
        public virtual User user { get; set; }
        [ForeignKey("Admin_id")]
        public virtual Admin admin { get; set; }
    }
}
