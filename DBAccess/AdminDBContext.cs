using DBAccess.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBAccess
{
    public class AdminDBContext : DbContext
    {
        public AdminDBContext()
            : base("name=AdminDBContext")
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<AdminDBContext, DBAccess.Migrations.Configuration>());
            Configuration.ProxyCreationEnabled = false;
            this.Configuration.LazyLoadingEnabled = false;
        }

        public virtual DbSet<ForgotPasswordToken> ForgotPasswordTokens { get; set; }
        public virtual DbSet<Notification> Notifications { get; set; }
        public virtual DbSet<Admin> Admins { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Doctor> Doctors { get; set; }
        public virtual DbSet<AppRatings> AppRatings { get; set; }
        public virtual DbSet<UserAddress> UserAddresses { get; set; }
        public virtual DbSet<ContactUs> ContactUs { get; set; }
        public virtual DbSet<UserDevice> UserDevices { get; set; }
        public virtual DbSet<RefreshTokens> RefreshTokens { get; set; }
        public virtual DbSet<Settings> Settings { get; set; }
        public virtual DbSet<AdminNotifications> AdminNotifications { get; set; }
        public virtual DbSet<DoctorDocument> DoctorDocuments { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {



            modelBuilder.Entity<AdminNotifications>()
               .HasMany(e => e.Notifications)
               .WithOptional(e => e.AdminNotification)
               .HasForeignKey(e => e.AdminNotification_Id)
               .WillCascadeOnDelete(false);


            modelBuilder.Entity<User>()
                .HasMany(e => e.UserDevices)
                .WithRequired(e => e.User)
                .HasForeignKey(e => e.User_Id)
                .WillCascadeOnDelete(false);

            //modelBuilder.Entity<User>()
            //    .HasMany(e => e.UserAddresses)
            //    .WithRequired(e => e.User)
            //    .HasForeignKey(e => e.User_ID)
            //    .WillCascadeOnDelete(false);

            //modelBuilder.Entity<User>()
            //  .HasMany(e => e.AppSettings)
            //  .WithRequired(e => e.User)
            //  .HasForeignKey(e => e.User_ID)
            //  .WillCascadeOnDelete(false);

            modelBuilder.Entity<User>()
                .HasMany(e => e.Notifications)
                .WithOptional(e => e.User)
                .HasForeignKey(e => e.User_ID)
                .WillCascadeOnDelete(false);

            //modelBuilder.Entity<User>()
            //    .Property(e => e.FirstName)
            //    .IsUnicode(false);

            //modelBuilder.Entity<User>()
            //    .Property(e => e.LastName)
            //    .IsUnicode(false);

            //modelBuilder.Entity<User>()
            //    .HasMany(e => e.Favourites)
            //    .WithRequired(e => e.User)
            //    .HasForeignKey(e => e.User_ID)
            //    .WillCascadeOnDelete(false);

            modelBuilder.Entity<User>()
                .HasMany(e => e.ForgotPasswordTokens)
                .WithOptional(e => e.User)
                .HasForeignKey(e => e.User_Id)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Doctor>()
                .HasMany(e => e.ForgotPasswordTokens)
                .WithOptional(e => e.Doctor)
                .HasForeignKey(e => e.Doctor_Id)
                .WillCascadeOnDelete(false);

            //modelBuilder.Entity<User>()
            // .HasMany(e => e.PaymentCards)
            // .WithRequired(e => e.User)
            // .HasForeignKey(e => e.User_ID)
            // .WillCascadeOnDelete(false);

            modelBuilder.Entity<Doctor>()
               .HasMany(e => e.DoctorDocuments)
               .WithRequired(e => e.Doctor)
               .HasForeignKey(e => e.Doctor_Id)
               .WillCascadeOnDelete(false);

        }
    }
}
