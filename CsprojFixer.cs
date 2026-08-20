#!/usr/bin/env dotnet-script

// Reference Duende OIDC client package

#:package System.CommandLine@2.0.10
#:package Microsoft.Build@18.8.2
#:property Version=1.0.0


using System.CommandLine;
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
            var projectXml = XDocument.Load(fullPath);
            var ns = XNamespace.Get("http://schemas.microsoft.com/developer/msbuild/2003");
            
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
restoreCommand.SetAction(parseResult =>
{
    var paths = new[]
    {
        pathLib, pathDemoCore, pathDemoApp
    }.ToList();

    paths.ForEach(path =>
    {
        var fullPath = Path.Combine(scriptDir, path);
        var backupPath = Path.Combine(scriptDir, "csproj-backups", Path.GetFileName(fullPath));
        
        if (File.Exists(backupPath))
        {
            File.Copy(backupPath, fullPath, true);
            Console.WriteLine($"Restored {Path.GetFileName(fullPath)} from backup");
        }
        else
        {
            Console.WriteLine($"Backup not found: {backupPath}");
        }
    });
});

return await rootCommand.Parse(args).InvokeAsync();