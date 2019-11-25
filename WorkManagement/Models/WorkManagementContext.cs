using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace WorkManagement.Models
{
    public partial class WorkManagementContext : DbContext
    {
        public WorkManagementContext()
        {
        }

        public WorkManagementContext(DbContextOptions<WorkManagementContext> options)
            : base(options)
        {
        }

        public virtual DbSet<User> User { get; set; }

    }
}
