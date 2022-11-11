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
        private Dictionary<string, List<KeyValuePair<string, List<string>>>> _transitions;
        private string _initialState;
        private HashSet<string> _finalStates;
        private Dictionary<string, List<string>> _epsTransitions;

        public nfa(string path)
        {
            using (StreamReader sr = new StreamReader(path))
            {
                string[] input;
                _alphabet = new HashSet<string>();
                _states = new HashSet<string>();
                _finalStates = new HashSet<string>();
                _transitions = new Dictionary<string, List<KeyValuePair<string, List<string>>>>();
                _epsTransitions = new Dictionary<string, List<string>>();

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
                            List<KeyValuePair<string, List<string>>> transitions = new List<KeyValuePair<string, List<string>>>();
                            for (int i = 1; i < input.Length; i++)
                            {
                                if (input[i].Contains('|'))
                                {
                                    var eps = input[i].Split('|');
                                    if (eps[1].Split(' ').Length > 1)
                                    {
                                        _epsTransitions.Add(state, eps[1].Split(' ').ToList());
                                    }
                                    else
                                    {
                                        _epsTransitions.Add(state, new List<string>() { eps[1]});
                                    }
                                    var temp = eps[0].Split(',');
                                    if (temp[1].Split(' ').Length > 1)
                                    {
                                        transitions.Add(new KeyValuePair<string, List<string>>(temp[0], temp[1].Split(' ').ToList()));
                                    }
                                    else
                                    {
                                        transitions.Add(new KeyValuePair<string, List<string>>(temp[0], new List<string>() { temp[1] }));
                                    }
                                }
                                else
                                {
                                    var temp = input[i].Split(',');
                                    transitions.Add(new KeyValuePair<string, List<string>>(temp[0], temp[1].Split(' ').ToList()));
                                }
                            }
                            _transitions.Add(state, transitions);
                        }
                    }
                }
            }
        }

        public void ShowEps()
        {
            foreach(var eps in _epsTransitions.Keys)
            {
                Console.Write("{0}", eps);
                foreach (var t in _epsTransitions[eps])
                {
                    Console.Write("{0} ", t);
                }
                Console.WriteLine();
            }
        }

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
                Console.WriteLine(state);
                if (idx == word.Length)
                {
                    if (_finalStates.Contains(state))
                    {
                        f = true;
                    }
                }
                if (_epsTransitions.ContainsKey(state))
                {
                    foreach (var eps in _epsTransitions[state])
                    {
                        q.Enqueue(new KeyValuePair<int, string>(idx, eps));
                        Console.Write("eps ");
                        ShowTransition(word[idx].ToString(), state, eps);
                    }

                }
                else if (_transitions.ContainsKey(state))
                {
                    foreach (var tr in _transitions[state])
                    {
                        if (word[idx].ToString() == tr.Key)
                        {
                            foreach (var st in tr.Value)
                            {
                                q.Enqueue(new KeyValuePair<int, string>(idx + 1, st));
                                Console.WriteLine(idx);
                                ShowTransition(word[idx].ToString(), state, st);
                                idx += 1;
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
                    Console.Write("\t{");
                    foreach(var t in trans.Value)
                    {
                        Console.Write("{0} ", t);
                    }
                    Console.Write("}");
                }
                Console.Write("\t");
                if (_epsTransitions.ContainsKey(state))
                {
                    Console.Write(" | ");
                    foreach (var eps in _epsTransitions[state])
                        Console.Write("{0} ", eps);
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
    }
}
