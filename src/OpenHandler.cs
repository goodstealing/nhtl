using Spectre.Console;
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
        public static string fileName;
        public static string fileNameWithoutExtension;
        public static string fileExtension;

        public static void OpenFile()
        {
            Console.Clear();
            Console.CursorVisible = true;

            fileName = GetFileName();
            fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
            fileExtension = Path.GetExtension(fileName);

            if (!File.Exists(fileName))
            {
                Console.Clear();
                AnsiConsole.MarkupInterpolated($"Файл [green]{fileName}[/] не найден.");
                Program.ShowMainMenu();
                return;
            }

            string[] lines = File.ReadAllLines(fileName);

            PreviewFileInfo(fileName, lines, 15);
            ShowEditMenu(fileName, lines);
        }

        public static string GetFileName()
        {
            Console.Write("Введите имя файла/путь для открытия: ");
            string? fileName = Console.ReadLine();

            return Path.IsPathRooted(fileName)
                ? fileName
                : Path.Combine(Directory.GetCurrentDirectory(), fileName);
        }

        public static string GetFileExtension(string fileName)
        {
            return Path.GetFileNameWithoutExtension(fileName);
        }

        static void PreviewFileInfo(string fileName, string[] lines, int count)
        {
            Console.Clear();
            Console.WriteLine($"Имя файла -> {Path.GetFileNameWithoutExtension(fileName)}{Path.GetExtension(fileName)} < - Расширение файла");

            Console.WriteLine($"Предпросмотр файла: {Path.GetFileNameWithoutExtension(fileName)} ");
            Console.WriteLine("══════════════════════════════════");
            for (int i = 0; i < Math.Min(lines.Length, count); i++)
            {
                Console.WriteLine($"{i + 1}: {lines[i]}");
            }
            Console.WriteLine("══════════════════════════════════\n");
        }

        public static void ShowEditMenu(string fileName, string[] lines)
        {
            Dictionary<ConsoleKey, Action> keyActions = new()
            {
                { ConsoleKey.E, () => EditHandler.EditFile(fileName) },
                { ConsoleKey.R, () => DeleteFile(fileName) },
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
                    if (keyActions.TryGetValue(keyInfo.Key, out var action))
                    {
                        action();
                        break;
                    }
                }
            }
        }

        public static void DeleteFile(string fileName)
        {
            Console.Clear();
            try
            {
                if (File.Exists(fileName))
                {
                    File.Delete(fileName);
                    AnsiConsole.MarkupInterpolated($"Файл [green]{fileNameWithoutExtension}[/] успешно удалён по пути: [red]{fileName}[/].\n");
                }
                else
                {
                    AnsiConsole.MarkupInterpolated($"Файл [green]{fileNameWithoutExtension}[/] по пути: [red]{fileName}[/] не найден.\n");
                }
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupInterpolated($"Произошла ошибка при удалении файла [green]{fileNameWithoutExtension}[/]: {ex.Message}.\n");
            }
            Program.ShowMainMenu();
        }

        public static string asciiArt = @"
███╗   ██╗██╗  ██╗████████╗██╗     
████╗  ██║██║  ██║╚══██╔══╝██║     
██╔██╗ ██║███████║   ██║   ██║     
██║╚██╗██║██╔══██║   ██║   ██║     
██║ ╚████║██║  ██║   ██║   ███████╗
╚═╝  ╚═══╝╚═╝  ╚═╝   ╚══════╝";
    }
}
