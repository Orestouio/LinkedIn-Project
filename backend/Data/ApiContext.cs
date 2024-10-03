using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackendApp.auth;
using BackendApp.Model;
using Microsoft.EntityFrameworkCore;

namespace BackendApp.Data;

public class ApiContext
(DbContextOptions<ApiContext> options, IConfiguration configuration) 
: DbContext(options)
{

    private readonly IConfiguration configuration = configuration;

    public DbSet<AdminUser> AdminUsers {get; private set;} 
    public DbSet<RegularUser> RegularUsers {get; private set;}
    public DbSet<Post> Posts {get; private set;}
    public DbSet<JobPost> JobPosts {get; private set;}
    public DbSet<Notification> Notifications {get; private set;}
    public DbSet<Message> Messages {get; private set;}
    public DbSet<Connection> Connections {get; private set;}

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
        
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.UseIdentityAlwaysColumns();
        modelBuilder.Entity<RegularUser>()
            .HasOne( u => u.HideableInfo )
            .WithOne()
            .HasForeignKey<RegularUserHideableInfo>("UserId")
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();

        modelBuilder.Entity<Message>()
            .HasOne( m => m.SentBy )
            .WithMany()
            .OnDelete(DeleteBehavior.Restrict)
            .HasForeignKey("SentId");

        modelBuilder.Entity<Message>()
            .HasOne( m => m.SentTo )
            .WithMany()
            .OnDelete(DeleteBehavior.Restrict)
            .HasForeignKey("ReceivedId");
            
        modelBuilder.Entity<PostBase>()
            .HasOne( p => p.PostedBy )
            .WithMany()
            .OnDelete(DeleteBehavior.Restrict)
            .HasForeignKey("PostedById");
        modelBuilder.Entity<PostBase>()
            .HasMany(p => p.InterestedUsers)
            .WithMany("LikedPosts");
        modelBuilder.Entity<PostBase>()
            .HasMany(p => p.PostFiles)
            .WithMany("PostsUsedIn");
        
        modelBuilder.Entity<Post>()
            .HasMany(p => p.Replies)
            .WithOne()
            .OnDelete(DeleteBehavior.Restrict)
            .HasForeignKey("OriginalPost");
        
        modelBuilder.Entity<Notification>()
            .HasOne(n => n.ToUser)
            .WithMany()
            .OnDelete(DeleteBehavior.Restrict)
            .HasForeignKey("NotificationsIds");
        modelBuilder.Entity<Notification>()
            .HasOne(n => n.AssociatedPost)
            .WithMany()
            .OnDelete(DeleteBehavior.Restrict)
            .HasForeignKey("NotificIds");

        modelBuilder.Entity<Connection>()
            .HasOne( n => n.SentBy)
            .WithMany()
            .OnDelete(DeleteBehavior.Restrict)
            .HasForeignKey("UsersSentNotificationId");
        modelBuilder.Entity<Connection>()
            .HasOne( n => n.SentTo)
            .WithMany()
            .OnDelete(DeleteBehavior.Restrict)
            .HasForeignKey("UsersReceivedNotificationId");
        


        // modelBuilder
    }
}
