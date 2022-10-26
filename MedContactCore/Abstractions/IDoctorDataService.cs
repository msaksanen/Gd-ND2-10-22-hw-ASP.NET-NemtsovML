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
      Task<DoctorData?> GetsDoctorDataByUserIdSpecId(Guid? userId, Guid? specId);
      Task<List<DoctorDataDto>> GetDoctorDataListByUserId(Guid userId);
      Task<int> MarkForDeleteDoctorDataAsync(DoctorDataDto dto);
    }
}
