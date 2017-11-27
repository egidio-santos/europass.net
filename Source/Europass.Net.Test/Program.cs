using System;
using System.IO;

namespace Europass.Net.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            var cv = Europass.Net.Converter.ReadXml(@"C:\GitHub\CV-Europass-20171127-LuísRodriguesDosSantosFilho-EN.xml");
            var pdf = Europass.Net.Converter.ReadPdf(@"C:\GitHub\CV-Europass-20171126-LuísRodriguesDosSantosFilho-EN.pdf");
            
            File.WriteAllText(@"C:\GitHub\CV-Europass-20171126-LuísRodriguesDosSantosFilho-EN.json", Europass.Net.Converter.ToJson(cv, true));

            Console.WriteLine(cv.LearnerInfo.Identification.PersonName.FirstName);
            Console.WriteLine(pdf.LearnerInfo.Identification.PersonName.FirstName);
            Console.ReadKey();
        }
    }
}
