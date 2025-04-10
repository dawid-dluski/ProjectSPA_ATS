using ProjectSPA_ATS.Exceptions;
using ProjectSPA_ATS.Structures;
using ProjectSPA_ATS.Structures.AST;

namespace ProjectSPA_ATS.PKB
{
    sealed public class PKBService : IPBKService
    {
        private static readonly PKBService _instance = new PKBService();
        private PKBService() 
        {
            ModifyList = new List<Modify>();
            UseList = new List<Use>();
            FollowList = new List<Follow>();
            ParentList = new List<Parent>();
        }

        public static PKBService Instance => _instance;
        
        private List<ProcedureNode> ProcedureList = new List<ProcedureNode>();
        private List<AssignNode> VariableList;
        private List<Modify> ModifyList;
        private List<Use> UseList;
        private List<Follow> FollowList;
        private List<Parent> ParentList;

        // Procedure API
        public void AddProcedure(ProcedureNode proc) {
            if (ProcedureList.Any(x => x.Name == proc.Name))
            {
                throw new ProcedureNameConflictException();
            }

            ProcedureList.Add(proc);
        }
        public List<ProcedureNode> GetProcedureList()
        {
            return ProcedureList;
        }
        public ProcedureNode GetProcedureByName(string n) 
        {
            return ProcedureList.FirstOrDefault(x => x.Name == n);
        }
        
        // Variable API
        public void AddVariable(AssignNode v)
        {
            if (VariableList.Any(x => x.VarName == v.VarName))
            {
                throw new VariableNameConflictException();
            }
            VariableList.Add(v);
        }
        public List<AssignNode> GetVariableList()
        {
            return VariableList;
        }
        public AssignNode GetVariableByName(string n)
        {
            return VariableList.FirstOrDefault(x => x.VarName == n);
        }
        public AssignNode GetVariableByIndex(int i)
        {
            if (i < 0 || i >= VariableList.Count)
            {
                throw new IndexOutOfRangeException("Index out of range");
            }
            return VariableList[i];
        }
        public int getVariableListSize()
        {
            return VariableList.Count;
        }

        // Modify API
        public void AddModify(Modify m)
        {
            /*if (ModifyList.Any(x => x.ModifyVar == m.ModifyVar))
            {
                throw new ModifyNameConflictException();
            }*/
            ModifyList.Add(m);
        }
        public List<Modify> GetModifyList()
        {
            return ModifyList;
        }
        public Modify GetModifyByIndex(int i)
        {
            if (i < 0 || i >= ModifyList.Count)
            {
                throw new IndexOutOfRangeException("Index out of range");
            }
            return ModifyList[i];
        }
        public int GetModifyListSize()
        {
            return ModifyList.Count;
        }

        // Follow API
        public void AddFollow(Follow f)
        {
            FollowList.Add(f);
        }
        public List<Follow> GetFollowList()
        {
            return FollowList;
        }
        public Follow GetFollowByIndex(int i)
        {
            if (i < 0 || i >= FollowList.Count)
            {
                throw new IndexOutOfRangeException("Index out of range");
            }
            return FollowList[i];
        }
        public int GetFollowListSize()
        {
            return FollowList.Count;
        }

        // Use API
        public void AddUses(Use u)
        {
            UseList.Add(u);
        }
        public List<Use> GetUseList()
        {
            return UseList;
        }
        public Use GetUseByIndex(int i)
        {
            if (i < 0 || i >= UseList.Count)
            {
                throw new IndexOutOfRangeException("Index out of range");
            }
            return UseList[i];
        }
        public int GetUseListSize()
        {
            return UseList.Count;
        }

        // Parent API
        public void AddParent(Parent p)
        {
            ParentList.Add(p);
        }
        public List<Parent> GetParentList()
        {
            return ParentList;
        }
        public Parent GetParentByIndex(int i)
        {
            if (i < 0 || i >= ParentList.Count)
            {
                throw new IndexOutOfRangeException("Index out of range");
            }
            return ParentList[i];
        }
        public int GetParentListSize()
        {
            return ParentList.Count;
        }

        public List<Follow> GetFollowAll() => FollowList;
        public List<Parent> GetParentAll() => ParentList;
        public List<Use> GetUsesAll() => UseList;
        public List<Modify> GetModifyAll() => ModifyList;
    }
}