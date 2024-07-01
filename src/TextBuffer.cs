using System;
using System.Collections.Generic;

namespace nhtl
{
    public class TextBuffer
    {
        // ���� ��� ��������
        private List<string> content;

        // ������ ������� ������
        private int currentLine;
        // ������ ������� �������
        private int currentColumn;

        // ����������� �������������� ����� ������ ������ ������� � ������������� ��������� �������
        public TextBuffer()
        {
            content = new List<string> { string.Empty };
            currentLine = 0;
            currentColumn = 0;
        }

        /// ����� ��������� ����� ������ � ����������� ������ � ����� � ������������ �������� ���������� �����������.
        /// <param name="initialContent">������� ������� ����� ���������������.</param>
        /// <returns>StartReading(string content).</returns>
        public string StartReading(string initialContent)
        {
            Console.CursorVisible = true;
            Console.WriteLine("������� ����� (Ctrl+E ��� ���������� �����):");

            // ������������� ������ � ��������� ����������
            InitializeBuffer(initialContent);

            // ���������� ��� ������������ ��������� ��������������
            bool isEditing = true;
            // ���������� ��� �������� ����������������� �����
            string userInput = initialContent;

            // ���� ��������� ����� ������������
            while (isEditing)
            {
                // ������ ������� �������
                ConsoleKeyInfo keyInfo = Console.ReadKey(true);

                // ��������� ������
                switch (keyInfo.Key)
                {
                    case ConsoleKey.E:
                        // ��������� Ctrl+E ��� ���������� ��������������
                        if ((keyInfo.Modifiers & ConsoleModifiers.Control) != 0)
                        {
                            isEditing = false;
                            break;
                        }
                        else
                        {
                            // ���������� ������� � ������ � ����������������� �����
                            AddCharacter(keyInfo.KeyChar);
                            userInput += keyInfo.KeyChar;
                        }
                        break;

                    case ConsoleKey.Backspace:
                        // ��������� ������� ������� Backspace
                        HandleBackspace();
                        if (userInput.Length > 0)
                        {
                            // �������� ���������� ������� �� ����������������� �����
                            userInput = userInput.Substring(0, userInput.Length - 1);
                        }
                        break;

                    case ConsoleKey.Enter:
                        // ��������� ������� ������� Enter
                        AddNewLine();
                        userInput += "\n"; // ���������� ������� ����� ������ � ����������������� �����
                        break;

                    case ConsoleKey.LeftArrow:
                        // ��������� ������� ������� LeftArrow
                        MoveCursorLeft();
                        break;

                    case ConsoleKey.RightArrow:
                        // ��������� ������� ������� RightArrow
                        MoveCursorRight();
                        break;

                    default:
                        // ���������� ������� � ������ � ����������������� �����
                        AddCharacter(keyInfo.KeyChar);
                        userInput += keyInfo.KeyChar;
                        break;
                }
            }

            // ������� ��������� ������
            Console.CursorVisible = false;
            return userInput;
        }

        /// ����� �������������� ����� ����� ��������� ����������
        /// <param name="initialContent">������� ������� ����� ���������������.</param>
        public void InitializeBuffer(string initialContent)
        {
            // �������� initialContent �� ������ � ���������� � �����
            content = new List<string>(initialContent.Split('\n'));
            // ������� ������ � ������� = ��������� ������ � � �����
            currentLine = content.Count - 1;
            currentColumn = content[currentLine].Length;
            Console.Write(initialContent);
        }

        /// ����� ��������� ������ � ������� ������� ������
        /// <param name="c">������ ��� ��������� �����.</param>
        private void AddCharacter(char c)
        {
            // ������� ������� � ������� ������ � ������� ������� �������
            content[currentLine] = content[currentLine].Insert(currentColumn, c.ToString());
            // ���������� ������� ������� �������
            currentColumn++;
            // ����� �������
            Console.Write(c);
        }

        /// ������������ �������� ��������, ������� �� ����� ������.
        private void HandleBackspace()
        {
            // ���� �� � ������ ������, �������� ������� ����� �� ������� Column
            if (currentColumn > 0)
            {
                content[currentLine] = content[currentLine].Remove(currentColumn - 1, 1);
                currentColumn--;
                // �������� ������� � �������
                Console.Write("\b \b");
            }
            else if (currentLine > 0)
            {
                // ������� �� ���������� ������ � ����������� � � �������
                currentColumn = content[currentLine - 1].Length;
                Console.CursorLeft = 0;
                Console.CursorTop--;
                Console.Write(content[currentLine - 1]);
                content[currentLine - 1] += content[currentLine];
                content.RemoveAt(currentLine);
                currentLine--;
            }
        }

        /// ���������� ����� ������ � �����
        public void AddNewLine()
        {
            // ������� ������ ������ ����� ������� ������
            content.Insert(currentLine + 1, string.Empty);
            // ������� � ����� ������ �  currentColumn = 0 
            currentLine++;
            currentColumn = 0;
            // ����� ����� ������ � �������
            Console.WriteLine();
        }

        /// ����������� ������� �����
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

        /// ����������� ������� ������
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

        /// ���������� �������� ������.
        /// <returns>���������� ������ � ���� ����� ������.</returns>
        public string GetBufferContent()
        {
            // ����������� ����� ������ � ���� ������ � ������������ "\n"
            return string.Join("\n", content);
        }
    }
}
