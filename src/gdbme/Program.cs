using System.Threading.Tasks;

public class Program
{
    public static async Task Main(string[] args)
    {
        var driveService = GoogleDriveAuthenticator.Authenticate();
        var driveManager = new GoogleDriveManager(driveService);
        var console = new AutoCompleteConsole(driveManager);

        await console.Run();
    }
}
