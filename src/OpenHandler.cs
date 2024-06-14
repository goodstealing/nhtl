using Spectre.Console;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NAudio.Wave;


namespace nhtl
{
    internal class OpenHandler
    {
        public static string ?fileName;
        public static string ?fileNameWithoutExtension;
        public static string ?fileExtension;
        public const string? asciiArt = "\r\n███╗   ██╗██╗  ██╗████████╗██╗     \r\n████╗  ██║██║  ██║╚══██╔══╝██║     \r\n██╔██╗ ██║███████║   ██║   ██║     \r\n██║╚██╗██║██╔══██║   ██║   ██║     \r\n██║ ╚████║██║  ██║   ██║   ███████╗\r\n╚═╝  ╚═══╝╚═╝  ╚═╝   ╚══════╝";

        public async static void OpenFile()
        {
            Console.Clear();
            Console.CursorVisible = true;

            fileName = await GetFileNameAsync();
            fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
            fileExtension = Path.GetExtension(fileName);

            if (!File.Exists(fileName))
            {
                Console.Clear();
                AnsiConsole.MarkupInterpolated($"Файл [green]{fileName}[/] не найден.\n");
                Program.ShowMainMenu();
                return;
            }

            string[] lines = File.ReadAllLines(fileName);

            PreviewFileInfo(fileName, lines, 15);
            ShowEditMenu(fileName, lines);
        }

        public static async Task<string> GetFileNameAsync()
        {
            string? fileName;

            do
            {
                Console.Write("Введите имя файла/путь для открытия: ");
                fileName = Console.ReadLine();

                if (string.IsNullOrEmpty(fileName))
                {
                    Console.Clear();
                    AnsiConsole.MarkupInterpolated($"LOG: [red]Ошибка! Имя файла не может быть null или пустым.[/] \n");
                    await PlayErrorSoundAsync("bib.wav"); // Воспроизведение звука ошибки
                    AnsiConsole.MarkupInterpolated($"LOG: Пожалуйста, введите имя файла заново.\n");
                    await PlayErrorSoundAsync("bib.wav"); // Воспроизведение звука ошибки
                }
            }
            while (string.IsNullOrEmpty(fileName));

            return Path.IsPathRooted(fileName)
                ? fileName
                : Path.Combine(Directory.GetCurrentDirectory(), fileName);
        }

        public static async Task PlayErrorSoundAsync(string filePath)
        {
            if (File.Exists(filePath))
            {
                await Task.Run(() =>
                {
                    using (var audioFile = new AudioFileReader(filePath))
                    using (var outputDevice = new WaveOutEvent())
                    {
                        outputDevice.Init(audioFile);
                        outputDevice.Play();
                        while (outputDevice.PlaybackState == PlaybackState.Playing)
                        {
                            System.Threading.Thread.Sleep(100);
                        }
                    }
                });
            }
            else
            {
                Console.WriteLine($"Файл звука не найден: {filePath}");
            }
        }

        public static string GetFileExtension(string? fileName) => Path.GetFileNameWithoutExtension(fileName)!;

        static void PreviewFileInfo(string fileName, string[] lines, int count)
        {
            Console.Clear();
            AnsiConsole.MarkupInterpolated($"Предпросмотр файла -> [green]{fileNameWithoutExtension}[/][red]{fileExtension}[/] <- Расширение файла\n");
            Console.WriteLine("══════════════════════════════════");
            for (int i = 0; i < Math.Min(lines.Length, count); i++)
            {
                Console.WriteLine($"{i + 1}: {lines[i]}");
            }
            Console.WriteLine("══════════════════════════════════\n");
        }

        public static void ShowEditMenu(string fileName, string[] lines)
        {
            ArgumentNullException.ThrowIfNull(lines);

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
                    AnsiConsole.MarkupInterpolated($"LOG: Файл [green]{fileNameWithoutExtension}[/] успешно удалён по пути: [red]{fileName}[/].\n");
                }
                else
                {
                    AnsiConsole.MarkupInterpolated($"LOG: Файл [green]{fileNameWithoutExtension}[/] по пути: [red]{fileName}[/] не найден.\n");
                }
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupInterpolated($"LOG: [red]Произошла ошибка при удалении файла[/] [green]{fileNameWithoutExtension}[/]: [red]{ex.Message}[/].\n");
            }
            Program.ShowMainMenu();
        }
    }
}
