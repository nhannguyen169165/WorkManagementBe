using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WorkManagement.Models
{
    public partial class Task
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Color { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime FinishDate { get; set; }
        public int Hours { get; set; }
        public string Priority { get; set; }
        public int StatusId { get; set; }
        public int TaskOwnerId { get; set; }
        [ForeignKey("StatusId ")]
        public virtual StatusProject StatusProject { get; set; }
        [ForeignKey("TaskOwnerId ")]
        public virtual User User { get; set; }
    }
}
