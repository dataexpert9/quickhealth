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
        public virtual DbSet<AppRating> AppRating { get; set; }
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
        public virtual DbSet<DoctorPrescription> DoctorPrescription { get; set; }
        public virtual DbSet<DoctorRating> DoctorRating { get; set; }
        public virtual DbSet<Countries> Countries { get; set; }
        public virtual DbSet<States> States { get; set; }
        public virtual DbSet<Cities> Cities { get; set; }
        
        // Pharmacy 
        public virtual DbSet<Pharmacy> Pharmacy { get; set; }
        public virtual DbSet<PharmacyRequestImages> PharmacyAppointmentImages { get; set; }
        //public virtual DbSet<Medicine> Medicine { get; set; }
        public virtual DbSet<PharmacyRequest> PharmacyRequest { get; set; }
        public virtual DbSet<PharmacyPrescription> PharmacyPrescription { get; set; }
        public virtual DbSet<DoctorPrescriptionImages> PrescriptionImages { get; set; }
        public virtual DbSet<PharmacyRating> PharmacyRating { get; set; }

        // order related tables
        public virtual DbSet<Order> Order { get; set; }
        public virtual DbSet<OrderPayment> OrderPayment { get; set; }
        public virtual DbSet<PharmacyOrder> PharmacyOrder { get; set; }
        public virtual DbSet<Order_Items> Order_Items { get; set; }


        // Transactional Data Saving Tables
        public virtual DbSet<AllergyHistory> AllergyHistory { get; set; }
        public virtual DbSet<VaccinationHistory> VaccinationHistory { get; set; }
        public virtual DbSet<MedicationHistory> MedicationHistory { get; set; }
        public virtual DbSet<MedicalConditionHistory> MedicalConditionHistory { get; set; }
        public virtual DbSet<LifeStyleHistory> LifeStyleHistory { get; set; }
        public virtual DbSet<AdminSubAdminNotifications> AdminSubAdminNotifications { get; set; }


        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {


            modelBuilder.Entity<Admin>()
                .HasMany(e => e.AdminSubAdminNotifications)
                .WithRequired(e => e.Admin)
                .HasForeignKey(e => e.AdminId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Appointment>()
                .HasMany(e => e.MedicalConditionHistory)
                .WithRequired(e => e.Appointment)
                .HasForeignKey(e => e.Appointment_Id)
                .WillCascadeOnDelete(false);


            modelBuilder.Entity<Appointment>()
                .HasMany(e => e.MedicationHistory)
                .WithRequired(e => e.Appointment)
                .HasForeignKey(e => e.Appointment_Id)
                .WillCascadeOnDelete(false);


            modelBuilder.Entity<Appointment>()
                .HasMany(e => e.VaccinationHistory)
                .WithRequired(e => e.Appointment)
                .HasForeignKey(e => e.Appointment_Id)
                .WillCascadeOnDelete(false);


            modelBuilder.Entity<Appointment>()
                .HasMany(e => e.AllergyHistory)
                .WithRequired(e => e.Appointment)
                .HasForeignKey(e => e.Appointment_Id)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Pharmacy>()
                .HasMany(e => e.Order)
                .WithOptional(e => e.Pharmacy)
                .HasForeignKey(e => e.Pharmacy_Id)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<User>()
                .HasMany(e => e.Order)
                .WithRequired(e => e.User)
                .HasForeignKey(e => e.User_Id)
                .WillCascadeOnDelete(false);


            modelBuilder.Entity<PharmacyOrder>()
                .HasMany(e => e.Order_Items)
                .WithRequired(e => e.PharmacyOrder)
                .HasForeignKey(e => e.PharmacyOrder_Id)
                .WillCascadeOnDelete(false);



            modelBuilder.Entity<Pharmacy>()
                .HasMany(e => e.PharmacyOrder)
                .WithRequired(e => e.Pharmacy)
                .HasForeignKey(e => e.Pharmacy_Id)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Order>()
                .HasMany(e => e.PharmacyOrder)
                .WithRequired(e => e.Order)
                .HasForeignKey(e => e.Order_Id)
                .WillCascadeOnDelete(false);


            modelBuilder.Entity<OrderPayment>()
                .HasRequired(e => e.Order)
                .WithRequiredDependent(e => e.OrderPayment)
                .WillCascadeOnDelete(false);


            modelBuilder.Entity<FamilyMember>()
                .HasMany(e => e.Medications)
                .WithOptional(e => e.FamilyMember)
                .HasForeignKey(e => e.FamilyMember_Id)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<FamilyMember>()
                .HasMany(e => e.Allergies)
                .WithOptional(e => e.FamilyMember)
                .HasForeignKey(e => e.FamilyMember_Id)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<FamilyMember>()
                .HasMany(e => e.MedicalConditions)
                .WithOptional(e => e.FamilyMember)
                .HasForeignKey(e => e.FamilyMember_Id)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<FamilyMember>()
                .HasMany(e => e.Vaccinations)
                .WithOptional(e => e.FamilyMember)
                .HasForeignKey(e => e.FamilyMember_Id)
                .WillCascadeOnDelete(false);

            //modelBuilder.Entity<FamilyMember>()
            //   .HasOptional(x => x.LifeStyle)
            //   .WithOptionalPrincipal(x => x.FamilyMember)
            //   .Map(a => a.MapKey("FamilyMember_Id"));

            //modelBuilder.Entity<User>()
            //   .HasOptional(x => x.LifeStyle)
            //   .WithOptionalPrincipal(x => x.User)
            //   .Map(a => a.MapKey("User_Id"));


            //modelBuilder.Entity<FamilyMember>()
            //    .HasOptional(e => e.LifeStyle)
            //    .WithRequired(e => e.FamilyMember);

            modelBuilder.Entity<Countries>()
                .HasMany(e => e.Cities)
                .WithOptional(e => e.Country)
                .HasForeignKey(e => e.Country_Id)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<States>()
                .HasMany(e => e.Cities)
                .WithOptional(e => e.State)
                .HasForeignKey(e => e.State_Id)
                .WillCascadeOnDelete(false);


            modelBuilder.Entity<Countries>()
                .HasMany(e => e.States)
                .WithOptional(e => e.Country)
                .HasForeignKey(e => e.Country_Id)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<User>()
                .HasMany(e => e.PharmacyRequest)
                .WithRequired(e => e.User)
                .HasForeignKey(e => e.User_Id)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Pharmacy>()
               .HasMany(e => e.PharmacyRating)
               .WithRequired(e => e.Pharmacy)
               .HasForeignKey(e => e.Pharmacy_Id)
               .WillCascadeOnDelete(false);

            modelBuilder.Entity<User>()
               .HasMany(e => e.PharmacyRating)
               .WithRequired(e => e.User)
               .HasForeignKey(e => e.User_Id)
               .WillCascadeOnDelete(false);


            modelBuilder.Entity<User>()
                .HasMany(e => e.DoctorRating)
                .WithRequired(e => e.User)
                .HasForeignKey(e => e.User_Id)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Doctor>()
               .HasMany(e => e.DoctorRating)
               .WithRequired(e => e.Doctor)
               .HasForeignKey(e => e.Doctor_Id)
               .WillCascadeOnDelete(false);

            modelBuilder.Entity<DoctorPrescription>()
               .HasMany(e => e.DoctorPrescriptionImages)
               .WithRequired(e => e.DoctorPrescription)
               .HasForeignKey(e => e.DoctorPrescription_Id)
               .WillCascadeOnDelete(false);

            modelBuilder.Entity<PharmacyRequest>()
               .HasMany(e => e.PharmacyRequestImages)
               .WithOptional(e => e.PharmacyRequest)
               .HasForeignKey(e => e.PharmacyRequest_Id)
               .WillCascadeOnDelete(false);

            modelBuilder.Entity<FamilyMember>()
                .HasMany(e => e.PharmacyRequest)
                .WithOptional(e => e.FamilyMember)
                .HasForeignKey(e => e.FamilyMember_Id)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<User>()
                .HasMany(e => e.PharmacyRequest)
                .WithRequired(e => e.User)
                .HasForeignKey(e => e.User_Id)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<PharmacyRequest>()
                .HasMany(e => e.PharmacyPrescription)
                .WithRequired(e => e.PharmacyRequest)
                .HasForeignKey(e => e.PharmacyRequest_Id)
                .WillCascadeOnDelete(false);


            modelBuilder.Entity<Pharmacy>()
                .HasMany(e => e.PharmacyRequest)
                .WithRequired(e => e.Pharmacy)
                .HasForeignKey(e => e.Pharmacy_Id)
                .WillCascadeOnDelete(false);


            //modelBuilder.Entity<Pharmacy>()
            //    .HasMany(e => e.Medicine)
            //    .WithRequired(e => e.Pharmacy)
            //    .HasForeignKey(e => e.Pharmacy_Id)
            //    .WillCascadeOnDelete(false);

            modelBuilder.Entity<Appointment>()
                .HasMany(e => e.AppointmentImages)
                .WithRequired(e => e.Appointment)
                .HasForeignKey(e => e.Appointment_Id)
                .WillCascadeOnDelete(false);


            modelBuilder.Entity<Appointment>()
                 .HasMany(e => e.DoctorPrescription)
                 .WithOptional(e => e.Appointment)
                 .HasForeignKey(e => e.Appointment_Id)
                 .WillCascadeOnDelete(false);


            modelBuilder.Entity<User>()
                .HasMany(e => e.PharmacyRequest)
                .WithRequired(e => e.User)
                .HasForeignKey(e => e.User_Id)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<User>()
                .HasMany(e => e.AppRating)
                .WithOptional(e => e.User)
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
                .WithOptional(e => e.User)
                .HasForeignKey(e => e.User_Id)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<FamilyMember>()
                .HasMany(e => e.Chat)
                .WithOptional(e => e.FamilyMember)
                .HasForeignKey(e => e.FamilyMember_Id)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Pharmacy>()
             .HasMany(e => e.Chat)
             .WithOptional(e => e.Pharmacy)
             .HasForeignKey(e => e.Pharmacy_Id)
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
                .WithOptional(e => e.User)
                .HasForeignKey(e => e.User_Id)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<User>()
                .HasMany(e => e.Vaccinations)
                .WithOptional(e => e.User)
                .HasForeignKey(e => e.User_Id)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<User>()
                .HasMany(e => e.Allergies)
                .WithOptional(e => e.User)
                .HasForeignKey(e => e.User_Id)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<User>()
               .HasMany(e => e.MedicalConditions)
               .WithOptional(e => e.User)
               .HasForeignKey(e => e.User_Id)
               .WillCascadeOnDelete(false);

            modelBuilder.Entity<User>()
               .HasMany(e => e.FamilyMembers)
               .WithRequired(e => e.User)
               .HasForeignKey(e => e.User_Id)
               .WillCascadeOnDelete(false);

            //modelBuilder.Entity<AdminNotifications>()
            //   .HasMany(e => e.Notifications)
            //   .WithOptional(e => e.AdminNotification)
            //   .HasForeignKey(e => e.AdminNotification_Id)
            //   .WillCascadeOnDelete(false);


            modelBuilder.Entity<User>()
                .HasMany(e => e.UserDevices)
                .WithOptional(e => e.User)
                .HasForeignKey(e => e.User_Id)
                .WillCascadeOnDelete(false);


            modelBuilder.Entity<Doctor>()
                .HasMany(e => e.UserDevices)
                .WithOptional(e => e.Doctor)
                .HasForeignKey(e => e.Doctor_Id)
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
                .HasForeignKey(e => e.User_Id)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Doctor>()
                .HasMany(e => e.Notifications)
                .WithOptional(e => e.Doctor)
                .HasForeignKey(e => e.Doctor_Id)
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
