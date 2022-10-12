using MedContactDb.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedContactDataAbstractions.Repositories
{
    public interface IBaseUserRepository<B>: IRepository<B> where B : BaseUser
    {
    }
}
