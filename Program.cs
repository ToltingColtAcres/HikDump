using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using HikNAS;

namespace HikDump
{
    class Program
    {
        //
        // HikDump - demo app which uses HikNAS Class library to retrieve file indexes, sorts by recording date, and dumps as a csv
        //           to either stdout or a file specified by the user.
        //
        // To run from the command-line, type:
        // "c:\program files\dotnet\dotnet.exe" hikdump.dll x:\path-to-nas-storage [y:\optional-output-file-for-csv-output]
        //
        static void Main(string[] args) 
        {
            String s_inputpath = String.Empty;
            String s_outputfile = String.Empty;

            List<HikFile> a_files = null;

            switch (args.Length)
            {
                case 1:
                    s_inputpath = args[0];
                    break;
                case 2:
                    s_inputpath = args[0];
                    s_outputfile = args[1];
                    break;
            }

            if (String.IsNullOrEmpty(s_inputpath) || !Directory.Exists(s_inputpath) || ! File.Exists(s_inputpath + "\\info.bin"))
            {
                Console.WriteLine("Usage: HikDump path-to-nas-storage [output file name]");
                return;
            }

            try
            {
                HikInfo nas = new HikInfo(s_inputpath);

                a_files = new List<HikFile>();

                foreach (HikIndex index in nas.a_index)
                {
                    a_files.AddRange(index.a_file);
                }

                a_files = a_files.OrderBy(o => o.dt_startTime).ToList();

                using (StreamWriter sw = new StreamWriter((String.IsNullOrEmpty(s_outputfile) ? Console.OpenStandardOutput() : new FileStream(s_outputfile, FileMode.Create, FileAccess.Write))))
                {
                    sw.Write(a_files.ToCSV());
                }
            }
            catch (HikException hikException)
            {
                Console.WriteLine(hikException.Message);
            }
            catch (Exception eX)
            {
                Console.WriteLine("General Exception Error:");
                Console.WriteLine(eX.Message);
                Console.WriteLine(eX.Source);
                Console.WriteLine(eX.StackTrace);
            }

        }
    }
}