using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectSPA_ATS.Structures.AST
{
    public class CallNode: StatementNode
    {
        public string Callee { get; }

        public CallNode(string callee) => Callee = callee;
    }
}
