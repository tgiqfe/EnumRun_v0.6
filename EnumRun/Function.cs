using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.IO;
using System.Management;
using System.Security.Principal;
using System.Net.NetworkInformation;
using System.Threading;
using System.Security.Cryptography;
using NLog;
using NLog.Config;
using NLog.Targets;

namespace EnumRun
{
    class Function
    {
        /// <summary>
        /// ログ出力設定
        /// </summary>
        /// <param name="processName"></param>
        /// <returns></returns>
        public static Logger SetLogger(string processName)
        {
            string logPath = System.IO.Path.Combine(
                Item.Setting.LogsPath,
                string.Format("{0}_{1}.log", processName, DateTime.Now.ToString("yyyyMMdd")));

            //  ファイル出力先設定
            FileTarget file = new FileTarget("File");
            file.Encoding = Encoding.GetEncoding("Shift_JIS");
            file.Layout = "[${longdate}][${windows-identity}][${uppercase:${level}}] ${message}";
            file.FileName = logPath;

            //  コンソール出力設定
            ConsoleTarget console = new ConsoleTarget("Console");
            console.Layout = "[${longdate}][${windows-identity}][${uppercase:${level}}] ${message}";

            LoggingConfiguration conf = new LoggingConfiguration();
            conf.AddTarget(file);
            conf.AddTarget(console);
            if (Item.Setting.DebugMode)
            {
                conf.LoggingRules.Add(new LoggingRule("*", LogLevel.Debug, file));
                conf.LoggingRules.Add(new LoggingRule("*", LogLevel.Debug, console));
            }
            else
            {
                conf.LoggingRules.Add(new LoggingRule("*", LogLevel.Info, file));
            }
            LogManager.Configuration = conf;
            Logger logger = LogManager.GetCurrentClassLogger();

            return logger;
        }

        /// <summary>
        /// Active Directoryドメインの名前を取得
        /// </summary>
        private static string _domainName = null;
        private static string GetDomainName()
        {
            if (_domainName == null)
            {
                ManagementObject mo = new ManagementClass("Win32_ComputerSystem").
                    GetInstances().
                    OfType<ManagementObject>().
                    FirstOrDefault(x => (bool)x["PartOfDomain"]);
                _domainName = mo == null ? "" : mo["Domain"] as string;
            }
            return _domainName;
        }

        /// <summary>
        /// 実行中PCがドメイン参加済みかどうか
        /// </summary>
        /// <returns>ドメイン参加済みであればtrue</returns>
        public static bool IsDomainMachine()
        {
            return !string.IsNullOrEmpty(GetDomainName());
        }

        /// <summary>
        /// システムアカウントかどうか
        /// 「System」「LocalService」「Network Service」「ホスト名$」が該当
        /// </summary>
        /// <returns>システムアカウントであればtrue</returns>
        public static bool IsSystemAccount()
        {
            return new string[] { "System", "Local Service", "Network Service", Environment.MachineName + "$" }.
                Any(x => x.Equals(Environment.UserName, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// ドメインユーザーかどうか
        /// </summary>
        /// <returns>ドメインユーザーであればtrue</returns>
        public static bool IsDomainUser()
        {
            return !IsSystemAccount() && IsDomainMachine() && (Environment.UserDomainName != Environment.MachineName);
        }

        /// <summary>
        /// デフォルトゲートウェイへの導通可否確認
        /// </summary>
        /// <returns>導通可の場合true</returns>
        public static bool IsDefaultGatewayReachable()
        {
            int count = 4;
            int interval = 500;
            Ping ping = new Ping();
            foreach (NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces())
            {
                foreach (GatewayIPAddressInformation gw in nic.GetIPProperties().GatewayAddresses)
                {
                    for (int i = 0; i < count; i++)
                    {
                        PingReply reply = ping.Send(gw.Address);
                        if (reply.Status == IPStatus.Success)
                        {
                            return true;
                        }
                        Thread.Sleep(interval);
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// 管理者実行しているかどうかの確認
        /// </summary>
        /// <returns>管理者として実行しているのならばtrue</returns>
        public static bool IsRunAdministrator()
        {
            WindowsIdentity id = WindowsIdentity.GetCurrent();
            WindowsPrincipal principal = new WindowsPrincipal(id);
            bool isAdmin = principal.IsInRole(WindowsBuiltInRole.Administrator);
            return isAdmin;
        }

        /// <summary>
        /// 標準出力表示の出力先ファイルを取得
        /// </summary>
        /// <param name="outputDir">出力先フォルダー</param>
        /// <param name="solt"></param>
        /// <returns></returns>
        public static string CreateOutputFileName(string outputDir, string solt)
        {
            string sourceText = solt + "_" + DateTime.Now.ToString("yyyyMMddHHmmss");
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            byte[] bytes = md5.ComputeHash(Encoding.UTF8.GetBytes(sourceText));
            md5.Clear();
            return Path.Combine(
                outputDir,
                sourceText + "_" + BitConverter.ToString(bytes).Replace("-", "") + ".txt");
        }
    }
}
