using MedContactDataAbstractions.Repositories;
using MedContactDb;
using MedContactDb.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedContactDataRepositories
{
    public class BaseUserRepository<B> : Repository<B>, IBaseUserRepository<B> where B : BaseUser
    {
        public BaseUserRepository(MedContactContext database) : base(database)
        {
        }
    }
}
