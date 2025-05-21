using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using StudentEmplacementApp.Models;

namespace StudentEmplacementApp.Data
{
    public class DataContext : IdentityDbContext<User>
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }

        public DbSet<Student> Students { get; set; }
        public DbSet<Choice> Choices { get; set; }
        public DbSet<Major> Majors { get; set; }
        public DbSet<University> Universities { get; set; }
        public DbSet<StudentChoice> StudentChoices { get; set; }
        

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Choice>()
                .HasKey(c => new { c.UniId, c.MajorId });

            modelBuilder.Entity<Choice>()
                .Property(c => c.Code)
                .HasComputedColumnSql("[UniId] * 100 + [MajorId]");

            modelBuilder.Entity<Choice>()
                .HasOne(c => c.Major)
                .WithMany(m => m.Choices)
                .HasForeignKey(c => c.MajorId)
                .OnDelete(DeleteBehavior.ClientSetNull);  

            modelBuilder.Entity<Choice>()
                .HasOne(c => c.University)
                .WithMany(u => u.Choices)
                .HasForeignKey(c => c.UniId)
                .OnDelete(DeleteBehavior.ClientSetNull);  

            modelBuilder.Entity<StudentChoice>()
                .HasKey(sc => new { sc.StudentId, sc.UniId, sc.MajorId });

            modelBuilder.Entity<StudentChoice>()
                .HasOne(sc => sc.Student)
                .WithMany(s => s.StudentChoices)
                .HasForeignKey(sc => sc.StudentId)
                .OnDelete(DeleteBehavior.Cascade);  

            modelBuilder.Entity<StudentChoice>()
                .HasOne(sc => sc.Choice)
                .WithMany(c => c.StudentChoices)
                .HasForeignKey(sc => new { sc.UniId, sc.MajorId })
                .OnDelete(DeleteBehavior.ClientSetNull);  

            modelBuilder.Entity<Major>()
                .Property(m => m.Language)
                .HasConversion(
                    v => (int)v,
                    v => (InstructionLanguage)v);

            modelBuilder.Entity<Student>()
                .HasOne(s => s.User)
                .WithOne(u => u.Student)
                .HasForeignKey<Student>(s => s.UserId)
                .OnDelete(DeleteBehavior.Cascade);

                
            //// 

            //modelBuilder.Entity<ResultCode>()
            //    .HasKey(c => c.ChoiceCode);

            //modelBuilder.Entity<Student>()
            //    .


            base.OnModelCreating(modelBuilder);
        }

    }
}
    