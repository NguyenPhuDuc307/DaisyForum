using DaisyForum.BackendServer.Data.Entities;
using DaisyForum.BackendServer.Data.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace DaisyForum.BackendServer.Data;

public class ApplicationDbContext : IdentityDbContext<User>
{
    public ApplicationDbContext(DbContextOptions options) : base(options)
    {
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken))
    {
        IEnumerable<EntityEntry> modified = ChangeTracker.Entries()
            .Where(e => e.State == EntityState.Modified || e.State == EntityState.Added);
        foreach (EntityEntry item in modified)
        {
            if (item.Entity is IDateTracking changedOrAddedItem)
            {
                if (item.State == EntityState.Added)
                {
                    changedOrAddedItem.CreateDate = DateTime.Now;
                }
                else
                {
                    changedOrAddedItem.LastModifiedDate = DateTime.Now;
                }
            }
        }
        return base.SaveChangesAsync(cancellationToken);
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

        builder.Entity<Room>().HasOne(s => s.Admin)
                .WithMany(u => u.Rooms)
                .IsRequired();

        builder.Entity<Message>()
                .HasOne(s => s.ToRoom)
                .WithMany(m => m.Messages)
                .HasForeignKey(s => s.ToRoomId)
                .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Follower>()
                        .HasKey(c => new { c.OwnerUserId, c.FollowerId });

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
    public DbSet<Room> Rooms { get; set; } = default!;
    public DbSet<Message> Messages { get; set; } = default!;
    public DbSet<Follower> Followers { get; set; } = default!;
}