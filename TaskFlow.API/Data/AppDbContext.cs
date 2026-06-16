using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;
using TaskFlow.API.Models;


namespace TaskFlow.API.Data
{
    public class AppDbContext : DbContext
    {

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }


        public DbSet<User> Users => Set<User>();
        public DbSet<Project> Projects => Set<Project>();
        public DbSet<BoardColumn> BoardColumns => Set<BoardColumn>();
        public DbSet<TaskItem> TaskItems => Set<TaskItem>();


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasIndex(u => u.Email).IsUnique();

            modelBuilder.Entity<Project>().HasOne(p =>p.User).WithMany(u =>u.Projects).HasForeignKey(p =>p.UserId).OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<BoardColumn>().HasOne(c => c.Project).WithMany(p => p.Columns).HasForeignKey(c => c.ProjectId).OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<TaskItem>().HasOne(t => t.BoardColumn).WithMany(c => c.Tasks).HasForeignKey(t => t.BoardColumnId).OnDelete(DeleteBehavior.Cascade);

        }


        }

    }

