using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WorkManagement.Models
{
    public partial class Project
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime FinishDate { get; set; }
        public int WorkingTimePerDay { get; set; }
        public string WorkingDayPerWeek { get; set; }
        public string Status { get; set; }
        public string Color { get; set; }
        public int User_id { get; set; }
        [ForeignKey("User_id")]
        public virtual User user { get; set; }
        public virtual ICollection<StatusProject> StatusTask { get; set; }
        public virtual ICollection<Task> TaskList { get; set; }
        public virtual ICollection<ListUserInProject> ListMember { get; set; }
    }
    public partial class ListUserInProject
    {
        public int Id { get; set; }
        public int User_id { get; set; }
        public int Project_Id { get; set; }
        [ForeignKey("Project_Id")]
        public virtual Project Project { get; set; }
        [ForeignKey("User_id")]
        public virtual User user{ get; set; }
    }


    public partial class StatusProject
    {
        public int Id { get; set; }
        public string StatusName { get; set; }
        public int Serial { get; set; }
        public int Relation { get; set; }
        public int ProjectId { get; set; }
        [ForeignKey("ProjectId")]
        public virtual Project Project { get; set; }
        public virtual ICollection<Task> TaskList { get; set; }
    }
}
