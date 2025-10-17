using DataAccess.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Context
{
	public class AppDbContext: IdentityDbContext<ApplicationUser>
	{
		public AppDbContext(DbContextOptions<AppDbContext> options)
		: base(options) { }
		public DbSet<Faculty> Faculties { get; set; }
		public DbSet<Department> Departments { get; set; }
		public DbSet<Doctor> Doctors { get; set; }
		public DbSet<Course> Courses { get; set; }
		public DbSet<Student> Students { get; set; }
		public DbSet<CourseRegistration> CourseRegistration { get; set; }
		public DbSet<DoctorCourse> DoctorCourses { get; set; }
		public DbSet<RegistrationPeriod> RegistrationPeriods { get; set; }
		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			modelBuilder.Entity<CourseRegistration>()
				.HasKey(sc => new { sc.StudentId, sc.CourseId });

			modelBuilder.Entity<CourseRegistration>()
				.HasOne(sc => sc.Student)
				.WithMany(s => s.CourseRegistrations)
				.HasForeignKey(sc => sc.StudentId)
				.OnDelete(DeleteBehavior.Restrict);

			modelBuilder.Entity<CourseRegistration>()
				.HasOne(sc => sc.Course)
				.WithMany(c => c.CourseRegistration)
				.HasForeignKey(sc => sc.CourseId)
				.OnDelete(DeleteBehavior.Restrict);

			modelBuilder.Entity<DoctorCourse>()
				.HasKey(dc => new { dc.DoctorId, dc.CourseId });

			modelBuilder.Entity<DoctorCourse>()
				.HasOne(dc => dc.Doctor)
				.WithMany(d => d.DoctorCourses)
				.HasForeignKey(dc => dc.DoctorId)
				.OnDelete(DeleteBehavior.Restrict);

			modelBuilder.Entity<DoctorCourse>()
				.HasOne(dc => dc.Course)
				.WithMany(c => c.DoctorCourses)
				.HasForeignKey(dc => dc.CourseId)
				.OnDelete(DeleteBehavior.Restrict);

			
			modelBuilder.Entity<Department>()
				.HasOne(d => d.Faculty)
				.WithMany(f => f.Departments)
				.HasForeignKey(d => d.FacultyId)
				.OnDelete(DeleteBehavior.Cascade);
		}
	}
}
