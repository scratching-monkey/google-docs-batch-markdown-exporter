using Google.Apis.Drive.v3;
using Spectre.Console;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

public class GoogleDriveManager
{
    private readonly DriveService _service;
    private readonly List<(string Id, string Name)> _currentPath = new() { ("root", "My Drive") };
    private string CurrentFolderId => _currentPath.Last().Id;
    private const string MimeTypeQuery = "(mimeType='application/vnd.google-apps.folder' or mimeType='application/vnd.google-apps.document')";

    public GoogleDriveManager(DriveService service)
    {
        _service = service;
    }

    public string GetCurrentPath()
    {
        return "/" + string.Join("/", _currentPath.Skip(1).Select(f => f.Name));
    }

    public async Task ListCurrentDirectory()
    {
        var request = _service.Files.List();
        request.Q = $"'{CurrentFolderId}' in parents and {MimeTypeQuery} and trashed = false";
        request.Fields = "files(id, name, mimeType)";
        request.OrderBy = "folder, name";

        var result = await request.ExecuteAsync();

        if (result.Files == null || result.Files.Count == 0)
        {
            AnsiConsole.MarkupLine("[yellow]Current directory is empty.[/]");
            return;
        }

        foreach (var file in result.Files)
        {
            if (file.MimeType == "application/vnd.google-apps.folder")
            {
                AnsiConsole.MarkupLine($"[blue]{file.Name}[/]");
            }
            else
            {
                AnsiConsole.WriteLine(file.Name);
            }
        }
    }

    public async Task ChangeDirectory(string folderName)
    {
        if (folderName == "..")
        {
            if (_currentPath.Count > 1)
            {
                _currentPath.RemoveAt(_currentPath.Count - 1);
            }
            return;
        }

        if (folderName == "/")
        {
            _currentPath.Clear();
            _currentPath.Add(("root", "My Drive"));
            return;
        }

        var request = _service.Files.List();
        request.Q = $"'{CurrentFolderId}' in parents and name = '{folderName}' and mimeType = 'application/vnd.google-apps.folder' and trashed = false";
        request.Fields = "files(id, name)";
        var result = await request.ExecuteAsync();

        var folder = result.Files.FirstOrDefault();
        if (folder != null)
        {
            _currentPath.Add((folder.Id, folder.Name));
        }
        else
        {
            AnsiConsole.MarkupLine($"[red]Folder '{folderName}' not found.[/]");
        }
    }

    public async Task<IEnumerable<string>> GetCurrentDirectoryItemNames()
    {
        var request = _service.Files.List();
        request.Q = $"'{CurrentFolderId}' in parents and {MimeTypeQuery} and trashed = false";
        request.Fields = "files(name)";
        request.OrderBy = "folder, name";

        var result = await request.ExecuteAsync();
        return result.Files?.Select(f => f.Name) ?? Enumerable.Empty<string>();
    }

    public async Task Download(string itemName, bool force)
    {
        if (itemName == ".")
        {
            await DownloadCurrentFolderContents(force);
            return;
        }

        var request = _service.Files.List();
        request.Q = $"'{CurrentFolderId}' in parents and name = '{itemName}' and {MimeTypeQuery} and trashed = false";
        request.Fields = "files(id, name, mimeType, createdTime, modifiedTime)";
        var result = await request.ExecuteAsync();

        var item = result.Files.FirstOrDefault();
        if (item == null)
        {
            AnsiConsole.MarkupLine($"[red]Item '{itemName}' not found.[/]");
            return;
        }

        if (item.MimeType == "application/vnd.google-apps.folder")
        {
            await DownloadFolder(item, ".", force);
        }
        else
        {
            await DownloadFile(item, ".", force);
        }
    }

    private async Task DownloadCurrentFolderContents(bool force)
    {
        AnsiConsole.MarkupLine($"Downloading all items in the current directory...");

        var request = _service.Files.List();
        request.Q = $"'{CurrentFolderId}' in parents and {MimeTypeQuery} and trashed = false";
        request.Fields = "files(id, name, mimeType, createdTime, modifiedTime)";

        var result = await request.ExecuteAsync();

        if (result.Files == null || result.Files.Count == 0)
        {
            AnsiConsole.MarkupLine("[yellow]Current directory is empty.[/]");
            return;
        }

        foreach (var item in result.Files)
        {
            if (item.MimeType == "application/vnd.google-apps.folder")
            {
                await DownloadFolder(item, ".", force);
            }
            else
            {
                await DownloadFile(item, ".", force);
            }
        }
    }

    private async Task DownloadFile(Google.Apis.Drive.v3.Data.File file, string localPath, bool force)
    {
        var filePath = Path.Combine(localPath, file.Name);
        var mdFilePath = Path.ChangeExtension(filePath, ".md");

        if (!force && File.Exists(mdFilePath))
        {
            AnsiConsole.MarkupLine($"[yellow]File '{mdFilePath}' already exists. Use -f to overwrite.[/]");
            return;
        }

        AnsiConsole.Markup($"Downloading [green]'{file.Name}'[/]...");

        try
        {
            var exportRequest = _service.Files.Export(file.Id, "text/markdown");
            using var stream = new MemoryStream();
            await exportRequest.DownloadAsync(stream);
            stream.Position = 0;
            using var reader = new StreamReader(stream);
            var markdown = await reader.ReadToEndAsync();

            var frontMatter = $"""
            ---
            title: "{file.Name}"
            gdrive_id: {file.Id}
            created_time: {file.CreatedTimeDateTimeOffset:o}
            modified_time: {file.ModifiedTimeDateTimeOffset:o}
            ---

            """;
            var finalContent = frontMatter + markdown;

            await File.WriteAllTextAsync(mdFilePath, finalContent);
            AnsiConsole.MarkupLine($" Converted to Markdown.");
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($" [red]Failed: {ex.Message}[/]");
        }
    }

    private async Task DownloadFolder(Google.Apis.Drive.v3.Data.File folder, string localPath, bool force)
    {
        var newLocalPath = Path.Combine(localPath, folder.Name);
        Directory.CreateDirectory(newLocalPath);

        AnsiConsole.MarkupLine($"Entering [blue]'{folder.Name}'[/]...");

        var request = _service.Files.List();
        request.Q = $"'{folder.Id}' in parents and {MimeTypeQuery} and trashed = false";
        request.Fields = "files(id, name, mimeType, createdTime, modifiedTime)";

        var result = await request.ExecuteAsync();

        if (result.Files == null) return;

        foreach (var item in result.Files)
        {
            if (item.MimeType == "application/vnd.google-apps.folder")
            {
                await DownloadFolder(item, newLocalPath, force);
            }
            else
            {
                await DownloadFile(item, newLocalPath, force);
            }
        }
    }
}
