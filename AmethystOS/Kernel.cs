using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using Sys = Cosmos.System;

namespace AmethystOS
{
    public class Kernel : Sys.Kernel
    {
        private FileSystem fileSystem;
        private Notepad notepad;
        protected override void BeforeRun()
        {
            Console.Clear();
            fileSystem = new FileSystem();
            notepad = new Notepad();
            Console.WriteLine("Cosmos booted successfully. Type a line of text to get it echoed back.");
        }

        protected override void Run()
        {
            int selectedIndex = 0;
            string[] menuOptions = { "File Manager", "Notepad" };

            while (true)
            {
                Console.Clear();
                Console.WriteLine("Select an option:");

                for (int i = 0; i < menuOptions.Length; i++)
                {
                    if (i == selectedIndex)
                    {
                        Console.BackgroundColor = ConsoleColor.DarkCyan;
                        Console.ForegroundColor = ConsoleColor.Black;
                    }

                    Console.WriteLine(menuOptions[i]);

                    Console.ResetColor();
                }

                ConsoleKeyInfo key = Console.ReadKey();

                switch (key.Key)
                {
                    case ConsoleKey.UpArrow:
                        selectedIndex = (selectedIndex - 1 + menuOptions.Length) % menuOptions.Length;
                        break;
                    case ConsoleKey.DownArrow:
                        selectedIndex = (selectedIndex + 1) % menuOptions.Length;
                        break;
                    case ConsoleKey.Enter:
                        Console.Clear();
                        switch (menuOptions[selectedIndex].ToLower())
                        {
                            case "file manager":
                                fileSystem.ListFiles();
                                break;
                            case "notepad":
                                notepad.openNotepad();
                                break;
                            default:
                                Console.WriteLine("Invalid option selected.");
                                break;
                        }
                        Console.WriteLine("Press any key to return to the menu...");
                        Console.ReadKey();
                        break;
                }
            }
        }

    }

}
