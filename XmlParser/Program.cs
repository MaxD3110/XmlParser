using static XmlParser.ProgramHelper;

var path = RequestValidPath();

var elements = await SearchItemsInFile(path);

if (elements.Count > 0)
    Console.WriteLine($"Founded {elements.Count} elements!");
else
{
    Console.WriteLine("Search wasn't successful :(");
    return;
}

var shardCount = RequestValidShardCount();

Console.WriteLine("Generating XML-files with selected items ...");

await GenerateXmlFiles(elements, path, shardCount);

Console.WriteLine($"Done. Check {Path.GetDirectoryName(path)} for your result. \n Have a nice day!");

Console.ReadLine();