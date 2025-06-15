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

            if (!candidates.Any()) return "none";

            string? designEntity = query.Declarations
                .FirstOrDefault(d => d.Synonyms.Contains(query.SelectSynonym))
                ?.DesignEntity;

            if (!string.IsNullOrEmpty(designEntity) && designEntity != "stmt" && designEntity !="variable")
            {
                var filtered = candidates
                    .Where(c =>
                    {
                        if (!int.TryParse(c, out int stmtId)) return false;

                        return designEntity switch
                        {
                            "assign" => _pkb.IsAssign(stmtId),
                            "while" => _pkb.IsWhile(stmtId),
                            "if" => _pkb.IsIf(stmtId),
                            _ => true 
                        };
                    })
                    .ToList();

                if (!filtered.Any()) return "none";
                return FormatResults(query.SelectSynonym, filtered);
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
                case "calls":
                case "calls*":
                    return EvaluateCalls(clause, clause.Relation.EndsWith("*"));
                default:
                    throw new Exception($"Unknown relationship: {clause.Relation}");
            }
        }


        private List<string> EvaluateFollows(RelationshipClause clause, bool isTransitive)
        {
            int? arg1 = ParseAsStmtNumber(clause.Arg1);
            int? arg2 = ParseAsStmtNumber(clause.Arg2);

            // RACZEJ DZIALA ZLE GetFollows
            // Follows(3, s) => 3 precedes s (GetFollows)
            if (arg1.HasValue && clause.Arg2 != "_")
            {
                var result = isTransitive
                    ? _pkb.GetFollowsStar(arg1.Value)
                    : _pkb.GetFollows(arg1.Value);

                return FilterResults(result, clause.Arg2);
            }

            // DZIALA PRAWIDLOWO 
            // Follows(s, 3) => s precedes 3 GetFollowedBy)
            if (arg2.HasValue && clause.Arg1 != "_")
            {
                var result = isTransitive
                    ? _pkb.GetFollowedStarBy(arg2.Value)
                    : _pkb.GetFollowedBy(arg2.Value);

                return FilterResults(result, clause.Arg1);
            }

            // Follows(3, 4)
            if (arg1.HasValue && arg2.HasValue)
            {
                var result = isTransitive
                    ? _pkb.GetFollowsStar(arg1.Value)
                    : _pkb.GetFollows(arg1.Value);

                return result.Contains(arg2.Value)
                    ? new List<string> { arg2.Value.ToString() }
                    : new List<string>();
            }

            // Follows(_, _) — czy jakas relacja istnieje
            if (clause.Arg1 == "_" && clause.Arg2 == "_")
            {
                return _pkb.GetFollowList().Any()
                    ? new List<string> { "true" }
                    : new List<string>();
            }

            // Follows(_, s) => znajdź wszystkie s mające poprzednika
            if (clause.Arg1 == "_" && clause.Arg2 != "_")
            {
                var target = clause.Arg2;
                var candidates = _pkb.GetFollowList()
                    .Select(f => f.FollowingStmtId)
                    .Distinct();

                return FilterResults(candidates, target);
            }

            // Follows(s, _) => znajdź wszystkie s mające nastepnika
            if (clause.Arg1 != "_" && clause.Arg2 == "_")
            {
                var target = clause.Arg1;
                var candidates = _pkb.GetFollowList()
                    .Select(f => f.PrecedingStmtId)
                    .Distinct();

                return FilterResults(candidates, target);
            }

            throw new NotImplementedException("Unhandled Follows case exception");
        }

        private List<string> EvaluateParent(RelationshipClause clause, bool isTransitive)
        {
            int? parent = ParseAsStmtNumber(clause.Arg1);
            int? child = ParseAsStmtNumber(clause.Arg2);

            //DZIALA  
            // Przypadek: Parent(3, s) znajdz dzieci bezposrednio 3 lub posrednio 3 czyli poza tym jednym blokiem
            if (parent.HasValue && clause.Arg2 != "_")
            {
                var children = isTransitive
                    ? _pkb.GetDescendants(parent.Value)
                    : _pkb.GetChildren(parent.Value);

                return FilterResults(children, clause.Arg2);
            }

            //DZIALA DOBRZE
            // Przypadek: Parent(s, 3) znajdz s'y ktore sa rodzicami 3 lub wszystkimi przodkami 3
            if (child.HasValue && clause.Arg1 != "_")
            {
                var parents = isTransitive
                    ? _pkb.GetAncestors(child.Value)
                    : _pkb.GetParentedBy(child.Value);
                 return FilterResults(parents, clause.Arg1,"while");
            }

            // DZIALA 
            // Przypadek: Parent(3, 4) - jesli 3 jest posrednim lubbezposrednim rodzicem 4 zwroc 4 
            if (parent.HasValue && child.HasValue)
            {
                bool isValid = isTransitive
                    ? _pkb.IsParentStar(parent.Value, child.Value)
                    : _pkb.IsParent(parent.Value, child.Value);
                return isValid ? new List<string> { child.Value.ToString() } : new List<string>();
            }

            //dziala zwraca true
            // Przypadek: Parent(_, _) czy istnieje jakakolwiek relacja Parent
            if (clause.Arg1 == "_" && clause.Arg2 == "_")
            {
                return _pkb.GetParentList().Any()
                    ? new List<string> { "true" }
                    : new List<string>();
            }

            //dziala 
            // Przypadek: Parent(_, s) instrukcje s, które mają jakiegos rodzica
            if (clause.Arg1 == "_" && clause.Arg2 != "_")
            {
                var target = clause.Arg2;
                var candidates = _pkb.GetParentList()
                    .Select(p => p.ChildStmtId)
                    .Distinct();
                return FilterResults(candidates, target);
            }

            //DZIALA 
            // Przypadek: Parent(s, _) - instrukcje s, które mają min. jedno dziecko
            if (clause.Arg1 != "_" && clause.Arg2 == "_")
            {
                var target = clause.Arg1;
                var candidates = _pkb.GetParentList()
                    .Select(p => p.ParentStmtId)
                    .Distinct();
                return FilterResults(candidates, target);
            }


            throw new NotImplementedException("Unhandled Parent/Parent* case.");
        }

        // tutaj to parser nie obsługuje procedure tak naprawde a reszta powinna zadzialac 
        private List<string> EvaluateModifies(RelationshipClause clause)
        {
            //DZIALA 
            // Przypadek: Modifies(3, "x1") lub Modifies(3, v) lub Modifies(3, _)
            if (int.TryParse(clause.Arg1, out int stmtNum))
            {
                var vars = _pkb.GetModified(stmtNum);
                // Jeśli Arg2 to synonim (np. v), to nie filtrujemy, tylko zwracamy wszystko
                if (!clause.Arg2.StartsWith("\"") && clause.Arg2 != "_")
                {
                    return vars;
                }
                // W przeciwnym wypadku (literal string lub "_") – filtrujemy
                return FilterResults(vars, clause.Arg2);
            }

            // Przypadek: Modifies(s, "x") znajdz s ktore modyfikuja x
            if (clause.Arg1 != "_" && !clause.Arg1.StartsWith("\"") && clause.Arg2.StartsWith("\""))
            {
                string varName = clause.Arg2.Trim('"');
                var stmts = _pkb.GetModifies(varName);
                return FilterResults(stmts, clause.Arg1);
            }

            // Przypadek: Modifies("proc", "x")
            if (clause.Arg1.StartsWith("\"") && clause.Arg2.StartsWith("\""))
            {
                string proc = clause.Arg1.Trim('"');
                string var = clause.Arg2.Trim('"');
                if (_pkb.IsProcModifies(proc, var))
                    return new List<string> { $"\"{proc}\"" };
                return new List<string>();
            }

            // Przypadek: Modifies("proc", _)
            if (clause.Arg1.StartsWith("\"") && clause.Arg2 == "_")
            {
                string proc = clause.Arg1.Trim('"');
                var vars = _pkb.GetProcModifies(proc);
                return vars.Any()
                    ? new List<string> { $"\"{proc}\"" }
                    : new List<string>();
            }

            // Przypadek: Modifies(_, "x")
            if (clause.Arg1 == "_" && clause.Arg2.StartsWith("\""))
            {
                string var = clause.Arg2.Trim('"');
                var stmts = _pkb.GetModifies(var);
                return stmts.Select(s => s.ToString()).ToList();
            }

            // Przypadek: Modifies(_, _) BRAKUJE FUNKCJI 
            // if (clause.Arg1 == "_" && clause.Arg2 == "_")
            // {
            //     return _pkb.GetModifyList().Any() // BRAKUJE FUNKCJI 
            //         ? new List<string> { "true" }
            //         : new List<string>();
            // }

            throw new NotImplementedException("Unhandled Modifies query form.");
        }

        private List<string> EvaluateUses(RelationshipClause clause)
        {
            //dziala
            // Uses(3, "x") / Uses(3, v) / Uses(3, _)
            if (int.TryParse(clause.Arg1, out int stmtNum))
            {
                var vars = _pkb.GetUsed(stmtNum);

                if (clause.Arg2.StartsWith("\""))
                {
                    string varName = clause.Arg2.Trim('"');
                    return vars.Contains(varName)
                        ? new List<string> { stmtNum.ToString() }
                        : new List<string>();
                }

                if (!clause.Arg2.StartsWith("\"") && clause.Arg2 != "_")
                {
                    return vars;
                }

                return FilterResults(vars, clause.Arg2);
            }
            //dziala 
            // Uses(s, "x")
            if (clause.Arg1 != "_" && !clause.Arg1.StartsWith("\"") && clause.Arg2.StartsWith("\""))
            {
                string varName = clause.Arg2.Trim('"');
                var stmts = _pkb.GetUses(varName);
                return FilterResults(stmts, clause.Arg1);
            }

            //BRAKUJE PROCEDURE W PARSERZE 
            //Uses("proc", "x")
            if (clause.Arg1.StartsWith("\"") && clause.Arg2.StartsWith("\""))
            {
                string proc = clause.Arg1.Trim('"');
                string var = clause.Arg2.Trim('"');
                return _pkb.IsProcUses(proc, var)
                    ? new List<string> { $"\"{proc}\"" }
                    : new List<string>();
            }

            //BRAK PRODCEDUURE W PARSERZE 
            // Uses("proc", _)
            // if (clause.Arg1.StartsWith("\"") && clause.Arg2 == "_")
            // {
            //     string proc = clause.Arg1.Trim('"');
            //     var vars = _pkb.GetProcUses(proc);
            //     return vars.Any()
            //         ? new List<string> { $"\"{proc}\"" }
            //         : new List<string>();
            // }

            //DZIALA 
            // Uses(_, "x")
            if (clause.Arg1 == "_" && clause.Arg2.StartsWith("\""))
            {
                string var = clause.Arg2.Trim('"');
                var stmts = _pkb.GetUses(var);
                return stmts.Select(s => s.ToString()).ToList();
            }

            //BRAK FUNKCJI W PKB 
            // // Uses(_, _)
            // if (clause.Arg1 == "_" && clause.Arg2 == "_")
            // {
            //     return _pkb.GetUseList().Any()
            //         ? new List<string> { "true" }
            //         : new List<string>();
            // }

            throw new NotImplementedException("Unhandled Uses query form.");
        }


        private List<string> EvaluateCalls(RelationshipClause clause, bool isTransitive)
        {
            string caller = clause.Arg1;
            string callee = clause.Arg2;

            // Calls("Main", "Init")
            if (caller.StartsWith("\"") && callee.StartsWith("\""))
            {
                string callerProc = caller.Trim('"');
                string calleeProc = callee.Trim('"');

                bool result = isTransitive
                    ? _pkb.IsCallsStar(callerProc, calleeProc)
                    : _pkb.IsCalls(callerProc, calleeProc);

                return result ? new List<string> { $"\"{calleeProc}\"" } : new List<string>();
            }

            // Calls("Main", p)
            if (caller.StartsWith("\"") && !callee.StartsWith("\"") && callee != "_")
            {
                string callerProc = caller.Trim('"');
                var callees = _pkb.GetCallees(callerProc, isTransitive);
                return FilterResults(callees, callee,true);
            }

            // Calls(p, "Init")
            if (!caller.StartsWith("\"") && caller != "_" && callee.StartsWith("\""))
            {
                string calleeProc = callee.Trim('"');
                var callers = _pkb.GetCallers(calleeProc, isTransitive);
                return FilterResults(callers, caller,true );
            }

            // Calls(p1, p2)
            if (!caller.StartsWith("\"") && caller != "_" &&
                !callee.StartsWith("\"") && callee != "_")
            {
                var callees = _pkb.GetCallees(null, isTransitive); // get all caller-callee pairs
                                                                   // Tu najlepiej byłoby mieć GetCallsPairs(), ale jako workaround:
                var allCallers = _pkb.GetCallers("*", isTransitive); // get all callers
                var results = new List<string>();

                foreach (var proc in allCallers)
                {
                    var innerCallees = _pkb.GetCallees(proc, isTransitive);
                    foreach (var calleeName in innerCallees)
                    {
                        results.Add($"{proc},{calleeName}");
                    }
                }

                // Tu trzeba by zmapować na pary i przefiltrować – pominę bo to edge-case
                return new List<string>(); // ✴ Można rozbudować jak będzie potrzeba
            }

            // Calls(_, _)
            if (caller == "_" && callee == "_")
            {
                return _pkb.GetCallees("*", isTransitive).Any()
                    ? new List<string> { "true" }
                    : new List<string>();
            }

            // Calls(_, p)
            if (caller == "_" && callee != "_")
            {
                var callees = _pkb.GetCallees("*", isTransitive);
                return FilterResults(callees, callee);
            }

            // Calls(p, _)
            if (caller != "_" && callee == "_")
            {
                var callers = _pkb.GetCallers("*", isTransitive);
                return FilterResults(callers, caller);
            }

            throw new NotImplementedException("Unhandled Calls/Calls* case.");
        }


        // TU ABY ZROBIUC WIECEJ TRZEBA public class WithClause rozbudowac o iinne przypoadki 
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

        private List<string> FilterResults(IEnumerable<int> stmts, string target, string designEntity = null)
        {
            if (target == "_")
            {
                return stmts
                    .Where(id => MatchesDesignEntityIfNeeded(id, designEntity))
                    .Select(x => x.ToString())
                    .ToList();
            }

            if (int.TryParse(target, out int val))
            {
                return stmts.Contains(val) && MatchesDesignEntityIfNeeded(val, designEntity)
                    ? new() { val.ToString() }
                    : new();
            }

            return stmts
                .Where(id => MatchesDesignEntityIfNeeded(id, designEntity))
                .Select(x => x.ToString())
                .ToList();
        }
        private bool MatchesDesignEntityIfNeeded(int stmtId, string? designEntity)
        {
            if (string.IsNullOrEmpty(designEntity) || designEntity == "stmt")
                return true;

            return designEntity.ToLower() switch
            {
                "while" => _pkb.IsWhile(stmtId),
                "assign" => _pkb.IsAssign(stmtId),
                "if" => _pkb.IsIf(stmtId),
                _ => true  // fallback: jeśli nie znamy typu, nie filtrujemy
            };
        }


        private List<string> FilterResults(IEnumerable<string> values, string target, bool isSynonym = false)
        {
            if (target == "_") return values.ToList();
            if (target.StartsWith("\"")) target = target.Trim('"');
            if (isSynonym == true) return values.ToList();
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
