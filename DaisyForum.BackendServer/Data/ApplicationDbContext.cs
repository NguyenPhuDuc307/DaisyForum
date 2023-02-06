using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DaisyForum.BackendServer.Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DaisyForum.BackendServer.Data
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<IdentityRole>().Property(x => x.Id).HasMaxLength(50).IsUnicode(false);
            builder.Entity<User>().Property(x => x.Id).HasMaxLength(50).IsUnicode(false);

            builder.Entity<LabelInKnowledgeBase>()
                        .HasKey(c => new { c.LabelId, c.KnowledgeBaseId });

            builder.Entity<Permission>()
                       .HasKey(c => new { c.RoleId, c.FunctionId, c.CommandId });

            builder.Entity<Vote>()
                        .HasKey(c => new { c.KnowledgeBaseId, c.UserId });

            builder.Entity<CommandInFunction>()
                       .HasKey(c => new { c.CommandId, c.FunctionId });

            builder.HasSequence("Daisy_Forum_Seq");
        }

        public DbSet<Command> Commands { set; get; } = default!;
        public DbSet<CommandInFunction> CommandInFunctions { set; get; } = default!;
        public DbSet<ActivityLog> ActivityLogs { set; get; } = default!;
        public DbSet<Category> Categories { set; get; } = default!;
        public DbSet<Comment> Comments { set; get; } = default!;
        public DbSet<Function> Functions { set; get; } = default!;
        public DbSet<KnowledgeBase> KnowledgeBases { set; get; } = default!;
        public DbSet<Label> Labels { set; get; } = default!;
        public DbSet<LabelInKnowledgeBase> LabelInKnowledgeBases { set; get; } = default!;
        public DbSet<Permission> Permissions { set; get; } = default!;
        public DbSet<Report> Reports { set; get; } = default!;
        public DbSet<Vote> Votes { set; get; } = default!;
        public DbSet<Attachment> Attachments { get; set; } = default!;
    }
}