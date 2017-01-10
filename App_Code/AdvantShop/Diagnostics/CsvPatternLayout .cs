//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System.IO;
using System.Text;
using log4net.Core;
using log4net.Layout;
using log4net.Util;

namespace AdvantShop.Diagnostics
{
    public class CsvPatternLayout : PatternLayout
    {
        public override void ActivateOptions()
        {
            // register custom pattern tokens
            AddConverter("newfield", typeof(NewFieldConverter));
            AddConverter("endrow", typeof(EndRowConverter));
            base.ActivateOptions();
        }

        public override void Format(TextWriter writer, LoggingEvent loggingEvent)
        {
            var ctw = new CsvTextWriter(writer);
            // write the starting quote for the first field
            ctw.WriteQuote();
            base.Format(ctw, loggingEvent);
        }
    }

    public class NewFieldConverter : PatternConverter
    {
        protected override void Convert(TextWriter writer, object state)
        {
            var ctw = writer as CsvTextWriter;
            // write the ending quote for the previous field
            if (ctw != null)
                ctw.WriteQuote();
            writer.Write(';');
            // write the starting quote for the next field
            if (ctw != null)
                ctw.WriteQuote();
        }
    }

    public class EndRowConverter : PatternConverter
    {
        protected override void Convert(TextWriter writer, object state)
        {
            var ctw = writer as CsvTextWriter;
            // write the ending quote for the last field
            if (ctw != null)
                ctw.WriteQuote();
            writer.WriteLine();
        }
    }

    public class CsvTextWriter : TextWriter
    {
        private readonly TextWriter _textWriter;

        public CsvTextWriter(TextWriter textWriter)
        {
            _textWriter = textWriter;
        }

        public override Encoding Encoding
        {
            get { return _textWriter.Encoding; }
        }

        public override void Write(char value)
        {
            if (value == '\n') return;

            _textWriter.Write(value);
            // double all quotes
            if (value == '"')
                _textWriter.Write(value);
        }

        public void WriteQuote()
        {
            // write a literal (unescaped) quote
            _textWriter.Write('"');
        }
    }
}