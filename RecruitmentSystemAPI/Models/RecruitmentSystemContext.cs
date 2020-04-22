using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using RecruitmentSystemAPI.ViewModels;

namespace RecruitmentSystemAPI.Models
{
    public partial class RecruitmentSystemContext : IdentityDbContext<SystemUser>
    {
        public RecruitmentSystemContext()
        {
        }

        public RecruitmentSystemContext(DbContextOptions<RecruitmentSystemContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Company> Companies { get; set; }
        public virtual DbSet<CompanyUser> CompanyUsers { get; set; }
        public virtual DbSet<JobSkill> JobSkills { get; set; }
        public virtual DbSet<Job> Jobs { get; set; }
        public virtual DbSet<LabourerSkillJob> LabourerSkillJobs { get; set; }
        public virtual DbSet<LabourerSkill> LabourerSkills { get; set; }
        public virtual DbSet<Labourer> Labourers { get; set; }
        public virtual DbSet<Skill> Skills { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("ProductVersion", "2.2.6-servicing-10079");

            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Company>(entity =>
            {
                entity.Property(e => e.Address)
                    .IsRequired()
                    .HasMaxLength(450)
                    .IsUnicode(false);

                entity.Property(e => e.City)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Country)
                    .IsRequired()
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(450)
                    .IsUnicode(false);

                entity.Property(e => e.Phone)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Province)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(450)
                    .IsUnicode(false);

                entity.Property(e => e.Rating)
                    .IsRequired()
                    .HasColumnType("float");
            });

            modelBuilder.Entity<CompanyUser>(entity =>
            {
                entity.Property(e => e.UserId)
                    .IsRequired()
                    .HasMaxLength(450);

                entity.HasOne(d => d.Company)
                    .WithMany(p => p.CompanyUsers)
                    .HasForeignKey(d => d.CompanyId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CompanyUsers_Companies");
            });

            modelBuilder.Entity<JobSkill>(entity =>
            {
                entity.HasOne(d => d.Job)
                    .WithMany(p => p.JobSkills)
                    .HasForeignKey(d => d.JobId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_JobSkills_Jobs");

                entity.HasOne(d => d.Skill)
                    .WithMany(p => p.JobSkills)
                    .HasForeignKey(d => d.SkillId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_JobSkills_Skills");
            });

            modelBuilder.Entity<Job>(entity =>
            {
                entity.Property(e => e.Address)
                    .IsRequired()
                    .HasMaxLength(450)
                    .IsUnicode(false);

                entity.Property(e => e.City)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Country)
                    .IsRequired()
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.CreateDate).HasColumnType("datetime");

                entity.Property(e => e.Description)
                    .HasMaxLength(450)
                    .IsUnicode(false);

                entity.Property(e => e.EndDate).HasColumnType("datetime");

                entity.Property(e => e.Province)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.StartDate).HasColumnType("datetime");

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.UpdateDate).HasColumnType("datetime");

                entity.HasOne(d => d.Company)
                    .WithMany(p => p.Jobs)
                    .HasForeignKey(d => d.CompanyId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Jobs_Companies");
            });

            modelBuilder.Entity<LabourerSkillJob>(entity =>
            {
                entity.Property(e => e.Date).HasColumnType("datetime");

                entity.Property(e => e.WageAmount).HasColumnType("decimal(18, 0)");

                entity.HasOne(d => d.Job)
                    .WithMany(p => p.LabourerSkillJobs)
                    .HasForeignKey(d => d.JobId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_LabourerSkillJob_Jobs");

                entity.HasOne(d => d.LabourerSkill)
                    .WithMany(p => p.LabourerSkillJobs)
                    .HasForeignKey(d => d.LabourerSkillId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_LabourerSkillJob_LabourerSkills");
            });

            modelBuilder.Entity<LabourerSkill>(entity =>
            {
                entity.HasOne(d => d.Labourer)
                    .WithMany(p => p.LabourerSkills)
                    .HasForeignKey(d => d.LabourerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_LabourerSkills_Labourers");

                entity.HasOne(d => d.Skill)
                    .WithMany(p => p.LabourerSkills)
                    .HasForeignKey(d => d.SkillId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_LabourerSkills_Skills");
            });

            modelBuilder.Entity<Labourer>(entity =>
            {
                entity.Property(e => e.Address)
                    .IsRequired()
                    .HasMaxLength(450)
                    .IsUnicode(false);

                entity.Property(e => e.QualityRating)
                   .IsRequired()
                   .HasMaxLength(450)
                   .IsUnicode(false);

                entity.Property(e => e.SafetyRating)
                   .IsRequired()
                   .HasMaxLength(450)
                   .IsUnicode(false);

                entity.Property(e => e.City)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Country)
                    .IsRequired()
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.FirstName)
                    .IsRequired()
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.LastName)
                    .IsRequired()
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.PersonalId)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Phone).HasMaxLength(10);

                entity.Property(e => e.Province)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.UserId)
                    .IsRequired()
                    .HasMaxLength(450);
            });

            modelBuilder.Entity<Skill>(entity =>
            {
                entity.Property(e => e.ChargeAmount).HasColumnType("decimal(18, 0)");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.PayAmount).HasColumnType("decimal(18, 0)");
            });

            SeedData(modelBuilder);
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<IdentityRole>().HasData(new IdentityRole
            {
                Id = "1",
                Name = "Admin",
                ConcurrencyStamp = "1",
                NormalizedName = "Admin".ToUpper()
            }, new IdentityRole
            {
                Id = "2",
                Name = "Company",
                ConcurrencyStamp = "2",
                NormalizedName = "Company".ToUpper()
            }, new IdentityRole
            {
                Id = "3",
                Name = "Labourer",
                ConcurrencyStamp = "3",
                NormalizedName = "Labourer".ToUpper()
            });
        }
    }
}
