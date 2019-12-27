using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WorkManagement.Models
{
    public partial class Template
    {
        public int Id { get; set; }
        public string TemplateName { get; set; }
        public ICollection<Status> Status { get; set; }
    }

    public partial class Status
    {
       
        public int  StatusId { get; set; }
        public string StatusName { get; set; }
        public int Serial { get; set; }
        public int TemplateId { get; set; }
        public Template Template { get; set; }
    }
}
