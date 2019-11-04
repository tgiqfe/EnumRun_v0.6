using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;
using System.Diagnostics;
using System.IO.Compression;

namespace Manifest
{
    //  V0.01.003
    class Program
    {
        const string PROJECT_NAME = "EnumRun";

        static void Main(string[] args)
        {
            Environment.CurrentDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            string debugDir = string.Format(@"..\..\..\{0}\bin\Debug", PROJECT_NAME);
            string releaseDir = string.Format(@"..\..\..\{0}\bin\Release", PROJECT_NAME);
            string moduleDir = string.Format(@"..\..\..\{0}\bin\{0}", PROJECT_NAME);
            string moduleZip = string.Format(@"..\..\..\{0}\bin\{0}.zip", PROJECT_NAME);

            PSD1.Create(PROJECT_NAME, debugDir);
            PSD1.Create(PROJECT_NAME, releaseDir);
            PSM1.Create(PROJECT_NAME, debugDir);
            PSM1.Create(PROJECT_NAME, releaseDir);

            //  Releaseフォルダーを公開用にコピー
            if (Directory.Exists(releaseDir))
            {
                using (Process proc = new Process())
                {
                    proc.StartInfo.FileName = "robocopy.exe";
                    proc.StartInfo.Arguments = string.Format(
                        "\"{0}\" \"{1}\" /COPY:DAT /MIR /E /XJD /XJF", releaseDir, moduleDir);
                    proc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                    proc.Start();
                    proc.WaitForExit();
                }
            }

            //  モジュールフォルダーをZipアーカイブ
            if (Directory.Exists(moduleDir))
            {
                if (File.Exists(moduleZip))
                {
                    File.Delete(moduleZip);
                }
                ZipFile.CreateFromDirectory(moduleDir, moduleZip, CompressionLevel.NoCompression, false);
            }
        }
    }
}
