using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Spectre.Console;
using System.Reflection;

public class GoogleDriveAuthenticator
{
    public static DriveService Authenticate()
    {
        AnsiConsole.MarkupLine("Authenticating with Google Drive...");

        UserCredential credential;
        var credentialPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "gdrive-cli", "credentials");

        var assemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        if (string.IsNullOrEmpty(assemblyPath))
        {
            throw new Exception("Could not determine the path of the executing assembly.");
        }
        var clientSecretsPath = Path.Combine(assemblyPath, "client_secrets.json");

        // ... existing code ...
        using (var stream = new FileStream(clientSecretsPath, FileMode.Open, FileAccess.Read))
        {
            var clientSecrets = GoogleClientSecrets.FromStream(stream).Secrets;
            credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                clientSecrets,
                new[] { DriveService.Scope.DriveReadonly },
                "user",
                CancellationToken.None,
                new FileDataStore(credentialPath, true),
                new ConsoleCodeReceiver()).Result;
        }

        AnsiConsole.MarkupLine("[green]Authentication successful.[/]");
// ... existing code ...

        return new DriveService(new BaseClientService.Initializer()
        {
            HttpClientInitializer = credential,
            ApplicationName = "Google Drive CLI"
        });
    }
}
