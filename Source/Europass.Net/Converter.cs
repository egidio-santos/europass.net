﻿using Europass.Net.Helpers;
using Europass.Net.Model;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.Xml.XPath;

namespace Europass.Net
{
    /// <summary>
    /// Read, Write and convert between Europass CV Types.
    /// </summary>
    public static class Converter
    {
        #region XML Functions
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
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(Model.SkillsPassport));
                return (Model.SkillsPassport)serializer.Deserialize(stream);
            }
            catch (Exception e)
            {
                throw new InvalidOperationException("Could not convert the XML Data, check inner exception for more information.", e);
            }
        }

        /// <summary>
        /// To the XML string.
        /// </summary>
        /// <param name="cv">The cv.</param>
        /// <returns></returns>
        public static string ToXmlString(Model.SkillsPassport cv)
        {
            XmlDocument xml = new XmlDocument();
            
            using (XmlWriter writer = xml.CreateNavigator().AppendChild())
            {
                new XmlSerializer(cv.GetType()).Serialize(writer, cv);
            }

            var settings = new XmlWriterSettings
            {
                Encoding = new UnicodeEncoding(false,false),
                Indent = true,
                OmitXmlDeclaration = false
            };

            using (var stringWriter = new StringWriter())
            using (var xmlTextWriter = XmlWriter.Create(stringWriter,settings))
            {
                xml.WriteTo(xmlTextWriter);
                xmlTextWriter.Flush();
                return stringWriter.GetStringBuilder().ToString();
            }
        }
        #endregion

        #region Json Functions
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
        #endregion

        #region PDF Functions
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
                throw new InvalidOperationException("Could not read the XML attachments, please check if the file was exported correctly.", e);
            }
        }
        #endregion
    }
}
