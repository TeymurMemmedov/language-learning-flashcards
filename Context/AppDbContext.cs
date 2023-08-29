using LearningCards.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata;

namespace LearningCards.Context
{
    public class AppDbContext : DbContext
    {
        public AppDbContext() { }
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Role> Roles { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<TermSet> TermSets { get; set; }
        public DbSet<Term> Terms { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

           
                modelBuilder.Entity<Role>().HasData(
                    new Role { Id = 1, Name = "Student",Description = "Student can make CRUD on his/her sets" },
                    new Role { Id = 3, Name = "Admin" , Description = "Admin can delete users, sets" }
                );
                
           

        }
    }
}
