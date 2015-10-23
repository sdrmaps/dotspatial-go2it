using System.Text;
using System.Runtime.InteropServices;

namespace SDR.Data.Files
{
    public static class IniHelper
    {
        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);
        // gathers a value from a known section header and entry key
        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string val, StringBuilder retVal, int size, string filePath);
        // gets a list of entry keys from a known section header
        [DllImport("kernel32")]
        static extern int GetPrivateProfileString(string section, int key, string val, [MarshalAs(UnmanagedType.LPArray)] byte[] result, int size, string fileName);

        ///<summary>
        /// Writes a value to the INI file
        ///</summary>
        ///<param name="section">Ini section</param>
        ///<param name="key">Ini key lookup</param>
        ///<param name="value">Ini key value</param>
        ///<param name="path">Ini file path</param>
        public static void IniWriteValue(string section, string key, string value, string path)
        {
            WritePrivateProfileString(section, key, value, path);
        }

        ///<summary>
        /// Gets a selected value from the ini file
        ///</summary>
        ///<param name="section">Ini section</param>
        ///<param name="key">Ini key lookup</param>
        ///<param name="path">Ini file path</param>
        ///<returns>String of ini key value</returns>
        public static string IniReadValue(string section, string key, string path)
        {
            for (int maxsize = 250; ; maxsize *= 2)
            {
                // obtains the EntryValue information and uses the StringBuilder
                // function and stores them in the maxsize buffers (result).
                var result = new StringBuilder(maxsize);
                var size = GetPrivateProfileString(section, key, "", result, maxsize, path);
                if (size < maxsize - 1)
                {
                    // returns the value gathered from the key
                    return result.ToString();
                }
            }
        }

        /// <summary>
        /// Returns an array of all KeyLookups under a section of the ini file
        /// </summary>
        /// <param name="section">Ini section</param>
        ///<param name="path">Ini file path</param>
        /// <returns>Array of all key lookups in section</returns>
        public static string[] IniGetAllSectionKeys(string section, string path)
        {
            for (int maxsize = 500; ; maxsize *= 2)
            {
                // obtains the EntryKey information in bytes
                // and stores them in the maxsize buffer (Bytes array).
                var bytes = new byte[maxsize];
                int size = GetPrivateProfileString(section, 0, "", bytes, maxsize, path);
                // check the information obtained is not bigger
                // than the allocated maxsize buffer - 2 bytes.
                // if it is, then skip over the next section
                // so that the maxsize buffer can be doubled.
                if (size >= maxsize - 2) continue;

                // Converts the bytes value into an ASCII char.
                // This is one long string.
                string entries = Encoding.ASCII.GetString(bytes, 0, size - (size > 0 ? 1 : 0));
                // splits the Long string into an array based on the "\0"
                // or null (Newline) value and returns the value(s) in an array
                return entries.Split(new[] { '\0' });
            }
        }
    }
}
