using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cosmos.System.FileSystem.VFS;
using Cosmos.System.FileSystem;
using System.IO;
using Cosmos.System.FileSystem.Listing;
using System.Threading;
using System.IO.Enumeration;
using Cosmos.HAL.BlockDevice.Registers;
using System.Data;

namespace AmethystOS
{
    public class FileSystem
    {
        CosmosVFS fileSystem = new CosmosVFS();
        private string currentDirectory = "0:\\";
        private int consoleCenterX = Console.WindowWidth / 2;
        private int consoleCenterY = Console.WindowHeight / 2;
        private int startPrintY;

        public string CurrentDirectory
        {
            get { return currentDirectory; }
            private set { currentDirectory = value; }
        }

        public FileSystem()
        {
            VFSManager.RegisterVFS(fileSystem);
        }

        public void GetAvailableFreeSpace()
        {
            float availableFreeSpace = VFSManager.GetAvailableFreeSpace("0:\\");
            Console.WriteLine("Available free space: " + availableFreeSpace / 1000000 + "GB");
        }

        public void TotalSize()
        {
            float totalSize = VFSManager.GetTotalSize("0:\\");
            Console.WriteLine("Total size: " + totalSize / 1000000 + "GB");
        }

        public void OpenFileManager(string currentDirectory)
        {
            var directoryList = VFSManager.GetDirectoryListing(currentDirectory);

            int selectedIndex = 0;

            while (true)
            {
                Console.BackgroundColor = ConsoleColor.Gray;
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Press the Spacebar to enter command line");

                for (int i = 0; i < directoryList.Count; i++)
                {
                    if (i == selectedIndex)
                    {
                        Console.ForegroundColor = ConsoleColor.Magenta;
                        Console.Write("> ");
                        Console.ForegroundColor = ConsoleColor.DarkCyan;
                    }
                    else
                    {
                        Console.Write("  ");
                    }

                    if (directoryList[i].mEntryType == DirectoryEntryTypeEnum.Directory)
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Black;
                    }
                    Console.WriteLine(directoryList[i].mName);
                }

                ConsoleKeyInfo key = Console.ReadKey();

                switch (key.Key)
                {
                    case ConsoleKey.UpArrow:
                        selectedIndex = (selectedIndex - 1 + directoryList.Count) % directoryList.Count;
                        break;
                    case ConsoleKey.DownArrow:
                        selectedIndex = (selectedIndex + 1) % directoryList.Count;
                        break;
                    case ConsoleKey.Spacebar:
                        HandleCommandLine(currentDirectory);
                        break;
                    case ConsoleKey.Enter:
                        Console.Clear();
                        string selectedName = directoryList[selectedIndex].mName;

                        if (directoryList[selectedIndex].mEntryType == DirectoryEntryTypeEnum.Directory)
                        {
                            if (selectedName == "Command Line")
                            {
                                HandleCommandLine(currentDirectory);
                            }
                            else
                            {
                                Console.WriteLine("Directory");
                                Console.WriteLine(currentDirectory);
                                HandleDirectorySelection(selectedName);
                            }
                        }
                        else
                        {
                            Console.WriteLine("File");
                            HandleFileSelection(selectedName);
                        }
                        break;
                }
            }
        }

