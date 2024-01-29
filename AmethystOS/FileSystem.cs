using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cosmos.System.FileSystem.VFS;
using Cosmos.System.FileSystem;
using System.IO;

namespace AmethystOS
{
    public class FileSystem
    {
        CosmosVFS fileSystem = new CosmosVFS();
        private string currentDirectory = "0:\\";

        public FileSystem() {
            VFSManager.RegisterVFS(fileSystem);
        }

        public void PrintCurrentDirectory()
        {
            Console.WriteLine(currentDirectory);
        }


        public void GetAvailableFreeSpace()
        {
            float availableFreeSpace = VFSManager.GetAvailableFreeSpace("0:\\");
            Console.WriteLine("Available free space: " + availableFreeSpace/1000000 + "GB");
        }

        public void TotalSize()
        {
            float totalSize = VFSManager.GetTotalSize("0:\\");
            Console.WriteLine("Total size: " + totalSize/1000000 + "GB");
        }
        
        public void ListFiles()
        {
            var directoryList = VFSManager.GetDirectoryListing(currentDirectory);
            foreach (var directoryEntry in directoryList)
            {
                Console.WriteLine(directoryEntry.mName);
            }
        }

        public void CreateFile()
        {
            try
            {
                Console.Write("File name: ");
                var fileName = Console.ReadLine().Trim();
                VFSManager.CreateFile($@"{currentDirectory}\{fileName}");
            }
            catch (Exception e)
            {
                Console.WriteLine("creating file error: " + e.ToString());
            }
        }

        public void ReadFile()
        {
            try
            {
                Console.Write("Type the file name you want to open: ");
                var fileName = Console.ReadLine().Trim();
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
                Console.WriteLine("reading selected file error: " + e.ToString());
            }
        }

        public void EditFile()
        {
            try
            {
                Console.Write("Type the file name you want to edit: ");
                var fileName = Console.ReadLine().Trim();
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
                    Console.Write(new string(' ', Console.WindowWidth - 1));  // Clear the line
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

        public void createNewDirectory()
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

        public void DeleteFile()
        {
            try
            {
                Console.Write("Enter the name of the file you want to delete: ");
                var fileName = Console.ReadLine().Trim();
                File.Delete($@"{currentDirectory}\{fileName}");
                Console.Write($"{fileName} deleted");
            }
            catch (Exception e)
            {
                Console.WriteLine("Error deleting file: " + e.ToString());  
            }
        }

        public void DeleteDirectory()
        {
            try
            {
                Console.Write("Enter the name of the folder you want to delete: ");
                var dirName = Console.ReadLine().Trim();
                Directory.Delete($@"{currentDirectory}\{dirName}", true);
                Console.Write($"{dirName} deleted");
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

                // Handle special case for moving up one directory
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
