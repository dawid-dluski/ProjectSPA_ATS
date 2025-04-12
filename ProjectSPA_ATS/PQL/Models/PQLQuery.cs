using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ProjectSPA_ATS.PQL.Models
{
    public class PQLQuery
    {
        public List<Declaration> Declarations { get; }
        public string SelectSynonym { get; }
        public RelationshipClause SuchThat { get; }
        public WithClause With { get; }

        public PQLQuery(List<Declaration> decls, string select, RelationshipClause suchThat, WithClause with)
        {
            Declarations = decls;
            SelectSynonym = select;
            SuchThat = suchThat;
            With = with;
        }
    }
}