        private void HandleCommandLine(string currentDirectory)
        {
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.Green;

            Console.Clear();
            Console.WriteLine("Type 'help' to view a list of available commands and their uses.");
            while (true)
            {
                Console.CursorVisible = true;
                Console.Write(currentDirectory);
                Console.Write(" > ");
                Console.ForegroundColor = ConsoleColor.Magenta;

                string userInput = Console.ReadLine();
                string[] commandParts = userInput.Split(' ');

                if (Console.KeyAvailable)
                {
                    ConsoleKeyInfo key = Console.ReadKey(true);

                    if (key.Key == ConsoleKey.Escape)
                    {
                        OpenFileManager(currentDirectory);
                        return;
                    }
                }

                if (commandParts.Length > 0)
                {
                    string command = commandParts[0].ToLower();
                    string name = string.Join(" ", commandParts.Skip(1));

                    switch (command)
                    {
                        case "cd..":
                            currentDirectory = Path.GetDirectoryName(currentDirectory.TrimEnd('\\'));
                            Console.WriteLine(currentDirectory);
                            break;

                        case "cd":
                            Console.WriteLine("cd");
                            Console.WriteLine(currentDirectory);

                            string newDirectory = Path.Combine(currentDirectory, name);

                            if (Directory.Exists(newDirectory))
                            {
                                currentDirectory = newDirectory;
                            }
                            else
                            {
                                Console.WriteLine("Directory not found: " + name);
                            }
                            Console.WriteLine(currentDirectory);
                            break;

                        case "mkdir":
                            try
                            {
                                Directory.CreateDirectory($@"{currentDirectory}\{name}\");
                                Console.WriteLine($"Directory '{name}' created successfully.");
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine("Error making a new folder: " + e.ToString());
                            }
                            break;
                        case "touch":
                            Console.WriteLine("touch");

                            try
                            {
                                name = name.Trim();
                                if (!name.EndsWith(".txt"))
                                {
                                    name += ".txt";
                                }

                                VFSManager.CreateFile($@"{currentDirectory}\{name}");
                                Console.WriteLine($"{name} created successfully.");

                            }
                            catch (Exception e)
                            {
                                Console.WriteLine("Creating file error: " + e.ToString());
                            }
                            break;

                        case "clear":
                            Console.Clear();
                            break;

                        case "rm":
                            Console.WriteLine("rm");

                            try
                            {
                                string fullPath = Path.Combine(currentDirectory, name);

                                if (File.Exists(fullPath))
                                {
                                    File.Delete($@"{currentDirectory}\{name}");
                                    Console.WriteLine($"{name} deleted successfully.");
                                }
                                else if (Directory.Exists(fullPath))
                                {
                                    Directory.Delete($@"{currentDirectory}\{name}", true);
                                    Console.WriteLine($"{name} directory deleted successfully.");
                                }
                                else
                                {
                                    Console.WriteLine("Item not found: " + name);
                                }
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine("Error deleting item: " + e.ToString());
                            }
                            break;

                        case "help":
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.WriteLine("Available Commands:");
                            Console.WriteLine("cd [directoryName] - Change current directory");
                            Console.WriteLine("cd.. - Move up one directory");
                            Console.WriteLine("mkdir [directoryName] - Create a new directory");
                            Console.WriteLine("touch [fileName] - Create a new file");
                            Console.WriteLine("rm [itemName] - Delete a file or directory");
                            Console.WriteLine("clear - Clear console");
                            Console.WriteLine("exit - Exit command line mode");
                            Console.WriteLine("help - Display this help message");
                            Console.ForegroundColor = ConsoleColor.Magenta;
                            break;

                        case "exit":
                            OpenFileManager(currentDirectory);
                            return;

                        default:
                            Console.WriteLine("Invalid command. Type 'exit' to leave command line mode or 'help' to view a list of available commands and their uses.");
                            break;
                    }
                }
            }
        }



        private void HandleDirectorySelection(string directoryName)
        {
            Console.WriteLine("Select an option:");
            string[] dirOptions = {
                "  Open folder  ",
                " Delete folder " };
            int dirSelectedIndex = 0;

            while (true)
            {
                Console.BackgroundColor = ConsoleColor.Gray;
                Console.ForegroundColor = ConsoleColor.Black;
                Console.Clear();

                startPrintY = consoleCenterY - (dirOptions.Length / 2);
                for (int i = 0; i < dirOptions.Length; i++)
                {
                    Console.BackgroundColor = ConsoleColor.Gray;
                    Console.ForegroundColor = ConsoleColor.Black;
                    if (i == dirSelectedIndex)
                    {
                        Console.BackgroundColor = ConsoleColor.DarkCyan;
                        Console.ForegroundColor = ConsoleColor.White;
                    }

                    int dirOptionCenterX = consoleCenterX - (dirOptions[i].Length / 2);

                    Console.SetCursorPosition(dirOptionCenterX, startPrintY + i);
                    Console.WriteLine(dirOptions[i]);
                }

                ConsoleKeyInfo key = Console.ReadKey();

                switch (key.Key)
                {
                    case ConsoleKey.UpArrow:
                        dirSelectedIndex = (dirSelectedIndex - 1 + dirOptions.Length) % dirOptions.Length;
                        break;
                    case ConsoleKey.DownArrow:
                        dirSelectedIndex = (dirSelectedIndex + 1) % dirOptions.Length;
                        break;
                    case ConsoleKey.Enter:
                        Console.Clear();
                        switch (dirOptions[dirSelectedIndex].Trim())
                        {
                            case "Open folder":
                                Console.Clear();
                                currentDirectory = ($@"{currentDirectory}\{directoryName}");
                                OpenFileManager(currentDirectory);
                                break;
                            case "Delete folder":
                                DeleteDirectory(directoryName);
                                break;
                            default:
                                Console.WriteLine("Invalid option selected.");
                                break;
                        }
                        Console.WriteLine("Press any key to return to the menu...");
                        while (Console.ReadKey().Key != ConsoleKey.Escape) { }
                        break;
                }
            }
        }


        private void HandleFileSelection(string fileName)
        {
            string[] fileOptions = {
            "     Open     ",
            "     Edit     ",
            "    Delete    " ,
            "     Back     "};
            int fileSelectedIndex = 0;

            startPrintY = consoleCenterY - (fileOptions.Length / 2);

            while (true)
            {
                Console.BackgroundColor = ConsoleColor.Gray;
                Console.ForegroundColor = ConsoleColor.Black;
                Console.Clear();

                for (int i = 0; i < fileOptions.Length; i++)
                {
                    Console.BackgroundColor = ConsoleColor.Gray;
                    Console.ForegroundColor = ConsoleColor.Black;

                    if (i == fileSelectedIndex)
                    {
                        Console.BackgroundColor = ConsoleColor.DarkCyan;
                        Console.ForegroundColor = ConsoleColor.White;
                    }

                    int fileOptionsCenterX = consoleCenterX - (fileOptions[i].Length / 2);
                    Console.SetCursorPosition(fileOptionsCenterX, startPrintY + i);
                    Console.WriteLine(fileOptions[i]);
                }

                ConsoleKeyInfo fileKey = Console.ReadKey();

                switch (fileKey.Key)
                {
                    case ConsoleKey.UpArrow:
                        fileSelectedIndex = (fileSelectedIndex - 1 + fileOptions.Length) % fileOptions.Length;
                        break;
                    case ConsoleKey.DownArrow:
                        fileSelectedIndex = (fileSelectedIndex + 1) % fileOptions.Length;
                        break;
                    case ConsoleKey.Enter:
                        Console.Clear();
                        switch (fileOptions[fileSelectedIndex].Trim())
                        {
                            case "Open":
                                ReadFile(fileName);
                                break;
                            case "Edit":
                                EditFile(fileName);
                                break;
                            case "Delete":
                                DeleteFile(fileName);
                                break;
                            case "Back":
                                OpenFileManager(currentDirectory);
                                break;
                            default:
                                Console.WriteLine("Invalid option selected.");
                                break;
                        }

                        // Print message at the bottom
                        Console.SetCursorPosition(0, Console.WindowHeight - 1);
                        Console.WriteLine("Press escape key to return to the menu...");
                        while (Console.ReadKey().Key != ConsoleKey.Escape) { }
                        break;
                    case ConsoleKey.Escape:
                        // Call OpenFileManager when Escape key is pressed
                        Console.Clear();
                        OpenFileManager(CurrentDirectory);
                        return;
                }
            }
        }


        public void CreateFile(string fileName)
        {
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
        }


        public void ReadFile(string fileName)
        {
            try
            {
                var getFile = VFSManager.GetFile($@"{currentDirectory}\{fileName}");
                var fileStream = getFile.GetFileStream();

                if (fileStream.CanRead)
                {
                    byte[] textToRead = new byte[fileStream.Length];
                    fileStream.Read(textToRead, 0, (int)fileStream.Length);
                    Console.WriteLine("File size: " + getFile.mSize + "kb");
                    Console.WriteLine(Encoding.Default.GetString(textToRead));
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("error reading selected file: " + e.ToString());
            }
        }

        public void EditFile(string fileName)
        {
            try
            {
                var getFile = VFSManager.GetFile($@"{currentDirectory}\{fileName}");
                var fileStream = getFile.GetFileStream();

                if (fileStream.CanRead && fileStream.CanWrite)
                {
                    // Read the existing content of the file
                    byte[] existingContent = new byte[fileStream.Length];
                    fileStream.Read(existingContent, 0, existingContent.Length);

                    // Allow the user to edit the content
                    var newText = EditableConsole.ReadLine(existingContent);

                    // Convert the modified text to a byte array
                    byte[] newTextBytes = Encoding.ASCII.GetBytes(newText);

                    // Clear the existing content
                    fileStream.SetLength(0);

                    // Write the modified content back to the correct position in the file
                    fileStream.Seek(0, SeekOrigin.Begin);
                    fileStream.Write(newTextBytes, 0, newTextBytes.Length);
                }
                else
                {
                    Console.WriteLine("Cannot edit the file. Check if the file is open or read-only.");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Editing file error: " + e.ToString());
            }
        }

        public static class EditableConsole
        {

            public static string ReadLine(byte[] initialContent)
            {
                Console.BackgroundColor = ConsoleColor.DarkCyan;
                Console.ForegroundColor = ConsoleColor.White;
                Console.Clear();

                var editableText = new EditableText(initialContent);
                ConsoleKeyInfo key;
                Console.Write(editableText.GetText());  // Display initial content

                do
                {
                    key = Console.ReadKey(true);

                    switch (key.Key)
                    {
                        case ConsoleKey.Backspace:
                            editableText.RemoveCharacter();
                            break;
                        case ConsoleKey.Escape:
                            Console.WriteLine();  // Move to a new line
                            Console.WriteLine("Editing canceled.");
                            return Encoding.ASCII.GetString(initialContent);
                        default:
                            editableText.AddCharacter(key.KeyChar);
                            break;
                    }

                    Console.SetCursorPosition(0, Console.CursorTop);
                    Console.Write(new string(' ', Console.WindowWidth - 1));
                    Console.SetCursorPosition(0, Console.CursorTop);
                    Console.Write(editableText.GetText());  // Rewrite the updated text

                } while (key.Key != ConsoleKey.Enter);

                Console.WriteLine();  // Move to a new line after Enter key
                return editableText.GetText();
            }
        }

        public class EditableText
        {
            private List<char> text;

            public EditableText(byte[] initialContent)
            {
                text = new List<char>(Encoding.ASCII.GetChars(initialContent));
            }

            public void AddCharacter(char character)
            {
                text.Add(character);
            }

            public void RemoveCharacter()
            {
                if (text.Count > 0)
                {
                    text.RemoveAt(text.Count - 1);
                }
            }

            public string GetText()
            {
                return new string(text.ToArray());
            }
        }

        public void CreateNewDirectory()
        {
            try
            {
                Console.Write("Enter directory name: ");
                var dirName = Console.ReadLine().Trim();
                Directory.CreateDirectory($@"{currentDirectory}\{dirName}\");
            }
            catch (Exception e)
            {
                Console.WriteLine("error making a new folder: " + e.ToString());
            }
        }

        public void DeleteFile(string fileName)
        {
            try
            {
                File.Delete($@"{currentDirectory}\{fileName}");
                Console.Clear();
                int consoleCenterX = Console.WindowWidth / 2;
                int consoleCenterY = Console.WindowHeight / 2;

                string message = $"{fileName} file deleted";
                int messageCenterX = consoleCenterX - (message.Length / 2);
                int startPrintY = consoleCenterY;

                Console.SetCursorPosition(messageCenterX, startPrintY);
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write(message);

                Console.SetCursorPosition(consoleCenterX, consoleCenterY + 2);
                Thread.Sleep(2000);
                OpenFileManager(CurrentDirectory);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error deleting file: " + e.ToString());
            }
        }


        public void DeleteDirectory(string dirName)
        {
            try
            {
                Console.Clear();
                Directory.Delete($@"{currentDirectory}\{dirName}", true);

                string message = $"{dirName} folder deleted";
                int consoleCenterX = Console.WindowWidth / 2;
                int consoleCenterY = Console.WindowHeight / 2;

                int messageCenterX = consoleCenterX - (message.Length / 2);
                int startPrintY = consoleCenterY;

                Console.SetCursorPosition(messageCenterX, startPrintY);

                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(message);
                Console.WriteLine(currentDirectory);
                

                currentDirectory = Path.GetDirectoryName(currentDirectory.TrimEnd('\\'));
                Console.WriteLine(currentDirectory);
                Thread.Sleep(1500);

                Console.Clear();
                OpenFileManager(CurrentDirectory);

            }
            catch (Exception e)
            {
                Console.WriteLine("Error deleting directory: " + e.ToString());
            }
        }


        public void ChangeDirectory()
        {
            try
            {
                Console.Write(currentDirectory + " ");
                var input = Console.ReadLine().Trim();

                if (input == "cd..")
                {
                    // Move up one directory (parent directory)
                    currentDirectory = Path.GetDirectoryName(currentDirectory.TrimEnd('\\'));
                }
                else
                {
                    // Combine the current directory with the user input
                    string newDirectory = Path.Combine(currentDirectory, input);

                    // Check if the new directory exists
                    if (Directory.Exists(newDirectory))
                    {
                        currentDirectory = newDirectory;
                    }
                    else
                    {
                        Console.WriteLine("Directory not found: " + input);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error changing directory: " + e.ToString());
            }
        }
    }
}
