using Europass.Net.Helpers;
using Europass.Net.Model;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace Europass.Net
{
    /// <summary>
    /// Read, Write and convert between Europass CV Types.
    /// </summary>
    public static class Converter
    {
        /// <summary>
        /// Reads the XML.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        public static Model.SkillsPassport ReadXml(string path)
        {
            using (var stream = File.OpenRead(path))
            {
                return Converter.ReadXml(stream);
            }
        }

        /// <summary>
        /// Reads the XML.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <returns></returns>
        public static Model.SkillsPassport ReadXml(Stream stream)
        {
            Model.SkillsPassport ret = null;

            XmlSerializer serializer = new XmlSerializer(typeof(Model.SkillsPassport));
            ret = (Model.SkillsPassport)serializer.Deserialize(stream);

            return ret;
        }

        /// <summary>
        /// To the json.
        /// </summary>
        /// <param name="skillsPassport">The skills passport.</param>
        /// <param name="includeBinaries">If set to <c>true</c> will serialize images and attachments.</param>
        /// <returns></returns>
        public static string ToJson(Model.SkillsPassport skillsPassport, bool includeBinaries = false)
        {
            if (includeBinaries) return Newtonsoft.Json.JsonConvert.SerializeObject(skillsPassport);

            var toSerialize = skillsPassport.Clone();
            toSerialize.AttachmentList = new AttachmentType[0];
            toSerialize.LearnerInfo.Identification.Photo = null;
            toSerialize.LearnerInfo.Identification.Signature = null;

            //TODO: Check if it's necessary to clean the references
            //toSerialize.LearnerInfo.AchievementList[0].Documentation[0].

            return Newtonsoft.Json.JsonConvert.SerializeObject(toSerialize);
        }

        /// <summary>
        /// Reads the PDF.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        public static Model.SkillsPassport ReadPdf(string path)
        {
            using (var stream = File.OpenRead(path))
            {
                return Converter.ReadPdf(stream);
            }
        }

        /// <summary>
        /// Reads the PDF.
        /// </summary>
        /// <param name="pdfStream">The PDF stream.</param>
        /// <returns></returns>
        public static Model.SkillsPassport ReadPdf(Stream pdfStream)
        {
            try
            {
                var files = new List<byte[]>();
                var reader = new iTextSharp.text.pdf.PdfReader(pdfStream);
                var root = reader.Catalog;
                var embeddedFiles = root.GetAsDict(PdfName.NAMES)?
                    .GetAsDict(PdfName.EMBEDDEDFILES)?.GetAsArray(PdfName.NAMES); //may be null

                //Just give up, attachment not found
                if (embeddedFiles == null) return null;

                for (var i = 0; i < embeddedFiles.Size; i += 2)
                {
                    //var name = embeddedFiles.GetAsName(i); // should always be present
                    var fileSpec = embeddedFiles.GetAsDict(i + 1); // ditto
                    var ef = fileSpec.GetAsDict(PdfName.EF);
                    files.Add(PdfReader.GetStreamBytes((ef.GetAsStream(PdfName.F) as PRStream)));
                }

                //No XML included
                if (files.Count == 0) return null;

                var serializer = new XmlSerializer(typeof(Model.SkillsPassport));
                var ret = (Model.SkillsPassport)serializer.Deserialize(new MemoryStream(files[0]));

                return ret;
            }
            catch (Exception e)
            {
                //TODO: Lookup exception and maybe translate the error?
                throw;
            }
        }
    }
}
