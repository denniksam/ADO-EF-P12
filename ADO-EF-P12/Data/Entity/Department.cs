using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADO_EF_P12.Data.Entity
{
    public class Department
    {
        public Guid   Id   { get; set; }
        public String Name { get; set; } = null!;
    }
}
