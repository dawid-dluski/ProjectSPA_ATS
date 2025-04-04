using ProjectSPA_ATS.Exceptions;
using ProjectSPA_ATS.Structures;
using ProjectSPA_ATS.Structures.AST;

namespace ProjectSPA_ATS.PKB
{
    sealed public class PKBService : IPBKService
    {
        private static readonly PKBService _instance = new PKBService();
        private PKBService() { }

        public static PKBService Instance => _instance;
        
        private List<ProcedureNode> ProcedureList = new List<ProcedureNode>();
        private List<Modifie> ModifieList;
        private List<Use> UseList;
        private List<Follow> FollowList;
        private List<Parent> ParentList;
        private List<Variable> VariableList;

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
    }
}