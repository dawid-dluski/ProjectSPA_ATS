using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectSPA_ATS.Structures.AST
{
    public class ConstNode : ExpressionNode
    {
        public int Value { get; }
        public ConstNode(int value)
        {
            Value = value;
        }
    }
}
