using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedContactCore.DataTransferObjects
{
    public class Result
    {
        public string? Name { get; set; }
        public int IntResult { get; set; } = 0;
        public int? IntResult1 { get; set; } = 0;
        public int? IntResult2 { get; set; } = 0;
        public Guid? GuidResult { get; set; }
    }
}
