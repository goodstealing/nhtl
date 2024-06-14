using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spectre.Console;


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
            if (string.IsNullOrEmpty(fileName))
            {
                Console.WriteLine("Имя файла не может быть пустым.");
                Program.ShowMainMenu();
            }

            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
            string fileExtension = Path.GetExtension(fileName);

            AnsiConsole.MarkupInterpolated($"Имя файла -> [green]{fileNameWithoutExtension}[/][red]{fileExtension}[/] <- Расширение файла\n");

            TextBuffer textBuffer = new();
            string inputText = textBuffer.StartReading();
            //Console.WriteLine(); // Переход на новую строку после нажатия Ctrl+E

            string filePath;
            do
            {
                Console.Clear();
                AnsiConsole.MarkupInterpolated($"Имя файла -> [green]{fileNameWithoutExtension}[/][red]{fileExtension}[/] <- Расширение файла\n");
                
                Console.Write("Проверьте правильность пути перед сохранением файла. ");
                Console.Write("Введите путь для сохранения файла: ");
                filePath = Console.ReadLine();
            }
            while (!Directory.Exists(filePath)); // Проверка существования папки
            string fullFilePath = Path.Combine(filePath, fileName);

            try
            {
                using (StreamWriter writer = File.CreateText(fullFilePath))
                {
                    writer.WriteLine(inputText);
                }
                Console.Clear();
                AnsiConsole.MarkupInterpolated($"Файл [green]{fileName}[/] успешно сохранён по пути: [red]{fullFilePath}[/] \n");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при создании файла: {ex.Message}");
            }
            Program.ShowMainMenu();
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
