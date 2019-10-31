using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Diagnostics;

namespace EnumRun
{
    public class Script
    {
        private Language _Lang = null;

        public bool Enabled { get; set; }
        public string Name { get; set; }
        public string File { get; set; }
        public string Args { get; set; }
        public string Lang
        {
            get
            {
                if (this._Lang == null && Item.Setting != null && !string.IsNullOrEmpty(this.File))
                {
                    string extension = Path.GetExtension(File);
                    this._Lang = Item.Setting.Languages.FirstOrDefault(x =>
                        x.Extensions.Any(y =>
                            y.Equals(extension, StringComparison.OrdinalIgnoreCase)));
                }
                return _Lang == null ? "" : _Lang.ToString();
            }
            set
            {
                this._Lang = Item.Setting.Languages.FirstOrDefault(x =>
                    x.Name.Equals(value, StringComparison.OrdinalIgnoreCase));
            }
        }
        public EnumRunOption Option { get; set; }
        public int BeforeTime { get; set; }
        public int AfterTime { get; set; }

        public Script() { }
        public Script(string scriptFile, int startNum, int endNum)
        {
            this.Name = Path.GetFileName(scriptFile);
            this.File = scriptFile;

            //  実行可否確認
            this.Enabled = CheckEnable(startNum, endNum);
            if (!Enabled) { return; }

            //  実行時オプション確認
            DetectOption();
        }

        /// <summary>
        /// 実行可否を確認
        /// </summary>
        private bool CheckEnable(int startNum, int endNum)
        {
            Match tempMatch;
            if ((tempMatch = Regex.Match(Name, @"^\d+(?=_)")).Success)
            {
                //  ファイル名の先頭が数字かどうか
                int fileNumber = int.Parse(tempMatch.Value);
                if (fileNumber < startNum || fileNumber > endNum)
                {
                    return false;
                }

                //  拡張子が事前登録している言語のものかどうか
                return !string.IsNullOrEmpty(this.Lang);
            }
            return false;
        }

        /// <summary>
        /// ファイル名末尾から実行時オプションを解析
        /// </summary>
        private void DetectOption()
        {
            Match tempMatch;
            if ((tempMatch = Regex.Match(Name, @"(\[[0-9a-zA-Z]+\])+(?=\..+$)")).Success)
            {
                string matchString = tempMatch.Value;

                //  実行時オプション
                if (matchString.Contains("n"))
                {
                    Option |= EnumRunOption.NoRun;
                    return;
                }
                if (matchString.Contains("w")) { Option |= EnumRunOption.WaitForExit; }
                if (matchString.Contains("a")) { Option |= EnumRunOption.RunAsAdmin; }
                if (matchString.Contains("d")) { Option |= EnumRunOption.DomainUserOnly; }
                if (matchString.Contains("l")) { Option |= EnumRunOption.LocalUserOnly; }
                if (matchString.Contains("s")) { Option |= EnumRunOption.SystemAccountOnly; }
                if (matchString.Contains("p")) { Option |= EnumRunOption.DGReachableOnly; }
                if (matchString.Contains("t")) { Option |= EnumRunOption.TrustedOnly; }
                if (matchString.Contains("k")) { Option |= EnumRunOption.WorkgroupPCOnly; }
                if (matchString.Contains("m")) { Option |= EnumRunOption.DomainPCOnly; }
                if (matchString.Contains("o")) { Option |= EnumRunOption.Output; }

                //  実行前/実行後待機時間
                Match match;
                if ((match = Regex.Match(matchString, @"\d{1,3}(?=r)")).Success)
                {
                    this.BeforeTime = int.Parse(match.Value);
                    Option |= EnumRunOption.BeforeWait;
                }
                if ((match = Regex.Match(matchString, @"(?<=r)\d{1,3}")).Success)
                {
                    this.AfterTime = int.Parse(match.Value);
                    Option |= EnumRunOption.AfterWait;
                    Option |= EnumRunOption.WaitForExit;
                }
            }
        }

        /// <summary>
        /// 対象のオプションが含まれているかどうか
        /// </summary>
        /// <param name="targetOption"></param>
        /// <returns></returns>
        private bool CheckOption(EnumRunOption targetOption)
        {
            return (Option & targetOption) == targetOption;
        }

