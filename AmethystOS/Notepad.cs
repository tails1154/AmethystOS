using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AmethystOS
{
    public class Notepad
    {
        public void openNotepad()
        {
            Console.Write("Enter file name: ");
            var fileName = Console.ReadLine().Trim();
        }


    }
}
