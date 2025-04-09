using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectSPA_ATS.PQL.Models
{
    public class RelationshipClause
    {
        public string Relation { get; }
        public bool IsTransitive { get; }
        public string Arg1 { get; }
        public string Arg2 { get; }

        public RelationshipClause(string relation, bool isTransitive, string arg1, string arg2)
        {
            Relation = relation;
            IsTransitive = isTransitive;
            Arg1 = arg1;
            Arg2 = arg2;
        }
    }
}
