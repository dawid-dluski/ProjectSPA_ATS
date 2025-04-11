using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectSPA_ATS.Structures
{
    public class Parent
    {
        public int ParentStmtId { get; }
        public int ChildStmtId { get; }

        public Parent(int parentStmtId, int childStmtId)
        {
            ParentStmtId = parentStmtId;
            ChildStmtId = childStmtId;
        }

        public override string ToString()
        {
            return $"Parent({ParentStmtId}, {ChildStmtId})";
        }
    }
}
