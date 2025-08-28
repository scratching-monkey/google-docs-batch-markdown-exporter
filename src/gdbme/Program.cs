using Spectre.Console;
using System.Threading.Tasks;

public class Program
{
    public static async Task Main(string[] args)
    {
        var driveService = GoogleDriveAuthenticator.Authenticate();
        var driveManager = new GoogleDriveManager(driveService);

        AnsiConsole.MarkupLine("[bold yellow]Welcome to Google Drive CLI![/]");

        var keepRunning = true;
        while (keepRunning)
        {
            var currentPath = driveManager.GetCurrentPath();
            var command = AnsiConsole.Ask<string>($"[bold blue]gdrive:{currentPath}> [/]");
            var parts = command.Split(' ', 2);
            var action = parts[0].ToLower();
            var argument = parts.Length > 1 ? parts[1] : null;

            switch (action)
            {
                case "ls":
                    await driveManager.ListCurrentDirectory();
                    break;
                case "cd":
                    if (!string.IsNullOrEmpty(argument))
                    {
                        await driveManager.ChangeDirectory(argument);
                    }
                    else
                    {
                        AnsiConsole.MarkupLine("[red]Usage: cd <folder_name> | .. | /[/]");
                    }
                    break;
                case "download":
                    if (!string.IsNullOrEmpty(argument))
                    {
                        var force = argument.StartsWith("-f ");
                        var itemName = force ? argument.Substring(3) : argument;
                        await driveManager.Download(itemName, force);
                    }
                    else
                    {
                        AnsiConsole.MarkupLine("[red]Usage: download [-f] <item_name>[/]");
                    }
                    break;
                case "exit":
                case "quit":
                    keepRunning = false;
                    break;
                default:
                    AnsiConsole.MarkupLine($"[bold red]Unknown command: {command}[/]");
                    break;
            }
        }
    }
}

