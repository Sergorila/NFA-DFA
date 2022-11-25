using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Automat
{
    public class nfa
    {
        private HashSet<string> _states;
        private HashSet<string> _alphabet;
        //private Dictionary<string, List<KeyValuePair<string, List<string>>>> _transitions;
        private Dictionary<string, Dictionary<string, List<string>>> _transitions;
        private string _initialState;
        private HashSet<string> _finalStates;
       // private Dictionary<string, List<string>> _epsTransitions;

        public nfa(HashSet<string> states, HashSet<string> alphabet, Dictionary<string, Dictionary<string, List<string>>> transitions,
            string initialState, HashSet<string> finalStates)
        {
            _states = states;
            _alphabet = alphabet;
            _transitions = transitions;
            _initialState = initialState;
            _finalStates = finalStates;
        }
        public nfa(string path)
        {
            using (StreamReader sr = new StreamReader(path))
            {
                string[] input;
                _alphabet = new HashSet<string>();
                _states = new HashSet<string>();
                _finalStates = new HashSet<string>();
                _transitions = new Dictionary<string, Dictionary<string, List<string>>>();
                //_epsTransitions = new Dictionary<string, List<string>>();

                _alphabet = sr.ReadLine().Split(' ').ToHashSet();
                input = sr.ReadLine().Split(' ');

                foreach (var item in input)
                {
                    if (item.Contains("->"))
                    {
                        _initialState = item.Substring(2, item.Length - 2);
                        _states.Add(_initialState);
                    }
                    else if (item.Contains('*'))
                    {
                        _finalStates.Add(item.Substring(1, item.Length - 1));
                        _states.Add(item.Substring(1, item.Length - 1));
                        Console.WriteLine(item.Substring(1, item.Length - 1));
                    }
                    else
                    {
                        _states.Add(item);
                    }
                }

                string s;
                while ((s = sr.ReadLine()) != null)
                {
                    input = s.Split(';');
                    foreach (var state in _states)
                    {
                        if (state == input[0])
                        {
                            Dictionary<string, List<string>> transitions = new Dictionary<string, List<string>>();
                            for (int i = 1; i < input.Length; i++)
                            {
                                if (input[i].Contains('|'))
                                {
                                    var eps = input[i].Split('|');
                                    if (eps[1].Split(' ').Length > 1)
                                    {
                                        transitions["_"] = eps[1].Split(' ').ToList();
                                        //_epsTransitions.Add(state, eps[1].Split(' ').ToList());
                                    }
                                    else
                                    {
                                        transitions["_"] = new List<string>() { eps[1] };
                                        //_epsTransitions.Add(state, new List<string>() { eps[1]});
                                    }
                                    var temp = eps[0].Split(',');
                                    if (temp[1].Split(' ').Length > 1)
                                    {
                                        transitions[temp[0]] = temp[1].Split(' ').ToList();
                                    }
                                    else
                                    {
                                        transitions[temp[0]] = new List<string>() { temp[1] };
                                    }
                                }
                                else if (input.Length > 2)
                                {
                                    var temp = input[i].Split(',');
                                    transitions.Add(temp[0], temp[1].Split(' ').ToList());
                                }
                            }
                            _transitions.Add(state, transitions);
                        }
                    }
                }
            }
        }

        //public void ShowEps()
        //{
        //    foreach(var eps in _epsTransitions.Keys)
        //    {
        //        Console.Write("{0}", eps);
        //        foreach (var t in _epsTransitions[eps])
        //        {
        //            Console.Write("{0} ", t);
        //        }
        //        Console.WriteLine();
        //    }
        //}

        public bool IsAccept(string word)
        {
            ShowWord(word);
            Queue<KeyValuePair<int, string>> q = new Queue<KeyValuePair<int, string>>();
            q.Enqueue(new KeyValuePair<int, string>(0, _initialState));
            bool f = false;

            while (q.Count != 0 && !f)
            {
                var frontq = q.Dequeue();
                int idx = frontq.Key;
                string state = frontq.Value;
                if (idx == word.Length)
                {
                    if (_finalStates.Contains(state))
                    {
                        f = true;
                    }
                }
                else if (_transitions.ContainsKey(state))
                {
                    foreach (var transition in _transitions[state])
                    {
                        var d = transition.Key;
                        var states = transition.Value;

                        if (d == "_")
                        {
                            foreach (var st in states)
                            {
                                q.Enqueue(new KeyValuePair<int, string>(idx, st));
                                ShowTransition(word[idx].ToString(), state, st);
                            }
                        }
                        else if (word[idx].ToString() == d)
                        {
                            foreach (var st in states)
                            {
                                q.Enqueue(new KeyValuePair<int, string>(idx + 1, st));
                                ShowTransition(word[idx].ToString(), state, st);
                            }
                        }
                    }
                }
            }

            if (word == "") return true;

            return f;
        }

        public void ShowWord(string s)
        {
            Console.WriteLine("Цепочка: {0}", s);
        }

        public void ShowTransition(string symb, string prevState, string nextState)
        {
            Console.WriteLine("Слово: {0}, переход {1} -> {2}", symb, prevState, nextState);
        }

        public void ShowData()
        {
            Console.Write("\t");
            foreach (var symbol in _alphabet)
            {
                Console.Write("\t{0}", symbol);
            }
            Console.WriteLine();
            Console.WriteLine();
            foreach (var state in _states)
            {
                Console.Write("\t");
                if (state == _initialState)
                {
                    Console.Write("->");
                }
                if (_finalStates.Contains(state))
                {
                    Console.Write("*");
                }
                Console.Write(state);

                foreach (var trans in _transitions[state])
                {
                    if (trans.Key != "_")
                    {
                        Console.Write("\t{");
                        foreach (var t in trans.Value)
                        {
                            Console.Write("{0} ", t);
                        }
                        Console.Write("}");
                    }
                    
                }
                foreach (var trans in _transitions[state])
                {
                    if (trans.Key == "_")
                    {
                        Console.Write(" | ");
                        foreach (var t in trans.Value)
                        {
                            Console.Write("{0} ", t);
                        }
                    }

                }
                Console.WriteLine();
                Console.WriteLine();
            }
        }

        public void ShowResult(bool res)
        {
            if (res)
            {
                Console.WriteLine("Принято");
            }
            else
            {
                Console.WriteLine("Не принято");
            }
        }

        public nfa RemoveEps()
        {
            var statesPrime = _states;
            var transitionsPrime = _transitions;
            var trInitState = _initialState;
            var trFinalStates = _finalStates;

            if (IsContainsEps())
            {
                transitionsPrime = new Dictionary<string, Dictionary<string, List<string>>>();

                foreach (var q in statesPrime)
                {
                    var closureStates = GetEpsClosure(q);

                    foreach (var sigma in _alphabet)
                    {
                        List<string> toEpsClosure = new List<string>();
                        List<string> newTransitions = new List<string>();

                        foreach (var closureState in closureStates)
                        {
                            if (_finalStates.Contains(closureState))
                            {
                                trFinalStates.Add(q);
                            }
                            if (_transitions.ContainsKey(closureState) && _transitions[closureState].ContainsKey(sigma))
                            {
                                toEpsClosure.AddRange(_transitions[closureState][sigma]);
                            }
                        }

                        foreach (var espClosure in toEpsClosure)
                        {
                            newTransitions.AddRange(GetEpsClosure(espClosure));
                        }

                        if (!transitionsPrime.ContainsKey(q))
                        {
                            transitionsPrime[q] = new Dictionary<string, List<string>>();
                        }

                        if (sigma != "_")
                        {
                            transitionsPrime[q][sigma] = new HashSet<string>(newTransitions).ToList();
                        }

                    }
                }
            }

            return new nfa(statesPrime, _alphabet, transitionsPrime, trInitState, trFinalStates);
        }

        public bool IsContainsEps()
        {
            foreach (var state in _transitions.Keys)
            {
                if (_transitions[state].ContainsKey("_"))
                {
                    return true;
                }
            }
            return false;
        }

        public List<string> GetEpsClosure(string q, List<string> visited = null)
        {
            List<string> ans = new List<string>() { q };

            if (visited == null)
            {
                visited = new List<string>() { q };
            }

            if (_transitions.ContainsKey(q))
            {
                if (_transitions[q].ContainsKey("_"))
                {
                    foreach (var st in _transitions[q]["_"])
                    {
                        visited.Add(st);
                        List<string> temp = new List<string>();
                        foreach (var k in GetEpsClosure(st, visited))
                        {
                            if (!ans.Contains(k))
                            {
                                temp.Add(k);
                            }
                        }
                        ans.AddRange(temp);
                    }
                }
            }

            return ans;
        }

        //public dfa GetDFA()
        //{
        //    var localNFA = RemoveEps();

        //    List<string> statesPrime = new List<string>();
        //    Dictionary<string, Dictionary<string, List<string>>> transitionsPrime = new Dictionary<string, Dictionary<string, List<string>>>();
        //    Queue<List<string>> queue = new Queue<List<string>>();
        //    List<List<string>> visited = new List<List<string>>() { new List<string> { localNFA._initialState } };
        //    queue.Enqueue(new List<string>() { localNFA._initialState });

        //    while(queue.Count != 0)
        //    {
        //        var qs = queue.Dequeue();
        //        Dictionary<string, List<string>> t = new Dictionary<string, List<string>>();

        //        foreach (var q in qs)
        //        {
        //            if (localNFA._transitions.ContainsKey(q))
        //            {
        //                foreach (var s in localNFA._transitions[q].Keys)
        //                {
        //                    var tmp = new List<string>(localNFA._transitions[q][s]);

        //                    if (tmp.Count != 0)
        //                    {
        //                        if (t.ContainsKey(s))
        //                        {
        //                            List<string> tt = new List<string>();
        //                            foreach (var k in tmp)
        //                            {
        //                                if (!t[s].Contains(k))
        //                                {
        //                                    tt.Add(k);
        //                                }
        //                            }
        //                            t[s].AddRange(tt);
        //                        }
        //                    }
        //                    else
        //                    {
        //                        t[s] = new List<string>(tmp);
        //                    }
        //                }
        //            }
        //        }

        //        foreach (var v in t.Keys)
        //        {
        //            t[v].Sort();
        //            var temp = new List<string>(t[v]);

        //            if (!visited.Contains(temp))
        //            {
        //                queue.Enqueue(temp);
        //                visited.Add(temp);
        //            }
        //            // t[v]
        //        }

        //        transitionsPrime[qs] =
        //    }

        //    HashSet<string> finalstatesPrime = new HashSet<string>();

        //    foreach (var qs in statesPrime)
        //    {
        //        foreach (var q in qs)
        //        {

        //        }
        //    }




        //}   


    }
}
