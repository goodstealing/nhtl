using System;
using System.Collections.Generic;

namespace nhtl
{
    public class TextBuffer
    {
        // Лист для контента
        private List<string> content;

        // Индекс текущей строки
        private int currentLine;
        // Индекс текущей колонки
        private int currentColumn;

        // Конструктор инициализирует буфер текста пустой строкой и устанавливает начальные индексы
        public TextBuffer()
        {
            content = new List<string> { string.Empty };
            currentLine = 0;
            currentColumn = 0;
        }

        /// Метод обработки ввода текста с сохранением текста в буфер и возможностью загрузки начального содержимого.
        /// <param name="initialContent">Контент который будет инициализирован.</param>
        /// <returns>StartReading(string content).</returns>
        public string StartReading(string initialContent)
        {
            Console.CursorVisible = true;
            Console.WriteLine("Введите текст (Ctrl+E для завершения ввода):");

            // Инициализация буфера с начальным содержимым
            InitializeBuffer(initialContent);

            // Переменная для отслеживания состояния редактирования
            bool isEditing = true;
            // Переменная для хранения пользовательского ввода
            string userInput = initialContent;

            // Цикл обработки ввода пользователя
            while (isEditing)
            {
                // Чтение нажатия клавиши
                ConsoleKeyInfo keyInfo = Console.ReadKey(true);

                // Обработка клавиш
                switch (keyInfo.Key)
                {
                    case ConsoleKey.E:
                        // Обработка Ctrl+E для завершения редактирования
                        if ((keyInfo.Modifiers & ConsoleModifiers.Control) != 0)
                        {
                            isEditing = false;
                            break;
                        }
                        else
                        {
                            // Добавление символа к буферу и пользовательскому вводу
                            AddCharacter(keyInfo.KeyChar);
                            userInput += keyInfo.KeyChar;
                        }
                        break;

                    case ConsoleKey.Backspace:
                        // Обработка нажатия клавиши Backspace
                        HandleBackspace();
                        if (userInput.Length > 0)
                        {
                            // Удаление последнего символа из пользовательского ввода
                            userInput = userInput.Substring(0, userInput.Length - 1);
                        }
                        break;

                    case ConsoleKey.Enter:
                        // Обработка нажатия клавиши Enter
                        AddNewLine();
                        userInput += "\n"; // Добавление символа новой строки к пользовательскому вводу
                        break;

                    case ConsoleKey.LeftArrow:
                        // Обработка нажатия клавиши LeftArrow
                        MoveCursorLeft();
                        break;

                    case ConsoleKey.RightArrow:
                        // Обработка нажатия клавиши RightArrow
                        MoveCursorRight();
                        break;

                    default:
                        // Добавление символа к буферу и пользовательскому вводу
                        AddCharacter(keyInfo.KeyChar);
                        userInput += keyInfo.KeyChar;
                        break;
                }
            }

            // Возврат введённого текста
            Console.CursorVisible = false;
            return userInput;
        }

        /// Метод инициализирует буфер строк начальным содержимым
        /// <param name="initialContent">Контент который будет инициализирован.</param>
        public void InitializeBuffer(string initialContent)
        {
            // Разбитие initialContent на строки и сохранение в буфер
            content = new List<string>(initialContent.Split('\n'));
            // Текущая строка и колонка = последняя строка и её длина
            currentLine = content.Count - 1;
            currentColumn = content[currentLine].Length;
            Console.Write(initialContent);
        }

        /// Метод добавляет символ в текущую позицию буфера
        /// <param name="c">Символ для обработки ввода.</param>
        private void AddCharacter(char c)
        {
            // Вставка символа в текущую строку в позиции текущей колонки
            content[currentLine] = content[currentLine].Insert(currentColumn, c.ToString());
            // Увеличение индекса текущей колонки
            currentColumn++;
            // Вывод символа
            Console.Write(c);
        }

        /// Обрабатывает удаление символов, переход на новую строку.
        private void HandleBackspace()
        {
            // Если не в начале строки, удаление символа слева от текущей Column
            if (currentColumn > 0)
            {
                content[currentLine] = content[currentLine].Remove(currentColumn - 1, 1);
                currentColumn--;
                // Удаление символа в консоли
                Console.Write("\b \b");
            }
            else if (currentLine > 0)
            {
                // Переход на предыдущую строку и объединение её с текущей
                currentColumn = content[currentLine - 1].Length;
                Console.CursorLeft = 0;
                Console.CursorTop--;
                Console.Write(content[currentLine - 1]);
                content[currentLine - 1] += content[currentLine];
                content.RemoveAt(currentLine);
                currentLine--;
            }
        }

        /// Добавление новой строки в буфер
        public void AddNewLine()
        {
            // Вставка пустой строки после текущей строки
            content.Insert(currentLine + 1, string.Empty);
            // Переход к новой строке и  currentColumn = 0 
            currentLine++;
            currentColumn = 0;
            // Вывод новой строки в консоль
            Console.WriteLine();
        }

        /// Перемещение курсора влево
        private void MoveCursorLeft()
        {
            if (currentColumn > 0)
            {
                currentColumn--;
                Console.SetCursorPosition(Console.CursorLeft - 1, Console.CursorTop);
            }
            else if (currentLine > 0)
            {
                currentLine--;
                currentColumn = content[currentLine].Length;
                Console.SetCursorPosition(content[currentLine].Length, Console.CursorTop - 1);
            }
        }

        /// Перемещение курсора вправо
        private void MoveCursorRight()
        {
            if (currentColumn < content[currentLine].Length)
            {
                currentColumn++;
                Console.SetCursorPosition(Console.CursorLeft + 1, Console.CursorTop);
            }
            else if (currentLine < content.Count - 1)
            {
                currentLine++;
                currentColumn = 0;
                Console.SetCursorPosition(0, Console.CursorTop + 1);
            }
        }

        /// Возвращает контента буфера.
        /// <returns>Содержимое буфера в виде одной строки.</returns>
        public string GetBufferContent()
        {
            // Объединение строк буфера в одну строку с разделителем "\n"
            return string.Join("\n", content);
        }
    }
}