        /// <summary>
        /// プロセス実行用タスク
        /// </summary>
        public void Process()
        {
            //  実行対象外
            if (CheckOption(EnumRunOption.NoRun))
            {
                Item.Logger.Info("{0} / n:{1} 実行対象外", Name, true);
                return;
            }

            //  ドメイン参加済みPCのみ ro ワークグループPCのみ
            if (
                (CheckOption(EnumRunOption.DomainPCOnly) && !CheckOption(EnumRunOption.WorkgroupPCOnly) && !Function.IsDomainMachine()) ||
                (!CheckOption(EnumRunOption.DomainPCOnly) && CheckOption(EnumRunOption.WorkgroupPCOnly) && Function.IsDomainMachine()))
            {
                Item.Logger.Info("{0} / m:{1} k:{2} ドメイン参加⇒{3}",
                    Name,
                    CheckOption(EnumRunOption.DomainPCOnly),
                    CheckOption(EnumRunOption.WorkgroupPCOnly),
                    Function.IsDomainMachine());
                return;
            }

            //  システムアカウントのみ
            if (CheckOption(EnumRunOption.SystemAccountOnly) && !Function.IsSystemAccount())
            {
                Item.Logger.Info("{0} / s:{1} システムアカウントのみ", Name, true);
                return;
            }

            //  ドメインユーザーのみ or ローカルユーザーのみ
            if ((CheckOption(EnumRunOption.DomainUserOnly) && !CheckOption(EnumRunOption.LocalUserOnly) && !Function.IsDomainUser()) ||
                (!CheckOption(EnumRunOption.DomainUserOnly) && CheckOption(EnumRunOption.LocalUserOnly) && Function.IsDomainUser()))
            {
                Item.Logger.Info("{0} / d:{1} l:{2} ドメインユーザー⇒{3}",
                    Name,
                    CheckOption(EnumRunOption.DomainUserOnly),
                    CheckOption(EnumRunOption.LocalUserOnly),
                    Function.IsDomainUser());
                return;
            }

            //  デフォルトゲートウェイとの通信可否を確認
            if (CheckOption(EnumRunOption.DGReachableOnly) && !Function.IsDefaultGatewayReachable())
            {
                Item.Logger.Info("{0} / p:{1} DG導通⇒{2}", Name, true, false);
                return;
            }

            //  管理者として実行しているかどうかの確認
            if (CheckOption(EnumRunOption.TrustedOnly) && !Function.IsRunAdministrator())
            {
                Item.Logger.Info("{0} / t:{1} 管理者実行⇒{2}", Name, true, false);
                return;
            }

            //  実行前待機
            if (BeforeTime > 0)
            {
                Item.Logger.Info("{0} / 〇r:{1} 実行前待機", Name, BeforeTime);
                Thread.Sleep(BeforeTime * 1000);
            }

            //  プロセス開始
            Item.Logger.Info("{0} 実行", Name);
            Task task = CheckOption(EnumRunOption.Output) ?
                ProcessThreadAndOutput() :
                ProcessThread();
            if (CheckOption(EnumRunOption.WaitForExit))
            {
                Item.Logger.Info("{0} / w:{1} 終了待ち", Name, true);
                task.Wait();
            }

            //  実行後待機
            if (AfterTime > 0)
            {
                Item.Logger.Info("{0} / r〇:{1} 実行後待機", Name, AfterTime);
                Thread.Sleep(AfterTime * 1000);
            }
        }

        /// <summary>
        /// プロセス実行するメソッド
        /// </summary>
        /// <returns></returns>
        private async Task ProcessThread()
        {
            await Task.Run(() =>
            {
                using (Process proc = _Lang.GetProcess(File, Args))
                {
                    proc.StartInfo.Verb = CheckOption(EnumRunOption.RunAsAdmin) ? "RunAs" : "";
                    proc.StartInfo.CreateNoWindow = true;
                    proc.StartInfo.UseShellExecute = false;
                    proc.Start();
                    proc.WaitForExit();
                    //proc.ExitCode;    ←戻り値の扱いを検討中
                }
            });
        }
        private async Task ProcessThreadAndOutput()
        {
            string outputFile =
                Function.CreateOutputFileName(Item.Setting.OutputPath, Path.GetFileNameWithoutExtension(File));
            Item.Logger.Info("{0} / o:{1} 標準出力リダイレクト先⇒{2}", Name, true, outputFile);

            if (!Directory.Exists(Item.Setting.OutputPath))
            {
                Directory.CreateDirectory(Item.Setting.OutputPath);
            }
            await Task.Run(() =>
            {
                using (Process proc = _Lang.GetProcess(File, Args))
                using (StreamWriter sw = new StreamWriter(outputFile, true, Encoding.GetEncoding("Shift_JIS")))
                {
                    proc.StartInfo.Verb = CheckOption(EnumRunOption.RunAsAdmin) ? "RunAs" : "";
                    proc.StartInfo.CreateNoWindow = true;
                    proc.StartInfo.UseShellExecute = false;
                    proc.StartInfo.RedirectStandardOutput = true;
                    proc.StartInfo.RedirectStandardError = true;
                    proc.StartInfo.RedirectStandardInput = false;
                    proc.OutputDataReceived += (sender, e) => { sw.WriteLine(e.Data); };
                    proc.ErrorDataReceived += (sender, e) => { sw.WriteLine(e.Data); };
                    proc.Start();
                    proc.BeginOutputReadLine();
                    proc.BeginErrorReadLine();
                    proc.WaitForExit();
                    //proc.ExitCode;    ←戻り値の扱いを検討中
                }
            });
        }
    }
}
