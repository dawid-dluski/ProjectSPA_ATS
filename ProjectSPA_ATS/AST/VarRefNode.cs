﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectSPA_ATS.AST
{
    public class VarRefNode : ExpressionNode
    {
        public string VarName { get; }
        public VarRefNode(string name)
        {
            VarName = name;
        }
    }
}
