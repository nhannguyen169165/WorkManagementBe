﻿using System;
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
        public virtual DbSet<StatusProject> StatusProject { get; set; }
        public virtual DbSet<Task> Task { get; set; }
        public virtual DbSet<Template> Template { get; set; }
        public virtual DbSet<StatusTemplate> StatusTemplate { get; set; }
    }
}
