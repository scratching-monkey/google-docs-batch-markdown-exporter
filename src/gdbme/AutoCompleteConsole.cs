using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spectre.Console;

public class AutoCompleteConsole
{
    private readonly GoogleDriveManager _driveManager;
    private readonly List<string> _baseCommands = new List<string> { "ls", "cd", "download", "pwd", "clear", "cls", "exit", "quit" };
    private StringBuilder _currentInput = new StringBuilder();
    private int _cursorPosition = 0;

    public AutoCompleteConsole(GoogleDriveManager driveManager)
    {
        _driveManager = driveManager;
    }

    // ... existing code ...
    public async Task Run()
    {
        AnsiConsole.MarkupLine("[bold yellow]Welcome to Google Drive CLI![/]");
        AnsiConsole.MarkupLine("Type a command and press [yellow]Tab[/] for auto-completion. Press [yellow]Enter[/] to execute.");

        RedrawConsole();
        while (true)
        {
            ConsoleKeyInfo key = Console.ReadKey(true);
            await HandleKey(key);
        }
    }

    private async Task HandleKey(ConsoleKeyInfo key)
// ... existing code ...
    {
        // ... existing code ...
        if (key.Key == ConsoleKey.Enter)
        {
            Console.WriteLine();
            var command = _currentInput.ToString();
            _currentInput.Clear();
            _cursorPosition = 0;
            await ExecuteCommand(command);
            RedrawConsole();
        }
        else if (key.Key == ConsoleKey.Backspace)
        {
// ... existing code ...
            if (_cursorPosition > 0)
            {
                _currentInput.Remove(_cursorPosition - 1, 1);
                _cursorPosition--;
                RedrawConsole();
            }
        }
        else if (key.Key == ConsoleKey.Tab)
        {
            await HandleTabCompletion();
            RedrawConsole();
        }
        // ... existing code ...
        else if (key.Key == ConsoleKey.LeftArrow)
        {
            if (_cursorPosition > 0)
            {
                _cursorPosition--;
                RedrawConsole();
            }
        }
        else if (key.Key == ConsoleKey.RightArrow)
        {
            if (_cursorPosition < _currentInput.Length)
            {
                _cursorPosition++;
                RedrawConsole();
            }
        }
        else if (!char.IsControl(key.KeyChar))
        {
// ... existing code ...
            _currentInput.Insert(_cursorPosition, key.KeyChar);
            _cursorPosition++;
            RedrawConsole();
        }
    }

    private async Task HandleTabCompletion()
    {
        var input = _currentInput.ToString();
        var parts = ParseCommand(input);
        var currentPart = parts.LastOrDefault() ?? "";

        IEnumerable<string> suggestions;
        if (parts.Length <= 1)
        {
            suggestions = _baseCommands.Where(c => c.StartsWith(currentPart, StringComparison.OrdinalIgnoreCase));
        }
        else
        {
            var command = parts[0];
            if (command.Equals("cd", StringComparison.OrdinalIgnoreCase) || command.Equals("download", StringComparison.OrdinalIgnoreCase))
            {
                var driveItems = await _driveManager.GetCurrentDirectoryItemNames();
                suggestions = driveItems.Where(item => item.StartsWith(currentPart, StringComparison.OrdinalIgnoreCase));
            }
            else
            {
                suggestions = new List<string>();
            }
        }

        // ... existing code ...
        var suggestionArray = suggestions.ToArray();
        if (suggestionArray.Length > 1 && !string.IsNullOrEmpty(currentPart))
        {
            Console.WriteLine(); // New line for suggestions
            AnsiConsole.WriteLine(string.Join("  ", suggestionArray));
            RedrawConsole(); // Redraw the prompt and current input
        }
        else if (suggestionArray.Length == 1)
        {
            var suggestion = suggestionArray[0];
// ... existing code ...
            var lastSpaceIndex = input.LastIndexOf(' ');
            var prefix = lastSpaceIndex == -1 ? "" : input.Substring(0, lastSpaceIndex + 1);

            // Handle spaces in suggestion
            var completedInput = suggestion.Contains(' ') ? $"\"{suggestion}\"" : suggestion;

            _currentInput.Clear().Append(prefix).Append(completedInput);
            _cursorPosition = _currentInput.Length;
        }
    }

    // ... existing code ...
    private void RedrawConsole()
    {
        Console.SetCursorPosition(0, Console.CursorTop);
        Console.Write(new string(' ', Console.WindowWidth - 1));
        Console.SetCursorPosition(0, Console.CursorTop);

        var prompt = $"gdrive:{_driveManager.GetCurrentPath()}> ";
        AnsiConsole.Markup($"[bold blue]{prompt}[/]");

        Console.Write(_currentInput);

        Console.SetCursorPosition(prompt.Length + _cursorPosition, Console.CursorTop);
    }

    private async Task ExecuteCommand(string commandText)
// ... existing code ...
    {
        if (string.IsNullOrWhiteSpace(commandText)) return;

        var parts = ParseCommand(commandText);
        var action = parts[0].ToLower();
        var argument = parts.Length > 1 ? parts[1] : null;

        switch (action)
        {
            case "ls":
                await _driveManager.ListCurrentDirectory();
                break;
            case "cd":
                if (!string.IsNullOrEmpty(argument)) await _driveManager.ChangeDirectory(argument);
                else AnsiConsole.MarkupLine("[red]Usage: cd <folder_name> | .. | /[/]");
                break;
            case "download":
                if (!string.IsNullOrEmpty(argument))
                {
                    var force = argument.StartsWith("-f ");
                    var itemName = force ? argument.Substring(3) : argument;
                    await _driveManager.Download(itemName, force);
                }
                else AnsiConsole.MarkupLine("[red]Usage: download [-f] <item_name> | .[/]");
                break;
            case "pwd":
                AnsiConsole.WriteLine(_driveManager.GetCurrentPath());
                break;
            case "clear":
            case "cls":
                AnsiConsole.Clear();
                break;
            case "exit":
            case "quit":
                Environment.Exit(0);
                break;
            default:
                AnsiConsole.MarkupLine($"[bold red]Unknown command: {commandText}[/]");
                break;
        }
    }

    private string[] ParseCommand(string commandText)
    {
        var parts = new List<string>();
        var currentPart = new StringBuilder();
        bool inQuotes = false;

        foreach (char c in commandText)
        {
            if (c == '\"')
            {
                inQuotes = !inQuotes;
            }
            else if (c == ' ' && !inQuotes)
            {
                if (currentPart.Length > 0)
                {
                    parts.Add(currentPart.ToString());
                    currentPart.Clear();
                }
            }
            else
            {
                currentPart.Append(c);
            }
        }

        if (currentPart.Length > 0)
        {
            parts.Add(currentPart.ToString());
        }

        return parts.ToArray();
    }
}
