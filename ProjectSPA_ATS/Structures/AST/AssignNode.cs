using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectSPA_ATS.Structures.AST
{
    public class AssignNode : StatementNode
    {
        public string VarName { get; }
        public ExpressionNode Expression { get; }
        public AssignNode(string varName, ExpressionNode expr)
        {
            VarName = varName;
            Expression = expr;
        }
    }
}
