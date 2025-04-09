using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectSPA_ATS.PQL.Models
{
    public class Declaration
    {
        public string DesignEntity { get; }
        public List<string> Synonyms { get; }

        public Declaration(string designEntity, List<string> synonyms)
        {
            DesignEntity = designEntity;
            Synonyms = synonyms;
        }
    }
}
