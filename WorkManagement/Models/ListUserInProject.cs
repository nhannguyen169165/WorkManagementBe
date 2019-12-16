using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WorkManagement.Models
{
    public partial class ListUserInProject
    {
        public int Id { get; set; }
        public int User_id { get; set; }
        public int Project_Id { get; set; }
    }
}
