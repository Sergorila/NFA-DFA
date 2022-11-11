using System;

namespace Automat
{
    class Program
    {
        static void Main(string[] args)
        {

            //dfa d = new dfa(@"C:\Users\Sergey\source\repos\Automat\Automat\TextFile1.txt");
            //d.ShowData();
            //d.ShowResult(d.IsAccept("aaabc"));

            nfa n = new nfa(@"C:\Users\Sergey\source\repos\Automat\Automat\TextFile2.txt");
            n.ShowData();
            n.ShowEps();
            n.ShowResult(n.IsAccept("a"));
        }
    }
}
