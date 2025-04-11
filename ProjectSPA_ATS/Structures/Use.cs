using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectSPA_ATS.Structures
{
    public class Use
    {
        public int StatementId { get; }
        public string VariableName { get; }

        public Use(int statementId, string variableName)
        {
            StatementId = statementId;
            VariableName = variableName;
        }

        public override string ToString()
        {
            return $"Uses(stmt#{StatementId}, \"{VariableName}\")";
        }
    }
}
