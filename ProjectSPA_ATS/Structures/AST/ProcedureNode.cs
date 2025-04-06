using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectSPA_ATS.Structures.AST
{
    public class ProcedureNode : AstNode
    {
        public string Name { get; }
        public List<StatementNode> Statements { get; }
        public ProcedureNode(string name, List<StatementNode> statements)
        {
            Name = name;
            Statements = statements;
        }
    }
}
