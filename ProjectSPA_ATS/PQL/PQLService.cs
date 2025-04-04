using ProjectSPA_ATS.Parser;
using ProjectSPA_ATS.PKB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectSPA_ATS.PQL
{
    sealed public class PQLService
    {
        private static PQLService? _instance;
        private readonly IPBKService _PBKService;

        private PQLService(IPBKService PKBService)
        {
            _PBKService = PKBService;
        }

        public static PQLService GetInstance(IPBKService pbkService)
        {
            return _instance ??= new PQLService(pbkService);
        }

        public void StartListening()
        {

        }
    }
}
