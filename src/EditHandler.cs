using Spectre.Console;
using System;
using System.IO;

namespace nhtl
{
    internal class EditHandler
    {
        // Путь к файлу, который будет открыт
        public static string? filePath;
        // Имя файла без расширения
        public static string? fileNameWithoutExtension;
        // Расширение файла
        public static string? fileExtension;

        public static void EditFile(string filePath)
        {
            // Получение имени файла без расширения и расширения файла
            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(filePath);
            string fileExtension = Path.GetExtension(filePath);

            // Очистка консоли и вывод информации о редактируемом файле
            Console.Clear();
            AnsiConsole.MarkupInterpolated($"Редактирование файла -> [green]{fileNameWithoutExtension}[/][red]{fileExtension}[/] <- Расширение файла\n");

            // Чтение содержимого файла в массив строк
            string[] content = File.ReadAllLines(filePath);

            // Отображение содержимого файла
            Console.WriteLine("Содержимое файла:");
            OpenHandler.PreviewFileInfo(filePath, content, 15);
            Console.CursorVisible = true;

            // Создание экземпляра TextBuffer для редактирования текста
            TextBuffer textBuffer = new();

            // Инициализация TextBuffer с начальным содержимым файла
            textBuffer.StartReading(string.Join("\n", content));
            // Получение отредактированного содержимого из TextBuffer
            string inputText = textBuffer.GetBufferContent();

            // Запись изменений в файл
            File.WriteAllText(filePath, inputText);
            Console.Clear();

            Console.WriteLine($"Файл {fileNameWithoutExtension}{fileExtension} успешно сохранён.");
            OpenHandler.ShowEditMenu(filePath);
        }


        // Очистка консоли перед выходом в главное меню.
        public static void Exit()
        {
            Console.Clear();
            Program.ShowMainMenu();
        }
    }
}
