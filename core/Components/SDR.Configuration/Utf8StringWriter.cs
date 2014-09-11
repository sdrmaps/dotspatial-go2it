// used for serialization of dictionaries to xml, utf8 is specified 
// to avoid encoding issues on the xml end of things

using System.IO;
using System.Text;

namespace SDR.Configuration
{
    public class Utf8StringWriter : StringWriter
    {
        public override Encoding Encoding
        {
            get { return Encoding.UTF8; }
        }
    }
}
