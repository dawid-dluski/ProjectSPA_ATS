using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectSPA_ATS.Structures.AST
{
    public class IfNode: StatementNode
    {
        public string ConditionVar { get; }
        public List<StatementNode> ThenBody { get; }
        public List<StatementNode> ElseBody { get; }

        public IfNode(string cond, List<StatementNode> thenBody, List<StatementNode> elseBody)
        {
            ConditionVar = cond;
            ThenBody = thenBody;
            ElseBody = elseBody;
        }
    }
}
