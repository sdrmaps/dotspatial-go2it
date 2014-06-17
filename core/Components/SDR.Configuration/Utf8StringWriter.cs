// used for serialization of dixtionaries to xml, utf8 is specified to avoid issues on xml end

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
