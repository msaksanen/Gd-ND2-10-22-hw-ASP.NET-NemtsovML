using MedContactBusiness.ServicesImplementations;
using MedContactCore.Abstractions;
using System.ComponentModel.DataAnnotations;

namespace MedContactApp.Helpers
{
    public class BirthDateChecker
    {
        private readonly IFamilyService _familyService;

        public BirthDateChecker(IFamilyService familyService)
        {
            _familyService = familyService;
        }
        //public  bool Check (DateTime? birthDate)
        //{

        //    if (birthDate <= DateTime.Now && (DateTime.Now.Year - birthDate.Year) <= 120)
        //        return true;

        //    return false;
        //}

        public  int Check(DateTime? birthDate, HttpContext context)
        {
           
           if (birthDate == null)
                return 0;

            if (context.Session.Keys.Contains("isDependent"))
            {
                int? isDependent=context.Session.GetInt32("isDependent");
                if (isDependent == 1) return 1;

            }
              
           var diff = (DateTime.Now).Subtract((DateTime)birthDate);
           var age = diff.TotalDays / 365;
           if (age <= 17)
               return 2;

           if (birthDate <= DateTime.Now && age <= 120)
              return 1;
          
            return 0;
        }
    }
}
