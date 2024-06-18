using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Spectre.Console;


namespace nhtl
{
    internal class CreateHandler
    {
        // Имя файла без расширения
        public static string? fileNameWithoutExtension;
        // Расширение файла
        public static string? fileExtension;

        /// <summary>
        /// Асинхронно создает новый файл с заданным именем и содержимым.
        /// </summary>
        public static async Task CreateFile()
        {
            // Очистка консоли и отображение курсора
            Console.Clear();
            Console.CursorVisible = true;

            string? fileName;
            // Регулярное выражение для проверки формата имени файла
            Regex fileNameRegex = new(@"^[\w\-. ]+\.[A-Za-z]{2,4}$");

            // Запрос имени файла у пользователя с проверкой корректности
            do
            {
                Console.Write("Введите имя нового файла (пример: file.txt): ");
                fileName = Console.ReadLine()?.Trim();

                if (string.IsNullOrEmpty(fileName))
                {
                    // Вывод сообщения о пустом имени файла
                    Console.Clear();
                    AnsiConsole.MarkupInterpolated($"LOG: [yellow]Имя файла не может быть пустым. Попробуйте снова.[/]\n");
                }
                else if (!fileNameRegex.IsMatch(fileName))
                {
                    // Вывод сообщения о некорректном формате имени файла
                    Console.Clear();
                    AnsiConsole.MarkupInterpolated($"LOG: [yellow]Неверный формат имени файла. Попробуйте снова.[/]\n");
                    fileName = null;
                }
            } while (string.IsNullOrEmpty(fileName));

            Console.Clear();
            // Получение имени файла без расширения и расширения файла
            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
            string fileExtension = Path.GetExtension(fileName);

            AnsiConsole.MarkupInterpolated($"Имя файла -> [green]{fileNameWithoutExtension}[/][red]{fileExtension}[/] <- Расширение файла\n");
            TextBuffer textBuffer = new();

            // Запуск обработки текста для нового файла
            string inputText = textBuffer.StartReading(string.Empty);
            string? filePath;

            // Запрос пути для сохранения файла у пользователя с проверкой существования директории
            do
            {
                Console.Clear();
                AnsiConsole.MarkupInterpolated($"Имя файла -> [green]{fileNameWithoutExtension}[/][red]{fileExtension}[/] <- Расширение файла\n");

                Console.Write("Введите путь для сохранения файла: ");
                Console.CursorVisible = true;
                filePath = Console.ReadLine()?.Trim();

                if (!Directory.Exists(filePath))
                {
                    Console.Clear();
                    AnsiConsole.MarkupInterpolated($"LOG: [red]Указанный путь не существует. Попробуйте снова.[/]\n");
                    await OpenHandler.PlayErrorSoundAsync("sound/bib.wav");
                }
            } while (!Directory.Exists(filePath));

            // Формирование полного пути к файлу
            string fullFilePath = Path.Combine(filePath, fileName);

            try
            {
                // Запись текста в новый файл
                using (StreamWriter writer = new(filePath))
                {
                    await writer.WriteLineAsync(inputText);
                }
                // Успешное сохранение файла
                Console.Clear();
                AnsiConsole.MarkupInterpolated($"Файл [green]{fileName}[/] успешно сохранён по пути: [red]{fullFilePath}[/] \n");
            }
            catch (UnauthorizedAccessException ex)
            {
                // Обработка ошибки доступа при создании файла
                await HandleExceptionAsync($"Ошибка доступа при создании файла: {ex.Message}", "sound/bib.wav");
            }
            catch (DirectoryNotFoundException ex)
            {
                // Обработка ошибки, когда директория не найдена
                await HandleExceptionAsync($"Директория не найдена: {ex.Message}", "sound/pip.wav");
            }
            catch (IOException ex)
            {
                // Обработка ошибок ввода-вывода
                await HandleExceptionAsync($"Ошибка ввода-вывода: {ex.Message}", "sound/bib.wav");
            }
            catch (Exception ex)
            {
                // Обработка непредвиденных ошибок
                await HandleExceptionAsync($"Непредвиденная ошибка: {ex.Message}", "sound/bib.wav");
            }
            finally
            {
                Program.ShowMainMenu();
            }
        }

        
        /// Асинхронно обрабатывает исключения, выводит сообщение об ошибке и воспроизводит звук ошибки.
        /// <param name="message">Сообщение об ошибке</param>
        /// <param name="soundPath">Путь к звуковому файлу</param>
        private static async Task HandleExceptionAsync(string message, string soundPath)
        {
            // Очистка консоли и вывод сообщения об ошибке
            Console.Clear();
            AnsiConsole.MarkupInterpolated($"LOG: [red]{message}[/]\n");
            // Воспроизведение звука ошибки
            await OpenHandler.PlayErrorSoundAsync(soundPath);
        }



        static void SaveFile(string fileName, string[] lines)
        {
            File.WriteAllLines(fileName, lines);
            Console.WriteLine($"Файл '{fileName}' сохранен.");
        }
    }
}
