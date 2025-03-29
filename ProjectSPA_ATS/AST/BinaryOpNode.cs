using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectSPA_ATS.AST
{
    public class BinaryOpNode : ExpressionNode
    {
        public string Operator { get; }        // np. "+" 
        public ExpressionNode Left { get; }
        public ExpressionNode Right { get; }
        public BinaryOpNode(ExpressionNode left, ExpressionNode right, string op)
        {
            Left = left;
            Right = right;
            Operator = op;
        }
    }
}
