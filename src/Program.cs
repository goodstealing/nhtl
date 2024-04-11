﻿using System;
using nhtl;

namespace nhtl
{
    public class Program
    {
        static void Main()
        {
            ShowMainMenu();
        }

        public static void ShowMainMenu()
        {
            Dictionary<ConsoleKey, Action> keyActions = new()
            {
                { ConsoleKey.O, OpenHandler.OpenFile },
                { ConsoleKey.N, CreateHandler.CreateFile },
            };

            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine(OpenHandler.asciiArt);
            Console.ResetColor();

            Console.CursorVisible = false;
            Console.WriteLine("┌───────────────────────────────┐");
            Console.WriteLine("| Ctrl + O | Открыть файл       |");
            Console.WriteLine("├───────────────────────────────┤");
            Console.WriteLine("| Ctrl + N | Создать новый файл |");
            Console.WriteLine("├───────────────────────────────┤");
            Console.WriteLine("| Ctrl + C | Выход              |");
            Console.WriteLine("└───────────────────────────────┘");

            while (true)
            {
                ConsoleKeyInfo keyInfo = Console.ReadKey(true);

                if ((keyInfo.Modifiers & ConsoleModifiers.Control) != 0)
                {
                    switch (keyInfo.Key)
                    {
                        case ConsoleKey.O:
                            OpenHandler.OpenFile();
                            break;
                        case ConsoleKey.N:
                            CreateHandler.CreateFile();
                            break;
                        default:
                            break; // Обработка неизвестных клавиш Ctrl
                    }
                }
            }
        }
    }
}