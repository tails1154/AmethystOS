using Cosmos.System.FileSystem;
using Cosmos.System.FileSystem.VFS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace AmethystOS
{
    public class Notepad
    {
        private FileSystem fileSystem;
        private string currentDirectory = "0:\\";


        public void OpenNotepad()
        {

            Console.CursorVisible = true;
            Console.Write("Enter File Name: ");
            Console.ForegroundColor = ConsoleColor.Yellow;
            string fileName = Console.ReadLine();
            Console.ForegroundColor = ConsoleColor.Black;

            try
            {
                fileName = fileName.Trim();
                if (!fileName.EndsWith(".txt"))
                {
                    fileName += ".txt";
                }

                VFSManager.CreateFile($@"{currentDirectory}\{fileName}");
            }
            catch (Exception e)
            {
                Console.WriteLine("Creating file error: " + e.ToString());
            }
            NotepadHeader(fileName);

            fileSystem.EditFile(fileName);
        }

        public void NotepadHeader(string fileName)
        { 
            Console.BackgroundColor = ConsoleColor.Magenta;
            Console.WriteLine($"{fileName} - Notepad");
            Console.BackgroundColor = ConsoleColor.DarkCyan;
        }
    }
}
