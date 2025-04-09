using ProjectSPA_ATS.PKB;
using ProjectSPA_ATS.PQL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectSPA_ATS.PQL
{
    public class QueryEvaluator
    {
        private readonly IPBKService _pkb;

        public QueryEvaluator(IPBKService pkbService)
        {
            _pkb = pkbService;
        }

        public string EvaluateQuery(PQLQuery query)
        {
            var candidates = EvaluateSuchThatClause(query.SuchThat, query.Declarations);

            if (query.With != null)
            {
                candidates = ApplyWithClause(candidates, query.With);
            }

            return FormatResults(query.SelectSynonym, candidates);
        }

        private List<string> EvaluateSuchThatClause(RelationshipClause clause, List<Declaration> declarations)
        {
            if (clause == null) return new List<string>(); // fallback

            switch (clause.Relation.ToLower())
            {
                case "follows":
                case "follows*":
                    return EvaluateFollows(clause, clause.Relation.EndsWith("*"));
                case "parent":
                case "parent*":
                    return EvaluateParent(clause, clause.Relation.EndsWith("*"));
                case "modifies":
                    return EvaluateModifies(clause);
                case "uses":
                    return EvaluateUses(clause);
                default:
                    throw new Exception($"Unknown relationship: {clause.Relation}");
            }
        }

        private List<string> EvaluateFollows(RelationshipClause clause, bool isTransitive)
        {
            int? arg1 = ParseAsStmtNumber(clause.Arg1);
            int? arg2 = ParseAsStmtNumber(clause.Arg2);

            if (arg1.HasValue && clause.Arg2 != "_")
            {
                var followed = isTransitive
                    ? _pkb.GetFollowedStarBy(arg1.Value)
                    : new List<int> { _pkb.GetFollowedBy(arg1.Value) };

                return FilterResults(followed, clause.Arg2);
            }

            if (arg2.HasValue && clause.Arg1 != "_")
            {
                var follows = isTransitive
                    ? _pkb.GetFollowsStar(arg2.Value)
                    : new List<int> { _pkb.GetFollows(arg2.Value) };

                return FilterResults(follows, clause.Arg1);
            }

            throw new NotImplementedException("Only partial Follows queries implemented.");
        }

        private List<string> EvaluateParent(RelationshipClause clause, bool isTransitive)
        {
            int? parent = ParseAsStmtNumber(clause.Arg1);
            int? child = ParseAsStmtNumber(clause.Arg2);

            if (parent.HasValue)
            {
                var children = isTransitive
                    ? _pkb.GetParentedStarBy(parent.Value)
                    : _pkb.GetParentedBy(parent.Value);

                return FilterResults(children, clause.Arg2);
            }

            throw new NotImplementedException("Parent/Parent* reverse queries not yet implemented.");
        }

        private List<string> EvaluateModifies(RelationshipClause clause)
        {
            if (int.TryParse(clause.Arg1, out int stmtNum))
            {
                var vars = _pkb.GetModified(stmtNum);
                return FilterResults(vars.Select(v => _pkb.GetVarName(v)), clause.Arg2);
            }

            if (clause.Arg2.StartsWith("\""))
            {
                string varName = clause.Arg2.Trim('"');
                var stmts = _pkb.GetModifies(varName);
                return stmts.Select(s => s.ToString()).ToList();
            }

            throw new NotImplementedException("Modifies(procedure, _) not yet implemented.");
        }

        private List<string> EvaluateUses(RelationshipClause clause)
        {
            if (int.TryParse(clause.Arg1, out int stmtNum))
            {
                var vars = _pkb.GetUsed(stmtNum);
                return FilterResults(vars.Select(v => _pkb.GetVarName(v)), clause.Arg2);
            }

            if (clause.Arg2.StartsWith("\""))
            {
                string varName = clause.Arg2.Trim('"');
                var stmts = _pkb.GetUses(varName);
                return stmts.Select(s => s.ToString()).ToList();
            }

            throw new NotImplementedException("Uses(procedure, _) not yet implemented.");
        }

        private List<string> ApplyWithClause(List<string> inputs, WithClause clause)
        {
            if (clause.ValueType == "int")
            {
                return inputs.Where(i => i == clause.Value).ToList();
            }

            if (clause.ValueType == "string")
            {
                string val = clause.Value.Trim('"');
                return inputs.Where(i => i == val).ToList();
            }

            return inputs;
        }

        private List<string> FilterResults(IEnumerable<int> stmts, string target)
        {
            if (target == "_") return stmts.Select(x => x.ToString()).ToList();
            if (int.TryParse(target, out int val)) return stmts.Contains(val) ? new() { val.ToString() } : new();
            return stmts.Select(x => x.ToString()).ToList();
        }

        private List<string> FilterResults(IEnumerable<string> values, string target)
        {
            if (target == "_") return values.ToList();
            if (target.StartsWith("\"")) target = target.Trim('"');
            return values.Contains(target) ? new() { target } : new();
        }

        private int? ParseAsStmtNumber(string arg)
        {
            return int.TryParse(arg, out int val) ? val : null;
        }

        private string FormatResults(string synonym, List<string> results)
        {
            if (results.Count == 0) return "";
            return string.Join(", ", results.Distinct());
        }
    }

}
