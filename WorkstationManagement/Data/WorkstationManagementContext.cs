using WorkstationManagement.Models;
using Microsoft.EntityFrameworkCore;
using System;
using Metsys.Bson;
using WorkstationManagement.Utils;

namespace WorkstationManagement.Data;

public class WorkstationManagementContext : DbContext 
{
    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<WorkPosition> WorkPositions { get; set; }
    public DbSet<UserWorkPosition> UserWorkPositions { get; set; }

    //static readonly string connectionString = "Server=localhost; User ID=root; Password=ivantmysqlpass#; Database=ProductionDB";
    static readonly string? connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING");

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    => optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
        
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>()
            .HasOne(u => u.Role) 
            .WithMany(r => r.Users) 
            .HasForeignKey(u => u.RoleId); 

        modelBuilder.Entity<UserWorkPosition>()
            .HasOne(uwp => uwp.User) 
            .WithMany(u => u.UserWorkPositions) 
            .HasForeignKey(uwp => uwp.UserId);

        modelBuilder.Entity<UserWorkPosition>()
            .HasOne(uwp => uwp.WorkPosition) 
            .WithMany(u => u.UserWorkPositions) 
            .HasForeignKey(uwp => uwp.WorkPositionId);

        // Seeding
        modelBuilder.Entity<Role>(r => 
        {
            r.HasData
            (
                new Role {Id = 1, RoleName = "Admin", Description = "Manages the production database"},
                new Role {Id = 2, RoleName = "User", Description = "Works in production"}
            );
        });

        modelBuilder.Entity<User>(u => 
        {
            u.HasData
            (
                new User {
                            Id = 1, 
                            FirstName = "ivan", 
                            LastName = "tvrdojevic", 
                            Username = "admin", 
                            Password = Utils.Helper.ComputeSha256Hash("admin"), 
                            RoleId = 1,
                            ChangePwNeeded = false
                          }
            );
        });

        base.OnModelCreating(modelBuilder);
    }
}