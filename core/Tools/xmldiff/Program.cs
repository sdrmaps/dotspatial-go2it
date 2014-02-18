using System;
using System.IO;
using System.Collections;
using System.ComponentModel;
using System.Xml;
using Microsoft.XmlDiffPatch;

namespace xmldiff
{
    class Program
    {
        private static void ShowUsage()
        {
            Console.WriteLine("xmldiff.exe [filename1] [filename2]");
        }

        [STAThread]
        public static int Main(string[] args)
        {
            if (args.Length > 0)
            {
                if (args.Length != 2)
                {
                    ShowUsage();
                    return 0;
                }
            
                string filename1 = args[0];
                string filename2 = args[1];

                XmlDiff diff = new XmlDiff();

                XmlDiffOptions diffOptions = XmlDiffOptions.IgnoreChildOrder;
                diffOptions = diffOptions | XmlDiffOptions.IgnoreComments;
                diffOptions = diffOptions | XmlDiffOptions.IgnoreXmlDecl;
                diffOptions = diffOptions | XmlDiffOptions.IgnoreWhitespace;
                diffOptions = diffOptions | XmlDiffOptions.IgnoreXmlDecl;

                //Default algorithm is Auto.
                diff.Algorithm = XmlDiffAlgorithm.Auto;
                diff.Options = diffOptions;

                bool compareFragments = false;
                bool isEqual = diff.Compare(filename1, filename2, compareFragments);
                if (!isEqual)
                {
                    Console.WriteLine("Non-Matching Files");
                    return 1;
                }
                else
                {
                    Console.WriteLine("Matching Files");
                    return 0;
                }
            }
            ShowUsage();
            return 0;
        }
    }
}
