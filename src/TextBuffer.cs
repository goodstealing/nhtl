using System;
using System.Collections.Generic;

namespace nhtl
{
    public class TextBuffer
    {
        private List<string> lines;
        private int currentLine;
        private int currentColumn;

        public TextBuffer()
        {
            lines = new List<string> { string.Empty };
            currentLine = 0;
            currentColumn = 0;
        }

        // ��������� ���������� ������ StartReading ��� �������� ���������� �����������
        public string StartReading(string initialContent)
        {

            Console.WriteLine("������� ����� (Ctrl+E ��� ���������� �����):");
            InitializeBuffer(initialContent); // �������������� ����� � ��������� ����������

            bool isEditing = true;
            string userInput = initialContent; // ��������� ���������� ��� ��������������

            while (isEditing)
            {
                ConsoleKeyInfo keyInfo = Console.ReadKey(true);

                switch (keyInfo.Key)
                {
                    case ConsoleKey.E:
                        if ((keyInfo.Modifiers & ConsoleModifiers.Control) != 0)
                        {
                            isEditing = false;
                            break;
                        }
                        else
                        {
                            AddCharacter(keyInfo.KeyChar);
                            userInput += keyInfo.KeyChar; // ��������� ������ � ����������������� �����
                        }
                        break;

                    case ConsoleKey.Backspace:
                        HandleBackspace();
                        if (userInput.Length > 0)
                        {
                            userInput = userInput.Substring(0, userInput.Length - 1); // ������� ��������� ������ �� ����������������� �����
                        }
                        break;

                    case ConsoleKey.Enter:
                        AddNewLine();
                        userInput += "\n"; // ��������� ������ ����� ������ � ����������������� �����
                        break;

                    default:
                        AddCharacter(keyInfo.KeyChar);
                        userInput += keyInfo.KeyChar; // ��������� ������ � ����������������� �����
                        break;
                }
            }

            return userInput; // ���������� ������������� ���������������� ����
        }

        // ������������� ������ � ��������� ����������
        public void InitializeBuffer(string initialContent)
        {
            lines = new List<string>(initialContent.Split('\n')); // ��������� ��������� ���������� �� ������
            currentLine = lines.Count - 1;
            currentColumn = lines[currentLine].Length;
            Console.Write(initialContent); // ������� ��������� ���������� � �������
        }


        private void AddCharacter(char c)
        {
            lines[currentLine] = lines[currentLine].Insert(currentColumn, c.ToString());
            currentColumn++;
            Console.Write(c);
        }

        private void HandleBackspace()
        {
            if (currentColumn > 0)
            {
                lines[currentLine] = lines[currentLine].Remove(currentColumn - 1, 1);
                currentColumn--;
                Console.Write("\b \b");
            }
            else if (currentLine > 0)
            {
                currentColumn = lines[currentLine - 1].Length;
                Console.CursorLeft = 0;
                Console.CursorTop--;
                Console.Write(lines[currentLine - 1]);
                lines[currentLine - 1] += lines[currentLine];
                lines.RemoveAt(currentLine);
                currentLine--;
            }
        }

        public void AddNewLine()
        {
            lines.Insert(currentLine + 1, string.Empty);
            currentLine++;
            currentColumn = 0;
            Console.WriteLine();
        }

        public string GetBufferContent()
        {
            return string.Join("\n", lines);
        }
    }
}
