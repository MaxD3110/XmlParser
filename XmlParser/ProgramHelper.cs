using System.Text.RegularExpressions;

namespace XmlParser;

internal static class ProgramHelper
{
    internal static string RequestValidPath()
    {
        var validFile = false;
        var enteredPath = string.Empty;
    
        while (!validFile)
        {
            Console.WriteLine("--> Enter path to file: ");

            enteredPath = Console.ReadLine();

            if (string.IsNullOrEmpty(enteredPath) || !File.Exists(enteredPath))
            {
                Console.WriteLine("Invalid path");
                continue;
            }

            validFile = true;
        }

        return enteredPath!;
    }

    private static string RequestValidDivider()
    {
        Console.WriteLine("Choose XML-tag to select objects \n (e.g. '<item>' will select <item>Something</item> constructions): ");

        var enteredDivider = string.Empty;
        var validDivider = false;

        while (!validDivider)
        {
            Console.WriteLine("--> Enter divider: ");

            enteredDivider = Console.ReadLine();

            if (string.IsNullOrEmpty(enteredDivider))
            {
                Console.WriteLine("Invalid divider");
                continue;
            }
        
            enteredDivider = Regex.Match(enteredDivider, @"<\/?(\w+)>?").Groups[1].Value;

            if (string.IsNullOrEmpty(enteredDivider))
            {
                Console.WriteLine("Invalid divider");
                continue;
            }

            validDivider = true;
        }

        return enteredDivider!.Trim();
    }
    
    internal static int? RequestValidShardCount()
    {
        Console.WriteLine("Application would generate number of files, \n depending on maximum number of items in single file.");
    
        while (true)
        {
            Console.WriteLine("--> Enter max number of items in one file (or leave it empty for no limit): ");

            var enteredNumber = Console.ReadLine();

            if (string.IsNullOrEmpty(enteredNumber))
                return null;

            if (int.TryParse(enteredNumber, out var number))
                return number;

            Console.WriteLine("Invalid number");
        }
    }
    
    internal static async Task<List<string>> SearchItemsInFile(string filepath)
    {
        var tagDivider = RequestValidDivider();
        var fileText = await File.ReadAllTextAsync(filepath);

        var items = new List<string>();

        var pattern = $@"<({tagDivider})\b[^>]*>.*?</\1>";
        var matches = Regex.Matches(fileText, pattern, RegexOptions.Singleline | RegexOptions.IgnoreCase);

        foreach (Match match in matches)
            items.Add(match.Value.Trim());

        return items;
    }
    
    internal static async Task GenerateXmlFiles(List<string> items, string originalPath, int? count)
    {
        var directory = Path.GetDirectoryName(originalPath) ?? string.Empty;
        var fileName = Path.GetFileNameWithoutExtension(originalPath);
        var fileNumber = 1;

        count ??= items.Count;

        for (var i = 0; i < items.Count; i+= count.Value)
        {
            var localPath = Path.Combine(directory, $"{fileName}_shard_{fileNumber}.xml");
        
            Console.WriteLine($"Writing into {localPath}");

            var currentChunk = new List<string> {Constants.Header};

            currentChunk.AddRange(items.Skip(i).Take(count.Value));
            currentChunk.Add(Constants.Footer);

            await File.WriteAllLinesAsync(localPath, currentChunk);
        
            fileNumber++;
        }
    }
}