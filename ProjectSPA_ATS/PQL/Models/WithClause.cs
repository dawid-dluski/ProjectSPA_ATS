using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectSPA_ATS.PQL.Models
{
    public class WithClause
    {
        public string Synonym { get; }
        public string Attribute { get; }
        public string ValueType { get; }
        public string Value { get; }

        public WithClause(string syn, string attr, string type, string value)
        {
            Synonym = syn;
            Attribute = attr;
            ValueType = type;
            Value = value;
        }
    }
}
