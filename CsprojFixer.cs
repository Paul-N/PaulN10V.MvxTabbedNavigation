#!/usr/bin/env dotnet-script

// Reference Duende OIDC client package

#:package System.CommandLine@2.0.10
#:package Microsoft.Build@18.8.2
#:property Version=1.0.0


using System.CommandLine;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Xml.Linq;

string scriptDir = AppContext.GetData("EntryPointFileDirectoryPath") as string
                   ?? AppContext.BaseDirectory;   // fallback when published

var pathLib = "sources/EvilGenius.MvxTabbedNavigation/EvilGenius.MvxTabbedNavigation.csproj";
var pathDemoCore = "sources/Demos/EvilGenius.MvxTabbedNavigation.Demo.Core/EvilGenius.MvxTabbedNavigation.Demo.Core.csproj";
var pathDemoApp = "sources/Demos/EvilGenius.MvxTabbedNavigation.Demo/EvilGenius.MvxTabbedNavigation.Demo.csproj";

Command fixCommand = new("fix")
{
    Description = "Fix csprojs",
    //Action = new CommandLineAction()
};

Command restoreCommand = new("restore")
{
    Description = "Restore csprojs",
};

Option<bool> skipbackupOption = new("--skipbackup")
{
    Description = "Skip backup when fixing",
};

RootCommand rootCommand = new("CsprojFixer CLI")
{
    // backupOption,
    // restoreOption,
};

fixCommand.Options.Add(skipbackupOption);

rootCommand.Subcommands.Add(fixCommand);
rootCommand.Subcommands.Add(restoreCommand);

fixCommand.SetAction(parseResult =>
{
    var paths = new[]
    {
        pathLib, pathDemoCore, pathDemoApp
    }.ToList();
    if (!parseResult.GetValue(skipbackupOption))
    {
        paths.ForEach(path =>
        {
            var fullPath = Path.Combine(scriptDir, path);
            if (File.Exists(fullPath))
            {
                File.Copy(fullPath, Path.Combine(scriptDir, "csproj-backups", Path.GetFileName(fullPath)), true);

            }
            else
            {
                Console.WriteLine($"File not found: {fullPath}");
            }
        });
    }
    
    paths.ForEach(path =>
    {
        var fullPath = Path.Combine(scriptDir, path);
        if (File.Exists(fullPath))
        {
            var projectXml = System.Xml.Linq.XDocument.Load(fullPath);
            var ns = System.Xml.Linq.XNamespace.Get("http://schemas.microsoft.com/developer/msbuild/2003");
            
            var tfmElement = projectXml.Root?.Element("PropertyGroup")?.Element("TargetFrameworks") ??
                            projectXml.Root?.Descendants("TargetFrameworks").FirstOrDefault();
            
            if (tfmElement != null)
            {
                var tfms = tfmElement.Value.Split(';', StringSplitOptions.RemoveEmptyEntries);
                var filtered = tfms
                    .Where(tfm => Regex.IsMatch(tfm.Trim(), @"^net\d+\.\d+(-android)?$"))
                    .ToList();
                
                if (filtered.Count > 0 && filtered.Count < tfms.Length)
                {
                    tfmElement.Value = string.Join(";", filtered);
                    projectXml.Save(fullPath);
                    Console.WriteLine($"Updated {Path.GetFileName(fullPath)}: {string.Join(", ", filtered)}");
                }
            }
        }
        else
        {
            Console.WriteLine($"File not found: {fullPath}");
        }
    });
    
});
restoreCommand.SetAction(_ => { Console.WriteLine("restore"); });

// rootCommand.SetAction(async (parseResult, _) =>
// {
//     try
//     {
//         var baseUri = parseResult.GetValue(backupOption) switch
//         {
//             "dev" => "http://localhost:8003",
//             "prod" => "https://id.n10v.me",
//             _ => null
//         };
//
//         if (baseUri is null)
//         {
//             Console.WriteLine("Invalid environment");
//             return await Task.FromResult(-1);
//         }
//
//         if (parseResult.GetValue(restoreOption) is string token)
//         {
//             var refresher = new TokenRefresher(baseUri);
//
//             var result = await refresher.RefreshTokenAsync(token);
//
//             Console.WriteLine(result != null ? result : "Failed to refresh token.");
//             return result != null ? 0: -1;
//         }
//         else
//         {
//             Console.WriteLine("No token provided.");
//             return -1;
//         }
//     }
//     catch (Exception e)
//     {
//         Console.WriteLine(e);
//         return -1;
//     }
// });

return await rootCommand.Parse(args).InvokeAsync();

class TokenRefresher
{
    private readonly string _baseUri;

    public TokenRefresher(string baseUri)
    {
        _baseUri = baseUri;
    }
    
    public async Task<RefreshedTokenResponse?> RefreshTokenAsync(string refreshToken)
    {
        using var client = new HttpClient();
        
        client.BaseAddress = new Uri(_baseUri);

        var formData = new Dictionary<string, string>
        {
            { "grant_type", "refresh_token" },
            { "refresh_token", refreshToken},
            { "client_id", "native.app" }
        };

        var content = new FormUrlEncodedContent(formData);

        content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/x-www-form-urlencoded");
        client.DefaultRequestHeaders.Accept.Add(
            new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

        var response = await client.PostAsync("/realms/smsresender/protocol/openid-connect/token", content);

        var context = RefreshTokenResponseContext.Default;
        var jsonTypeInfo =  context.RefreshedTokenResponse;
        return await response.Content.ReadFromJsonAsync(jsonTypeInfo, CancellationToken.None);
    }
}


[JsonSerializable(typeof(RefreshedTokenResponse))]
internal partial class RefreshTokenResponseContext : JsonSerializerContext
{
}

public record RefreshedTokenResponse(
    [property: JsonPropertyName("access_token")] string AccessToken,
    [property: JsonPropertyName("expires_in")] int ExpiresIn,
    [property: JsonPropertyName("refresh_expires_in")] int RefreshExpiresIn,
    [property: JsonPropertyName("refresh_token")] string RefreshToken,
    [property: JsonPropertyName("token_type")] string TokenType,
    [property: JsonPropertyName("not-before-policy")] int NotBeforePolicy,
    [property: JsonPropertyName("session_state")] string SessionState,
    [property: JsonPropertyName("scope")] string Scope
);