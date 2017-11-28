using System;
using System.IO;
using System.Linq;

namespace Europass.Net.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            //Reach the samples folder
            var lookupFolder = @"..\..\Doc\Samples\";
            var samples = Directory.GetFiles(lookupFolder, "*.xml").ToList();
            samples.AddRange(Directory.GetFiles(lookupFolder, "*.pdf"));

            foreach (var path in samples)
            {
                try
                {
                    var file = new FileInfo(path);

                    //Ignore test files
                    if (file.Name.Contains("Test")) continue;

                    Console.WriteLine($"Reading file: {path}");
                    Europass.Net.Model.SkillsPassport cv = null;
                    switch (file.Extension.ToLowerInvariant())
                    {
                        case ".xml":
                            cv = Europass.Net.Converter.ReadXml(path);
                            break;
                        case ".pdf":
                            cv = Europass.Net.Converter.ReadPdf(path);
                            break;
                        default:
                            throw new InvalidOperationException("Invalid file type");
                            break;
                    }

                    //Strip file extension
                    var fileName = $"{lookupFolder}{file.Name.Replace(".", "-")}";

                    //Testing XML writing
                    File.WriteAllText($"{fileName}-Test.xml",
                        Europass.Net.Converter.ToXmlString(cv));

                    //Testing Json Convertion
                    File.WriteAllText($"{fileName}-Test.json", 
                        Europass.Net.Converter.ToJson(cv, true));
                    File.WriteAllText($"{fileName}-NoBinaries-Test.json",
                        Europass.Net.Converter.ToJson(cv));

                    //Testing PDF Generation
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }

            Console.ReadKey();
        }
    }
}
