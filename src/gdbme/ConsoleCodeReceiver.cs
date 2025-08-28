using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Requests;
using Google.Apis.Auth.OAuth2.Responses;
using Spectre.Console;

public class ConsoleCodeReceiver : ICodeReceiver
{
    public string RedirectUri => "http://127.0.0.1:58263/";

    public async Task<AuthorizationCodeResponseUrl> ReceiveCodeAsync(AuthorizationCodeRequestUrl url, CancellationToken taskCancellationToken)
    {
        var authorizationUrl = url.Build().ToString();
        AnsiConsole.MarkupLine("Please go to the following link in your browser:");
        AnsiConsole.MarkupLine($"[link]{authorizationUrl}[/]");

        var code = AnsiConsole.Ask<string>("Enter the authorization code you received:");

        return new AuthorizationCodeResponseUrl { Code = code };
    }
}
