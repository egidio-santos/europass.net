﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Europass.Net.Helpers
{
    public sealed class StringWriterWithEncoding : StringWriter
    {
        private readonly Encoding encoding;

        public StringWriterWithEncoding(Encoding encoding)
        {
            this.encoding = encoding;
        }

        public override Encoding Encoding
        {
            get { return encoding; }
        }
    }
}
