using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectSPA_ATS.Structures
{
    public class Follow
    {
        public int PrecedingStmtId { get; }
        public int FollowingStmtId { get; }

        public Follow(int precedingStmtId, int followingStmtId)
        {
            PrecedingStmtId = precedingStmtId;
            FollowingStmtId = followingStmtId;
        }

        public override string ToString()
        {
            return $"Follows({PrecedingStmtId}, {FollowingStmtId})";
        }
    }
}
