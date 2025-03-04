using WorkstationManagement.Models;
using Microsoft.EntityFrameworkCore;
using System.Data.Common;
using System;
using System.Linq;
using WorkstationManagement.Utils;
using System.IO;
using System.Threading.Tasks;

namespace WorkstationManagement.Data;

public class WorkstationManagementContext : DbContext 
{
    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<WorkPosition> WorkPositions { get; set; }
    public DbSet<UserWorkPosition> UserWorkPositions { get; set; }

    //static readonly string connectionString = "Server=localhost; User ID=root; Password=ivantmysqlpass#; Database=ProductionDB";
    static readonly string? connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING");
    private readonly StreamWriter _logStream = new StreamWriter("Data/EFCoreLogs.txt", append: true);

    public override void Dispose()
    {
        base.Dispose();
        _logStream.Dispose();
    }

    public override async ValueTask DisposeAsync()
    {
        await base.DisposeAsync();
        await _logStream.DisposeAsync();
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    => optionsBuilder
        .UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
        .UseSeeding((context, _) =>
        {
            var role = context.Set<Role>().FirstOrDefault(r => r.RoleName == "Admin");
            if(role == null)
            {
                context.Set<Role>().Add(new Role{Id = 1, RoleName = "Admin", Description = "Controls the production database"});
            }

            role = context.Set<Role>().FirstOrDefault(r => r.RoleName == "User");
            if(role == null)
            {
                context.Set<Role>().Add(new Role{Id = 2, RoleName = "User", Description = "Works in production"});
            }

            var user = context.Set<User>().FirstOrDefault(u => u.Id == 1);
            if(user == null)
            {               
            context.Set<User>().Add(new User{
                                                Id = 1,
                                                FirstName = "ivan",
                                                LastName = "tvrdojevic",
                                                Username = "admin",
                                                Password = Helper.ComputeSha256Hash("adminpass"),
                                                RoleId = 1
                                            });
            }
            context.SaveChanges();
        })
        .UseAsyncSeeding(async (context, _, cancellationToken) =>
        {
            var adminRole = await context.Set<Role>().FirstOrDefaultAsync(r => r.RoleName == "Admin");
            if(adminRole == null)
            {
                context.Set<Role>().Add(new Role{Id = 1, RoleName = "Admin", Description = "Controls the production database"});
            }

            var userRole = await context.Set<Role>().FirstOrDefaultAsync(r => r.RoleName == "User");
            if(userRole == null)
            {
                context.Set<Role>().Add(new Role{Id = 2, RoleName = "User", Description = "Works in production"});
            }

            var user = await context.Set<User>().FirstOrDefaultAsync(u => u.Id == 1);
            if(user == null)
            {               
            context.Set<User>().Add(new User{
                                                 Id = 1,
                                                FirstName = "ivan",
                                                LastName = "tvrdojevic",
                                                Username = "admin",
                                                Password = Helper.ComputeSha256Hash("adminpass"),
                                                RoleId = 1
                                            });
            }
            await context.SaveChangesAsync(cancellationToken);
        });

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

        base.OnModelCreating(modelBuilder);
    }
}