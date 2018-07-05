using DBAccess.Models;
using DBAccess.ViewModels.Order;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.HelperServices
{
    public static class ExtensionMethods
    {
        public static void CalculateProfilePercentage(this User User)
        {
            double Count = 0;

            if (User.FamilyHistory.Count != 0)
                Count++;
            if (User.Vaccinations.Count != 0)
                Count++;
            if (User.LifeStyle != null)
            {
                if (!string.IsNullOrEmpty(User.LifeStyle.Alcohol) && !string.IsNullOrEmpty(User.LifeStyle.DietryRestrictions) && !string.IsNullOrEmpty(User.LifeStyle.RecreationalDrugs) && !string.IsNullOrEmpty(User.LifeStyle.SexuallyActive) && !string.IsNullOrEmpty(User.LifeStyle.Smoking))
                    Count++;
            }
            if (User.MedicalConditions.Count != 0)
                Count++;
            if (User.Allergies.Count != 0)
                Count++;
            if (User.Medications.Count != 0)
                Count++;
            if (!string.IsNullOrEmpty(User.Height) && !string.IsNullOrEmpty(User.Weight) && !string.IsNullOrEmpty(User.BMI) && !string.IsNullOrEmpty(User.Location_Name))
                Count++;
            
            User.CompletionPercentage = Convert.ToDouble(Math.Round(((Count) / 7) * 100, 2));

        }

        public static void CalculateFamilyMemberProfilePercentage(this FamilyMember FamilyMember)
        {
            double Count = 0;

            if (FamilyMember.Vaccinations.Count != 0)
                Count++;
            if (FamilyMember.MedicalConditions.Count != 0)
                Count++;
            if (FamilyMember.Allergies.Count != 0)
                Count++;
            if (FamilyMember.Medications.Count != 0)
                Count++;
            if (FamilyMember.FamilyMemberLifeStyle != null)
                Count++;

            FamilyMember.CompletionPercentage = Convert.ToDouble(Math.Round(((Count) / 5) * 100, 2));

        }

        public static void CalculateAverageRating(this Doctor Doctor)
        {
            try
            {
                if (Doctor.DoctorRating.Count > 0)
                {
                    Doctor.AverageRating = Doctor.DoctorRating.Average(x => x.Rating);
                }
            }
            catch (Exception ex)
            {

            }

        }

        public static void CalculateAverageRating(this Pharmacy Pharmacy)
        {
            try
            {
                if (Pharmacy.PharmacyRating.Count > 0)
                {
                    Pharmacy.AverageRating = Pharmacy.PharmacyRating.Average(x => x.Rating);
                }
            }
            catch (Exception ex)
            {

            }

        }

        public static double CalculateAverageRating(this List<PharmacyRating> Pharmacy)
        {
            try
            {
                double Rating = 0;
                if (Pharmacy.Count > 0)
                {
                    Rating = Pharmacy.Average(x => x.Rating);
                }

                return Rating;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public static void CalculateOrderSubTotal(this OrderViewModel model)
        {
            foreach (var item in model.Cart.CartItems)
            {
                model.SubTotal = model.SubTotal + (item.Price * item.Qty);
            }
        }

        public static void CalculateOrderTotal(this OrderViewModel model, double DeliveryFee)
        {
            model.Total = model.SubTotal + DeliveryFee;
        }

    }
}
