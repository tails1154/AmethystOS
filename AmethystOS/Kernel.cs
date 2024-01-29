using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Sys = Cosmos.System;

namespace AmethystOS
{
    public class Kernel : Sys.Kernel
    {
        private FileSystem fileSystem;
        private Notepad notepad;
        private Calculator calculator;
        private HeadersAndFooters headersAndFooters;

        protected override void BeforeRun()
        {
            Console.BackgroundColor = ConsoleColor.Gray;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.Clear();
            fileSystem = new FileSystem();
            notepad = new Notepad();
            calculator = new Calculator();
            headersAndFooters = new HeadersAndFooters();
            Console.WriteLine("Cosmos booted successfully. Type a line of text to get it echoed back.");
        }

        protected override void Run()
        {
            int selectedIndex = 0;
            string[] menuOptions = {
            "      Notepad      ",
            "    File Manager   ",
            " Simple Calculator "};

            while (true)
            {
                Console.BackgroundColor = ConsoleColor.Gray;
                Console.ForegroundColor = ConsoleColor.Black;
                Console.Clear();
                Console.CursorVisible = false;

                int consoleCenterX = Console.WindowWidth / 2;
                int consoleCenterY = Console.WindowHeight / 2;

                int startPrintY = consoleCenterY - (menuOptions.Length / 2);

                for (int i = 0; i < menuOptions.Length; i++)
                {
                    Console.BackgroundColor = ConsoleColor.Gray;
                    Console.ForegroundColor = ConsoleColor.Black;

                    if (i == selectedIndex)
                    {
                        Console.BackgroundColor = ConsoleColor.DarkCyan;
                        Console.ForegroundColor = ConsoleColor.White;
                    }

                    int menuOptionCenterX = consoleCenterX - (menuOptions[i].Length / 2);

                    Console.SetCursorPosition(menuOptionCenterX, startPrintY + i);
                    Console.WriteLine(menuOptions[i]);
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
                        switch (menuOptions[selectedIndex].Trim().ToLower())
                        {
                            case "file manager":
                                fileSystem.OpenFileManager("0:\\");
                                break;
                            case "notepad":
                                notepad.OpenNotepad();
                                break;
                            case "calculator":
                                Console.CursorVisible = true;
                                calculator.OpenCalculator();
                                break;
                            default:
                                Console.WriteLine("Invalid option selected.");
                                break;
                        }
                        headersAndFooters.EscapeFooterMessage();
                        break;
                }
            }
        }
    }
}
