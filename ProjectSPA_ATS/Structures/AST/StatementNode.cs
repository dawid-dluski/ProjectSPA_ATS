using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectSPA_ATS.Structures.AST
{
    public abstract class StatementNode : AstNode
    {
        public int StatementId { get; set; }
    }
}
