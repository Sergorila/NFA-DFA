using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Automat
{
    public class dfa
    {
        private HashSet<string> _states;
        private HashSet<string> _alphabet;
        private Dictionary<string, List<KeyValuePair<string, string>>> _transitions;
        private string _initialState;
        private HashSet<string> _finalStates;

        public dfa(HashSet<string> states, HashSet<string> alphabet, Dictionary<string, List<KeyValuePair<string, string>>> transitions,
            string initialState, HashSet<string> finalStates)
        {
            _states = states;
            _alphabet = alphabet;
            _transitions = transitions;
            _initialState = initialState;
            _finalStates = finalStates;
        }
        public dfa(string path)
        {
            using(StreamReader sr = new StreamReader(path))
            {
                string[] input;
                _alphabet = new HashSet<string>();
                _states = new HashSet<string>();
                _finalStates = new HashSet<string>();
                _transitions = new Dictionary<string, List<KeyValuePair<string, string>>>();

                _alphabet = sr.ReadLine().Split(' ').ToHashSet();
                input = sr.ReadLine().Split(' ');

                foreach(var item in input)
                {
                    if (item.Contains("->"))
                    {
                        _initialState = item.Substring(2, item.Length-2);
                        _states.Add(_initialState);
                    }
                    else if (item.Contains('*'))
                    {
                        _finalStates.Add(item.Substring(1, item.Length-1));
                        _states.Add(item.Substring(1, item.Length-1));
                    }
                    else
                    {
                        _states.Add(item);
                    }
                }

                string s;
                while((s = sr.ReadLine()) != null)
                {
                    input = s.Split(':');
                    foreach (var state in _states)
                    {
                        if (state == input[0])
                        {
                            List<KeyValuePair<string, string>> transitions = new List<KeyValuePair<string, string>>();
                            string[] inpTransitions = input[1].Split(' ');
                            foreach (var tr in inpTransitions)
                            {
                                string[] temp = tr.Split(',');
                                transitions.Add(new KeyValuePair<string, string>(temp[0], temp[1]));
                            }

                            _transitions.Add(state, transitions);
                        }
                    }
                }
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

                if (idx == word.Length)
                {
                    if (_finalStates.Contains(state))
                    {
                        f = true;
                    }
                }
                else if (_transitions.ContainsKey(state))
                {
                    foreach (var tr in _transitions[state])
                    {
                        if (word[idx].ToString() == tr.Key)
                        {
                            q.Enqueue(new KeyValuePair<int, string>(idx + 1, tr.Value));
                            ShowTransition(word[idx].ToString(), state, tr.Value);
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
                    Console.Write("\t({0},{1})", trans.Key, trans.Value);
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
