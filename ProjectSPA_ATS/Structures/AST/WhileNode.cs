using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectSPA_ATS.Structures.AST
{
    public class WhileNode : StatementNode
    {
        public string ConditionVar { get; }
        public List<StatementNode> Body { get; }
        public WhileNode(string condVar, List<StatementNode> body)
        {
            ConditionVar = condVar;
            Body = body;
        }
    }
}
