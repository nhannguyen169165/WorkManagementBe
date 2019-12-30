using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WorkManagement.Models
{
    public partial class Project
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public int User_id { get; set; }

        public virtual ICollection<StatusProject> StatusTask { get; set; }
    }
    public partial class ListUserInProject
    {
        public int Id { get; set; }
        public int User_id { get; set; }
        public int Project_Id { get; set; }
    }


    public partial class StatusProject
    {
        public int Id { get; set; }
        public string StatusName { get; set; }
        public int Serial { get; set; }
        public int ProjectId { get; set; }
        public virtual Project Project { get; set; }
    }
}
