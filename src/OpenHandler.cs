using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nhtl
{
    internal class OpenHandler
    {
        public static void OpenFile()
        {
            Console.Clear();
            Console.CursorVisible = true;

            string fileName = GetFileName();

            if (!File.Exists(fileName))
            {
                Console.Clear();
                Console.WriteLine($"Файл '{fileName}' не найден.");
                Program.ShowMainMenu();
                return;
            }

            string[] lines = File.ReadAllLines(fileName);

            PreviewFileInfo(fileName, lines, 15);
            ShowEditMenu(fileName, lines);
        }

        static string GetFileName()
        {
            Console.Write("Введите имя файла/путь для открытия: ");
            string ?fileName = Console.ReadLine();

            return Path.IsPathRooted(fileName)
                ?fileName
                :Path.Combine(Directory.GetCurrentDirectory(), fileName);
        }

        static void PreviewFileInfo(string fileName, string[] lines, int count)
        {
            Console.Clear();
            Console.WriteLine($"Имя файла -> {fileName} <- Расширение файла");

            Console.WriteLine($"Предпросмотр файла: {Path.GetFileName(fileName)} ");
            Console.WriteLine("══════════════════════════════════");
            for (int i = 0; i < Math.Min(lines.Length, count); i++)
            {
                Console.WriteLine($"{i + 1}: {lines[i]}");
            }
            Console.WriteLine("══════════════════════════════════\n");
        }

        static void ShowEditMenu(string fileName, string[] lines)
        {
            Dictionary<ConsoleKey, Action> keyActions = new()
            {
                { ConsoleKey.E, () => EditHandler.EditFile(fileName) },
                { ConsoleKey.R, CreateHandler.CreateFile },
                { ConsoleKey.Z, Program.ShowMainMenu },
            };

            Console.CursorVisible = false;
            Console.WriteLine("┌───────────────────────────────┐");
            Console.WriteLine("| Ctrl + E | Редактировать файл |");
            Console.WriteLine("├───────────────────────────────┤");
            Console.WriteLine("| Ctrl + R | Удалить файл       |");
            Console.WriteLine("├───────────────────────────────┤");
            Console.WriteLine("| Ctrl + Z | Выход              |");
            Console.WriteLine("└───────────────────────────────┘");

            while (true)
            {
                ConsoleKeyInfo keyInfo = Console.ReadKey(true);

                if ((keyInfo.Modifiers & ConsoleModifiers.Control) != 0)
                {
                    switch (keyInfo.Key)
                    {
                        case ConsoleKey.E:
                            Console.WriteLine("Редактирование файла");
                            EditHandler.EditFile(fileName);
                            break;
                        case ConsoleKey.R:
                            Console.WriteLine("Удаление файла");
                            break;
                        case ConsoleKey.Z:
                            Console.Clear();
                            Program.ShowMainMenu();
                            break;
                        default:
                            break; // Обработка неизвестных клавиш Ctrl
                    }
                }
            }
        }

        static void EditFile(string fileName, string[] lines)
        {
            // ...
        }

        static void SaveFile(string fileName, string[] lines)
        {
            // ...
        }

        public static string asciiArt = @"
███╗   ██╗██╗  ██╗████████╗██╗     
████╗  ██║██║  ██║╚══██╔══╝██║     
██╔██╗ ██║███████║   ██║   ██║     
██║╚██╗██║██╔══██║   ██║   ██║     
██║ ╚████║██║  ██║   ██║   ███████╗
╚═╝  ╚═══╝╚═╝  ╚═╝   ╚═╝   ╚══════╝
                                                                  
                                                                   



";
    }
}
