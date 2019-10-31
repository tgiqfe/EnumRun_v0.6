using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnumRun
{
    /// <summary>
    /// DefaultのRange設定値を取得
    /// </summary>
    class DefaultRangeSettings
    {
        public static List<Range> Create()
        {
            string startupScript = "StartupScript";
            string logonScript = "LogonScript";
            string logoffScript = "LogoffScript";
            string shutdownScript = "ShutdownScript";

            List<Range> list = new List<Range>();

            list.Add(new Range()
            {
                Name = startupScript,
                StartNumber = 1,
                EndNumber = 9
            });
            list.Add(new Range()
            {
                Name = logonScript,
                StartNumber = 11,
                EndNumber = 29
            });
            list.Add(new Range()
            {
                Name = logoffScript,
                StartNumber = 81,
                EndNumber = 89
            });
            list.Add(new Range()
            {
                Name = shutdownScript,
                StartNumber = 91,
                EndNumber = 99,
            });
            return list;
        }
    }
}
