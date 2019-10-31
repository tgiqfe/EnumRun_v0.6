using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management;
using System.IO;
using EnumRun.Serialize;

namespace EnumRun
{
    public class BootAndLogonSession
    {
        public string ProcessName { get; set; }
        public string BootUpTime { get; set; }
        public List<string> LogonIdList { get; set; }

        public BootAndLogonSession() { }
        public BootAndLogonSession(string processName)
        {
            this.ProcessName = processName;

            //  起動時間
            foreach (ManagementObject mo in new ManagementClass("Win32_OperatingSystem").
                GetInstances().
                OfType<ManagementObject>())
            {
                this.BootUpTime = mo["LastBootUpTime"] as string;
            }

            //  ログオン時間
            this.LogonIdList = new List<string>();
            foreach (ManagementObject mo in new ManagementClass("Win32_LoggedOnUser").
                GetInstances().
                OfType<ManagementObject>())
            {
                //  Win32_Account
                ManagementObject moA = new ManagementObject(mo["Antecedent"] as string);
                if (moA["Name"] as string == Environment.UserName)
                {
                    //  Win32_LogonSession
                    ManagementObject moB = new ManagementObject(mo["Dependent"] as string);
                    this.LogonIdList.Add(moB["LogonId"] as string);
                }
            }
            LogonIdList.Sort();
        }

        /// <summary>
        /// 現在実行している同名のプロセスが1回目かどうかを判定
        /// </summary>
        /// <param name="processName"></param>
        /// <returns>1回目であればtrue</returns>
        public static bool Check(string processName)
        {
            bool retVal = false;

            //  現在のセッション情報
            BootAndLogonSession session = new BootAndLogonSession(processName);

            //  前回セッション情報を確認
            string tempDir = Path.Combine(
                Environment.ExpandEnvironmentVariables("%TEMP%"), Item.APPLICATION_NAME);
            if (!Directory.Exists(tempDir))
            {
                Directory.CreateDirectory(tempDir);
            }
            string sessionFile = Path.Combine(tempDir, Item.SESSION_FILE);

            Dictionary<string, BootAndLogonSession> sessionData =
                DataSerializer.Deserialize<Dictionary<string, BootAndLogonSession>>(sessionFile);
            if (sessionData == null)
            {
                sessionData = new Dictionary<string, BootAndLogonSession>();
            }
            if (sessionData.ContainsKey(processName))
            {
                retVal = sessionData[processName].BootUpTime != session.BootUpTime &&
                    !sessionData[processName].LogonIdList.SequenceEqual(session.LogonIdList);
            }
            else
            {
                retVal = true;
            }

            //  現在のセッションを保存
            sessionData[processName] = session;
            DataSerializer.Serialize<Dictionary<string, BootAndLogonSession>>(sessionData, sessionFile);

            return retVal;
        }
    }
}
