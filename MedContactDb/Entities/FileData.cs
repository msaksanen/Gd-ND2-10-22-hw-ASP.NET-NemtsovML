using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedContactDb.Entities
{
    public  class FileData : IBaseEntity
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? Category { get; set; }
        public string? Path { get; set; }
        public Guid? UserId { get; set; }
        public User? User { get; set; }
        public Guid? MedDataId { get; set; }
        public MedData? MedData { get; set; }
    }
}
