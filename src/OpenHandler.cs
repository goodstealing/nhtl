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
        // Путь к файлу, который будет открыт
        public static string? filePath;
        // Имя файла без расширения
        public static string? fileNameWithoutExtension;
        // Расширение файла
        public static string? fileExtension;
        // Константа, содержащая ASCII-арт
        public const string? asciiArt = "\r\n███╗   ██╗██╗  ██╗████████╗██╗     \r\n████╗  ██║██║  ██║╚══██╔══╝██║     \r\n██╔██╗ ██║███████║   ██║   ██║     \r\n██║╚██╗██║██╔══██║   ██║   ██║     \r\n██║ ╚████║██║  ██║   ██║   ███████╗\r\n╚═╝  ╚═══╝╚═╝  ╚═╝   ╚══════╝";


        /// Асинхронный метод для открытия файла.
        /// Выполняет очистку консоли, отображение курсора и загрузку информации о файле.
        public async static void OpenFile()
        {
            // Очистка консоли для нового ввода
            Console.Clear();
            // Включение видимости курсора в консоли
            Console.CursorVisible = true;

            // Ожидание пути к файлу от пользователя
            filePath = await GetFileNameAsync();

            // Получение имени файла без расширения и расширения файла
            fileNameWithoutExtension = Path.GetFileNameWithoutExtension(filePath);
            fileExtension = Path.GetExtension(filePath);
                
            // Проверка существования файла по указанному пути
            if (!File.Exists(filePath))
            {
                // Очистка консоли при отсутствии файла
                Console.Clear();
                // Отображение сообщения о том, что файл не найден
                AnsiConsole.MarkupInterpolated($"Файл [green]{filePath}[/] не найден.\n");
                // Возврат в главное меню программы
                Program.ShowMainMenu();
                return;
            }

            // Чтение всех строк из файла
            string[] content = File.ReadAllLines(filePath);

            // Предварительный просмотр информации о файле (ограничен 15 строками)
            PreviewFileInfo(filePath, content, 15);
            // Показать меню редактирования для текущего файла
            ShowEditMenu(filePath);
        }



        /// Асинхронно запрашивает у пользователя имя файла или путь к файлу, проверяет его наличие
        /// и возвращает полное имя файла, если файл существует.
        /// <returns>Полное имя файла (включая путь).</returns>
        public static async Task<string> GetFileNameAsync()
        {
            string? fileName;

            do
            {
                Console.Write("Введите имя файла/путь для открытия: ");
                fileName = Console.ReadLine();

                // Проверка на Empty или null
                if (string.IsNullOrEmpty(fileName))
                {
                    Console.Clear();
                    AnsiConsole.MarkupInterpolated($"LOG: [red]Ошибка! Имя файла не может быть null или пустым.[/] \n");
                    await PlayErrorSoundAsync("sound/bib.wav");
                    AnsiConsole.MarkupInterpolated($"LOG: Пожалуйста, введите имя файла заново.\n");
                    await PlayErrorSoundAsync("sound/bib.wav");
                }
                // Проверка существования файла
                else if (!File.Exists(fileName))
                {
                    Console.Clear();
                    AnsiConsole.MarkupInterpolated($"LOG: [red]Ошибка! Файл не существует.[/] \n");
                    await PlayErrorSoundAsync("sound/bib.wav");
                    AnsiConsole.MarkupInterpolated($"LOG: Пожалуйста, введите имя файла заново.\n");
                    await PlayErrorSoundAsync("sound/bib.wav");
                }
                else
                {
                    // Открытие файла для чтения
                    try
                    {
                        using var fileStream = File.OpenRead(fileName);
                        // Успешное открытие файла
                    }
                    catch (Exception ex)
                    {
                        Console.Clear();
                        AnsiConsole.MarkupInterpolated($"LOG: [red]Ошибка при открытии файла: {ex.Message}[/] \n");
                        await PlayErrorSoundAsync("sound/bib.wav");

                        // Сброс имени файла для повторной попытки ввода
                        fileName = null;
                    }
                }
            }
            while (string.IsNullOrEmpty(fileName) || !File.Exists(fileName));

            // Return полное имя файла, если путь относительный -> приводим к абсолютному
            return Path.IsPathRooted(fileName)
                ? fileName
                : Path.Combine(Directory.GetCurrentDirectory(), fileName);
        }


        /// Асинхронно воспроизводит аудиофайлу, если он существует по указанному пути.
        /// <param name="filePath">Путь к аудиофайлу.</param>
        public static async Task PlayErrorSoundAsync(string filePath)
        {
            // Проверка существования файла по указанному пути
            if (File.Exists(filePath))
            {
                await Task.Run(() =>
                {
                    // Использование AudioFileReader для чтения аудиофайла
                    // Использование WaveOutEvent для воспроизведения аудио
                    using var audioFile = new AudioFileReader(filePath);
                    using var outputDevice = new WaveOutEvent();

                    // Инициализация устройства вывода аудиофайлом
                    outputDevice.Init(audioFile);
                    // Запуск воспроизведения аудиофайла
                    outputDevice.Play();
                    // Ожидание завершения воспроизведения
                    outputDevice.PlaybackStopped += (sender, args) =>
                    {
                        outputDevice.Dispose(); // Освобождение ресурсов после завершения воспроизведения
                    };

                    while (outputDevice.PlaybackState == PlaybackState.Playing)
                    {
                        // Пауза на 100 миллисекунд перед повторной проверкой состояния воспроизведения
                        System.Threading.Thread.Sleep(100);
                    }
                });
            }
            else
            {
                // Логирование в консоль без блокировки основного потока
                await Task.Run(() =>
                {
                    AnsiConsole.MarkupInterpolated($"LOG: [yellow]Файл звука[/] {filePath} [yellow]не найден[/].\n");
                });
            }
        }


        /// Получает расширение файла из его имени.
        /// <param name="fileName">Имя файла.</param>
        /// <returns>Расширение файла без точки или пустая строка, если расширения нет.</returns>
        public static string GetFileExtension(string? fileName) => Path.GetFileNameWithoutExtension(fileName)!;


        /// Отображает предпросмотр содержимого файла в консоли.
        /// <param name="filePath">Путь до файла для предпросмотра.</param>
        /// <param name="content">Массив строк, представляющих содержимое файла.</param>
        /// <param name="count">Количество строк, отображаемых из файла.</param>
        public static void PreviewFileInfo(string filePath, string[] content, int count)
        {
            // Очистка консоли
            Console.Clear();

            // Отображение имени файла и расширение
            AnsiConsole.MarkupInterpolated($"Предпросмотр файла -> [green]{fileNameWithoutExtension}[/][red]{fileExtension}[/] <- Расширение файла\n");

            // Разделительная линия
            Console.WriteLine("══════════════════════════════════");
            // Итерация по строкам и отображение строк до указанного значения count
            for (int i = 0; i < Math.Min(content.Length, count); i++)
            {
                Console.WriteLine($"{i + 1}: {content[i]}");
            }
            Console.WriteLine("══════════════════════════════════");
        }


        /// Отображает меню редактирования с опциями редактирования, удаления файла и выхода в главное меню.
        /// <param name="filePath">Путь до файла для предпросмотра.</param>
        public static void ShowEditMenu(string filePath)
        {
            // Создание словаря для сопоставления горячих клавиш с методами
            Dictionary<ConsoleKey, Action> keyActions = new()
            {
                { ConsoleKey.E, () => EditHandler.EditFile(filePath) }, // Редактирование файла
                { ConsoleKey.R, () => DeleteFile(filePath) },           // Удаление файла
                { ConsoleKey.Z, EditHandler.Exit},                      // Выход в главное меню
            };
            Console.CursorVisible = false;

            // Отображение меню опций
            Console.WriteLine("┌───────────────────────────────┐");
            Console.WriteLine("| Ctrl + E | Редактировать файл |");
            Console.WriteLine("├───────────────────────────────┤");
            Console.WriteLine("| Ctrl + R | Удалить файл       |");
            Console.WriteLine("├───────────────────────────────┤");
            Console.WriteLine("| Ctrl + Z | Выход              |");
            Console.WriteLine("└───────────────────────────────┘");

            // Постоянная обработка клавиш для срабатывания триггера
            while (true)
            {
                // Обработка нажатия всех клавиш (без отображения)
                ConsoleKeyInfo keyInfo = Console.ReadKey(true);

                // Проверка нажатия модификатора Control
                if ((keyInfo.Modifiers & ConsoleModifiers.Control) != 0)
                {
                    // Если нажатая клавиша сопоставлена ​​с действием, выполните действие и выйдите из цикла
                    if (keyActions.TryGetValue(keyInfo.Key, out var action))
                    {
                        action();
                        break;
                    }
                }
            }
        }


        /// Асинхронно удаляет указанный файл, если он существует, и логирует результат.
        /// <param name="filePath">Полный путь к файлу, который нужно удалить.</param>
        public async static void DeleteFile(string filePath)
        {
            // Очистка консоли
            Console.Clear();

            try
            {
                // Проверка существования файла по указанному пути
                if (File.Exists(filePath))
                {
                    // Удаление файла при условии что он существует
                    File.Delete(filePath);
                    AnsiConsole.MarkupInterpolated($"LOG: Файл [green]{fileNameWithoutExtension}[/] успешно удалён по пути: [red]{filePath}[/].\n");

                    // Возврат в главное меню
                    Program.ShowMainMenu();
                }
                else
                {
                    // LOG: Файл не найден
                    AnsiConsole.MarkupInterpolated($"LOG: Файл [green]{fileNameWithoutExtension}[/] по пути: [red]{filePath}[/] не найден.\n");
                    await PlayErrorSoundAsync("sound/bib.wav");

                    // Возврат в главное меню
                    Program.ShowMainMenu();
                }
            }
            catch (Exception ex)
            {
                // LOG: Сообщение об ошибке при попытке удаления файла
                AnsiConsole.MarkupInterpolated($"LOG: [red]Произошла ошибка при удалении файла[/] [green]{fileNameWithoutExtension}[/]: [red]{ex.Message}[/].\n");
                await PlayErrorSoundAsync("sound/bib.wav");

                // Возврат в главное меню
                Program.ShowMainMenu();
            }
        }
    }
}
