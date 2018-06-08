using DBAccess.Models;
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
                Count++;
            if (User.MedicalConditions.Count != 0)
                Count++;
            if (User.Allergies.Count != 0)
                Count++;
            if (User.Medications.Count != 0)
                Count++;

            User.CompletionPercentage = Convert.ToDouble(Math.Round(((Count) / 6) * 100,2));
            
        }
    }
}
