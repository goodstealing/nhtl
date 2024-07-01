using Spectre.Console;

namespace nhtl
{
    public class Program
    {
        static void Main()
        {
            try
            {
                ShowMainMenu();
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupInterpolated($"Произошла ошибка: [red]{ex.Message}[/]");
                Console.WriteLine("Нажмите любую клавишу для выхода...");
                Console.ReadKey(true);
            }
        }

        public static void ShowMainMenu()
        {
            Dictionary<ConsoleKey, Func<Task>> keyActions = new()
            {
                { ConsoleKey.O,       () => { OpenHandler.OpenFile(); return Task.CompletedTask; } },
                { ConsoleKey.N,       CreateHandler.CreateFile }
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

                if ((keyInfo.Modifiers & ConsoleModifiers.Control) != 0 && keyActions.ContainsKey(keyInfo.Key))
                {
                    try
                    {
                        keyActions[keyInfo.Key]().Wait();
                    }
                    catch (Exception ex)
                    {
                        AnsiConsole.MarkupInterpolated($"Произошла ошибка: [red]{ex.Message}[/]");
                    }
                }
                else if (keyInfo.Key == ConsoleKey.C && (keyInfo.Modifiers & ConsoleModifiers.Control) != 0)
                {
                    break;
                }
            }
        }
    }
}
