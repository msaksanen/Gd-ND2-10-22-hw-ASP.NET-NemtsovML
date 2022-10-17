using MedContactCore.DataTransferObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedContactCore.Abstractions
{
    public interface IDoctorDataService
    {
      Task<DoctorInfo> GetDoctorInfoById(Guid? doctorDataId);
    }
}
