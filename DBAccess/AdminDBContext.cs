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
        //public virtual DbSet<UserAddress> UserAddresses { get; set; }
        public virtual DbSet<ContactUs> ContactUs { get; set; }
        public virtual DbSet<UserDevice> UserDevices { get; set; }
        public virtual DbSet<RefreshTokens> RefreshTokens { get; set; }
        public virtual DbSet<Settings> Settings { get; set; }
        public virtual DbSet<AdminNotifications> AdminNotifications { get; set; }
        public virtual DbSet<DoctorDocument> DoctorDocuments { get; set; }
        public virtual DbSet<FamilyMember> FamilyMember { get; set; }
        public virtual DbSet<FamilyHistory> FamilyHistory { get; set; }
        public virtual DbSet<MedicalConditions> MedicalConditions { get; set; }
        public virtual DbSet<LifeStyle> LifeStyle { get; set; }
        public virtual DbSet<Allergies> Allergies { get; set; }
        public virtual DbSet<Vaccinations> Vaccinations { get; set; }
        public virtual DbSet<Medications> Medications { get; set; }
        public virtual DbSet<Appointment> Appointment { get; set; }
        public virtual DbSet<AppointmentImages> AppointmentImages { get; set; }
        public virtual DbSet<Chat> Chat { get; set; }
        public virtual DbSet<AppRating> AppRating { get; set; }
        public virtual DbSet<Pharmacy> Pharmacy { get; set; }
        public virtual DbSet<PharmacyAppointment> PharmacyAppointment { get; set; }
        public virtual DbSet<PharmacyAppointmentImages> PharmacyAppointmentImages { get; set; }





        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {

            //modelBuilder.Entity<PharmacyAppointment>()
            //.HasMany(e => e.PharmacyAppointmentImages)
            //.WithOptional(e => e.PharmacyAppointment)
            //.HasForeignKey(e => e.PharmacyAppointment_Id)
            //.WillCascadeOnDelete(false);

            //modelBuilder.Entity<Pharmacy>()
            //    .HasMany(e => e.PharmacyAppointment)
            //    .WithOptional(e => e.Pharmacy)
            //    .HasForeignKey(e => e.Pharmacy_Id)
            //    .WillCascadeOnDelete(false);

            modelBuilder.Entity<User>()
                .HasMany(e => e.PharmacyAppointment)
                .WithRequired(e => e.User)
                .HasForeignKey(e => e.User_Id)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<User>()
                .HasMany(e => e.AppRating)
                .WithRequired(e => e.User)
                .HasForeignKey(e => e.User_Id)
                .WillCascadeOnDelete(false);



            modelBuilder.Entity<Doctor>()
                .HasMany(e => e.AppRating)
                .WithOptional(e => e.Doctor)
                .HasForeignKey(e => e.Doctor_Id)
                .WillCascadeOnDelete(false);


            modelBuilder.Entity<Doctor>()
                .HasMany(e => e.Chat)
                .WithOptional(e => e.Doctor)
                .HasForeignKey(e => e.Doctor_Id)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<User>()
                .HasMany(e => e.Chat)
                .WithRequired(e => e.User)
                .HasForeignKey(e => e.User_Id)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Doctor>()
                .HasMany(e => e.Appointments)
                .WithOptional(e => e.Doctor)
                .HasForeignKey(e => e.Doctor_Id)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<User>()
                .HasMany(e => e.Appointment)
                .WithRequired(e => e.User)
                .HasForeignKey(e => e.User_Id)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<FamilyMember>()
                .HasMany(e => e.Appointment)
                .WithOptional(e => e.FamilyMember)
                .HasForeignKey(e => e.FamilyMember_Id)
                .WillCascadeOnDelete(false);
            

            modelBuilder.Entity<User>()
                .HasMany(e => e.Medications)
                .WithRequired(e => e.User)
                .HasForeignKey(e => e.User_Id)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<User>()
                .HasMany(e => e.Vaccinations)
                .WithRequired(e => e.User)
                .HasForeignKey(e => e.User_Id)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<User>()
                .HasMany(e => e.Allergies)
                .WithRequired(e => e.User)
                .HasForeignKey(e => e.User_Id)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<User>()
               .HasMany(e => e.MedicalConditions)
               .WithRequired(e => e.User)
               .HasForeignKey(e => e.User_Id)
               .WillCascadeOnDelete(false);

            modelBuilder.Entity<User>()
               .HasMany(e => e.FamilyMembers)
               .WithRequired(e => e.User)
               .HasForeignKey(e => e.User_Id)
               .WillCascadeOnDelete(false);
            
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
