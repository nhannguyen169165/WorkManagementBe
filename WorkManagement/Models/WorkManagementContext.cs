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
        public virtual DbSet<Authentication> Authentication { get; set; }
        public virtual DbSet<Project> Project { get; set; }
        public virtual DbSet<ListUserInProject> ListUserInProject { get; set; }
    }
}
