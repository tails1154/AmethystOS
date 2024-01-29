using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AmethystOS
{
    internal class ForegroundColors
    {
        public ForegroundColors(string str, params (string substring, ConsoleColor color)[] colors)
        {
            var words = Regex.Split(str, @"( )");

            foreach (var word in words)
            {
                (string substring, ConsoleColor color) cl = colors.FirstOrDefault(x => x.substring.Equals("{" + word + "}"));
                if (cl.substring != null)
                {
                    Console.ForegroundColor = cl.color;
                    Console.Write(cl.substring.Substring(1, cl.substring.Length - 2));
                    Console.ResetColor();
                }
                else
                {
                    Console.Write(word);
                }
            }
        }
    }
}
