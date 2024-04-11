using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nhtl
{
    internal class EditHandler
    {
        public static void EditFile(string filePath)
        {
            Console.Clear();
            Console.WriteLine($"Редактирование файла: {filePath}\n");

            // Читаем содержимое файла
            string content = File.ReadAllText(filePath);

            // Отображаем содержимое файла
            Console.WriteLine("Содержимое файла:");
            Console.WriteLine(content);

            // Предлагаем пользователю внести изменения
            Console.WriteLine("\nВведите новый текст. Нажмите Ctrl + E для завершения редактирования.");

            string input = content;
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

            // Сохраняем изменения в файле
            File.WriteAllText(filePath, input);

            Console.WriteLine("\nФайл успешно сохранен.");
        }
    }

}
