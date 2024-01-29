using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmethystOS
{
    public class HeadersAndFooters
    {
        public void EscapeFooterMessage()
        {
            Console.SetCursorPosition(0, Console.WindowHeight - 1);
            Console.WriteLine("Press escape key to return to the menu...");
            while (Console.ReadKey().Key != ConsoleKey.Escape) { }
        }        
    }
}
