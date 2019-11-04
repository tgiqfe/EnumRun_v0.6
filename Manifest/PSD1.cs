﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;
using System.Reflection;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Manifest
{
    //  V0.01.002
    class PSD1
    {
        const string EXTENSION = ".psd1";

        public static void Create(string projectName, string outputDir)
        {
            string dllFile = Path.Combine(outputDir, projectName + ".dll");
            string outputFile = Path.Combine(outputDir, projectName + EXTENSION);
            if (!File.Exists(dllFile)) { return; }

            string dllFile_absolute = Path.GetFullPath(dllFile);

            //  Cmdletを探してセット
            List<string> CmdletsToExportList = new List<string>();
            string cmdletDir = @"..\..\..\" + projectName + @"\Cmdlet";
            foreach (string csFile in Directory.GetFiles(cmdletDir, "*.cs", SearchOption.AllDirectories))
            {
                using (StreamReader sr = new StreamReader(csFile, Encoding.UTF8))
                {
                    string readLine = "";
                    while ((readLine = sr.ReadLine()) != null)
                    {
                        if (Regex.IsMatch(readLine, @"^\s*\[Cmdlet\(Verbs"))
                        {
                            string cmdPre = readLine.Substring(
                                readLine.IndexOf(".") + 1, readLine.IndexOf(",") - readLine.IndexOf(".") - 1);
                            string cmdSuf = readLine.Substring(
                                readLine.IndexOf("\"") + 1, readLine.LastIndexOf("\"") - readLine.IndexOf("\"") - 1);
                            CmdletsToExportList.Add(cmdPre + "-" + cmdSuf);
                        }
                    }
                }
            }

            //  Format.ps1xmlを探してセット
            List<string> FormatsToProcessList = new List<string>();
            string formatDir = string.Format(@"..\..\..\{0}\Format", projectName);
            foreach (string formatFile in Directory.GetFiles(formatDir, "*.ps1xml"))
            {
                FormatsToProcessList.Add(Path.GetFileName(formatFile));
            }

            //  バージョン取得
            FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(dllFile);

            //  GUID取得
            GuidAttribute attr =
                Attribute.GetCustomAttribute(Assembly.LoadFile(dllFile_absolute), typeof(GuidAttribute)) as GuidAttribute;

            string RootModule = Path.GetFileName(dllFile);
            string ModuleVersion = fvi.FileVersion;
            string Guid = attr.Value;
            string Author = "q";
            string CompanyName = "q";
            string Copyright = fvi.LegalCopyright;
            string Description = "Run enumerated script";

            string manifestString = string.Format(@"@{{
RootModule = ""{0}""
ModuleVersion = ""{1}""
GUID = ""{2}""
Author = ""{3}""
CompanyName = ""{4}""
Copyright = ""{5}""
Description = ""{6}""
CmdletsToExport = @(
  ""{7}""
)
FormatsToProcess = @(
  ""{8}""
)
}}",
RootModule, ModuleVersion, Guid, Author, CompanyName, Copyright, Description,
string.Join("\",\r\n  \"", CmdletsToExportList),
string.Join("\",\r\n  \"", FormatsToProcessList)
);
            using (StreamWriter sw = new StreamWriter(outputFile, false, Encoding.UTF8))
            {
                sw.WriteLine(manifestString);
            }
        }
    }
}
