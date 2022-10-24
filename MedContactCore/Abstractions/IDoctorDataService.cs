using MedContactCore.DataTransferObjects;
using MedContactDb.Entities;
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
      Task<int> CreateDoctorDataAsync(DoctorDataDto dto);
      Task<bool> IsDoctorDataExists(Guid? userId, Guid? specId);
      Task<List<DoctorDataDto>> GetDoctorDataByUserId(Guid userId);
      Task<int> DeleteDoctorDataAsync(DoctorDataDto dto);
    }
}
