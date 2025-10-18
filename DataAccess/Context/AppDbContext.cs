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
	public class AppDbContext : IdentityDbContext<ApplicationUser>
	{
		public AppDbContext(DbContextOptions<AppDbContext> options)
		: base(options) { }
		public DbSet<Faculty> Faculties { get; set; }
		public DbSet<Department> Departments { get; set; }
		public DbSet<Doctor> Doctors { get; set; }
		public DbSet<Course> Courses { get; set; }
		public DbSet<Student> Students { get; set; }
		public DbSet<CoursePrerequisite> CoursePrerequisites { get; set; }

		public DbSet<CourseRegistration> CourseRegistration { get; set; }

		public DbSet<DoctorCourse> DoctorCourses { get; set; }
		public DbSet<RegistrationPeriod> RegistrationPeriods { get; set; }

		// NEW - Class Management DbSets
		public DbSet<Class> Classes { get; set; }
		public DbSet<ClassSchedule> ClassSchedules { get; set; }
		public DbSet<ClassRegistration> ClassRegistrations { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			// CoursePrerequisite Configuration - Two separate relationships
			modelBuilder.Entity<CoursePrerequisite>()
				.HasKey(cp => cp.Id);

			// Relationship 1: Course -> CoursePrerequisite (the course that has prerequisites)
			modelBuilder.Entity<CoursePrerequisite>()
				.HasOne(cp => cp.Course)
				.WithMany(c => c.PrerequisiteFor)
				.HasForeignKey(cp => cp.CourseId)
				.OnDelete(DeleteBehavior.Restrict);

			// Relationship 2: PrerequisiteCourse -> CoursePrerequisite (the course that IS a prerequisite)
			modelBuilder.Entity<CoursePrerequisite>()
				.HasOne(cp => cp.PrerequisiteCourse)
				.WithMany()  // No navigation property on the other side for this relationship
				.HasForeignKey(cp => cp.PrerequisiteCourseId)
				.OnDelete(DeleteBehavior.Restrict);

			// CourseRegistration Configuration
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

			// DoctorCourse Configuration
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

			// Department Configuration
			modelBuilder.Entity<Department>()
				.HasOne(d => d.Faculty)
				.WithMany(f => f.Departments)
				.HasForeignKey(d => d.FacultyId)
				.OnDelete(DeleteBehavior.Cascade);

			// Class Configuration
			modelBuilder.Entity<Class>()
				.HasOne(c => c.Course)
				.WithMany()
				.HasForeignKey(c => c.CourseId)
				.OnDelete(DeleteBehavior.Restrict);

			modelBuilder.Entity<Class>()
				.HasOne(c => c.Doctor)
				.WithMany()
				.HasForeignKey(c => c.DoctorId)
				.OnDelete(DeleteBehavior.Restrict);

			modelBuilder.Entity<Class>()
				.HasMany(c => c.Schedules)
				.WithOne(cs => cs.Class)
				.HasForeignKey(cs => cs.ClassId)
				.OnDelete(DeleteBehavior.Cascade);

			modelBuilder.Entity<Class>()
				.HasMany(c => c.StudentRegisterations)
				.WithOne(cr => cr.Class)
				.HasForeignKey(cr => cr.ClassId)
				.OnDelete(DeleteBehavior.Cascade);

			// ClassSchedule Configuration
			modelBuilder.Entity<ClassSchedule>()
				.HasOne(cs => cs.Class)
				.WithMany(c => c.Schedules)
				.HasForeignKey(cs => cs.ClassId)
				.OnDelete(DeleteBehavior.Cascade);

			// ClassRegistration Configuration
			modelBuilder.Entity<ClassRegistration>()
				.HasKey(cr => cr.Id);

			modelBuilder.Entity<ClassRegistration>()
				.HasOne(cr => cr.Student)
				.WithMany(s => s.ClassRegistrations)
				.HasForeignKey(cr => cr.StudentId)
				.OnDelete(DeleteBehavior.Restrict);

			modelBuilder.Entity<ClassRegistration>()
				.HasOne(cr => cr.Class)
				.WithMany(c => c.StudentRegisterations)
				.HasForeignKey(cr => cr.ClassId)
				.OnDelete(DeleteBehavior.Cascade);

			modelBuilder.Entity<ClassRegistration>()
				.HasOne(cr => cr.RegistrationPeriod)
				.WithMany(rp => rp.ClassRegistrations)
				.HasForeignKey(cr => cr.RegistrationPeriodId)
				.OnDelete(DeleteBehavior.Restrict);
		}
	}
}