using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using WorkManagement.Models;

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
        public virtual DbSet<Admin> Admin { get; set; }
        public DbSet<WorkManagement.Models.Authentication> Authentication { get; set; }

    }
}
