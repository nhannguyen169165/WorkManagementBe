using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WorkManagement.Models
{
    public partial class Template
    {
        public int Id { get; set; }
        public string TemplateName { get; set; }
        public int AdminId { get; set; }
        public virtual ICollection<StatusTemplate> Status { get; set; }
        
    }
    public partial class StatusTemplate
    {
        public int Id { get; set; }
        public string StatusName { get; set; }
        public int Serial { get; set; }
        public int Relation { get; set; }
        public int TemplateId { get; set; }
        [ForeignKey("TemplateId")]
        public virtual Template Template { get; set; }
    }

}
