using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nhtl
{
    internal class CreateHandler
    {
        public static void CreateFile()
        {
            Console.Clear();
            Console.CursorVisible = true;

            Console.Write("Введите имя нового файла: ");
            string fileName = Console.ReadLine();

            Console.Clear();
            Console.Write($"|Имя файла -> {fileName} <- Расширение файла|");
            Console.WriteLine("\nВведите текст для записи в файл (для завершения ввода нажмите Ctrl+E):");
            string inputText = ReadMultilineInput();

            Console.WriteLine(); // Переход на новую строку после нажатия Ctrl+E

            string filePath;
            do
            {
                Console.Clear();
                Console.Write($"|Имя файла -> {fileName} <- Расширение файла|");
                Console.Write("\nВведите путь для сохранения файла: ");
                filePath = Console.ReadLine();
            }
            while (!Directory.Exists(filePath)); // Проверяем существование папки

            string fullFilePath = Path.Combine(filePath, fileName);

            try
            {
                using (StreamWriter writer = File.CreateText(fullFilePath))
                {
                    writer.WriteLine(inputText);
                }
                Console.Clear();
                Console.WriteLine($"Файл '{fileName}' успешно сохранён по пути: {fullFilePath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при создании файла: {ex.Message}");
            }
            Program.ShowMainMenu();
        }

    private static string ReadMultilineInput()
    {
        string input = string.Empty;
        bool isEditing = true;

        while (isEditing)
        {
            ConsoleKeyInfo keyInfo = Console.ReadKey(true);

            // Обработка ввода
            switch (keyInfo.Key)
            {
                case ConsoleKey.E: // Ctrl+E - выход
                    if ((keyInfo.Modifiers & ConsoleModifiers.Control) != 0)
                    {
                        isEditing = false;
                        break;
                    }
                    else
                    {
                        // Добавляем символ в текст
                        input += keyInfo.KeyChar;
                    }
                break;

                case ConsoleKey.Backspace:
                    if (input.Length > 0)
                    {
                        // Удаляем последний символ из текста
                        input = input.Substring(0, input.Length - 1);
                        Console.Write("\b \b"); // Удаляем символ с консоли
                    }
                break;

                case ConsoleKey.Enter: // Enter - перейти на новую строку
                    input += "\n";
                    Console.WriteLine(); // Переход на новую строку в консоли
                    break;

                default: // Добавить введенный текст
                    input += keyInfo.KeyChar;
                    Console.Write(keyInfo.KeyChar); // Отображаем введенный символ в консоли
                    break;
            }
        }

        return input;
    }



        public static void EditFile(string fileName, string[] lines)
        {
            Console.WriteLine("╔════════════════════════════╗");
            Console.WriteLine($"║ Редактирование нового файла: {fileName} ");
            Console.WriteLine("╚════════════════════════════╝");

            Console.WriteLine("Введите новый текст. Наберите 'exit' для завершения редактирования.");

            var editedLines = new List<string>();

            foreach (var line in lines)
            {
                Console.Write("> ");
                var input = Console.ReadLine();
                Console.WriteLine(input);

                // Если пользователь ввел "exit", завершаем редактирование
                if (input == "exit")
                    break;

                editedLines.Add(input);
            }

            // Сохраняем изменения
            SaveFile(fileName, editedLines.ToArray());
        }

        static void SaveFile(string? fileName, string[] lines)
        {
            File.WriteAllLines(fileName, lines);
            Console.WriteLine($"Файл '{fileName}' сохранен.");
        }
    }
}
