using MedContactDb.Entities;

namespace MedContactApp.FilterSortPageHelpers
{
    public class CustomFamilyComparer : IComparer<User>
    {
       
            public int Compare(User? x, User? y)
            {
            int xScore = 0; 
            int yScore = 0;
            if (x!= null || x?.FamilyId == null)
                xScore = 0;
            if (y!=null || y?.FamilyId == null)
                yScore = 0;
            if (x?.Family!= null && x?.FamilyId != null)
            {
                xScore = 1;
                if(x.Id.Equals(x?.Family?.MainUserId))
                {
                    xScore = 2;
                }
            }

            if (y?.Family != null && y?.FamilyId != null)
            {
                yScore  = 1;
                if (y.Id.Equals(y?.Family?.MainUserId))
                {
                    yScore = 2;
                }
            }


            return xScore - yScore;
        }
       
    }
}
