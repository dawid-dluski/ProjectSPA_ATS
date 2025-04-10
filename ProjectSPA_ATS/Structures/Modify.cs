using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectSPA_ATS.Structures
{
    public class Modify
    {
        public int Statement { get; }
        public string Variable { get; }

        public Modify(int stmt, string variable)
        {
            Statement = stmt;
            Variable = variable;
        }
    }
}
