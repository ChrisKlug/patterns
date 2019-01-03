using Microsoft.EntityFrameworkCore;
using Repository.Entities;

namespace Repository.Data
{
    public class UsersContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=data.db");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>(x =>
            {
                x.Property<int>("UserId").ValueGeneratedOnAdd();
                x.Property<string>("FirstName");
                x.Property<bool>("IsAdmin");
                x.Property<bool>("IsDeleted");
                x.Property<string>("LastName");

                x.HasKey("UserId");

                x.ToTable("Users");
            });
        }

        public DbSet<User> Users { get; set; }
    }
}
