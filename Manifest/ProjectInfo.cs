using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manifest
{
    public enum Mode { Debug, Release }

    public class ProjectInfo
    {
        //  デバッグ/リリースのモード指定
        public Mode Mode { get; set; }

        public string ProjectName { get; set; } = "EnumRun";

        public string TargetDir { get { return string.Format(@"..\..\..\{0}\bin\{1}", ProjectName, Mode); } }
        public string ModuleDir { get { return string.Format(@"..\..\..\{0}\bin\{0}", ProjectName); } }
        public string ModuleZip { get { return string.Format(@"..\..\..\{0}\bin\{0}.zip", ProjectName); } }
        public string ScriptDir { get { return string.Format(@"..\..\..\{0}\Script", ProjectName); } }
        public string FormatDir { get { return string.Format(@"..\..\..\{0}\Format", ProjectName); } }
        public string CmdletDir { get { return string.Format(@"..\..\..\{0}\Cmdlet", ProjectName); } }

        public string DllFile { get { return string.Format(@"..\..\..\{0}\bin\{1}\{0}.dll", ProjectName, Mode); } }
        public string Psd1File { get { return string.Format(@"..\..\..\{0}\bin\{1}\{0}.psd1", ProjectName, Mode); } }
        public string Psm1File { get { return string.Format(@"..\..\..\{0}\bin\{1}\{0}.psm1", ProjectName, Mode); } }

        public string Description { get; set; } = "Run enumerated script";
        public string Author { get; set; } = "q";
        public string CompanyName { get; set; } = "q";
        public string Copyright { get; set; } = null;

        public ProjectInfo() { }
    }
}
