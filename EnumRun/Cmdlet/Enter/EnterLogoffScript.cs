using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management.Automation;
using System.IO;
using EnumRun.Serialize;

namespace EnumRun.Cmdlet
{
    [Cmdlet(VerbsCommon.Enter, "LogoffScript")]
    public class EnterLogoffScript : PSCmdlet
    {
        const string ProcessName = "LogoffScript";

        [Parameter(Position = 0), Alias("Path")]
        public string SettingPath { get; set; }

        private EnumRunSetting _setting = null;
        private DateTime _startTime;

        protected override void BeginProcessing()
        {
            if (string.IsNullOrEmpty(SettingPath))
            {
                SettingPath = File.Exists(Item.CURRENT_DIR_SETTING) ? Item.CURRENT_DIR_SETTING : Item.PROGRAMDATA_SETTING;
            }
            _setting = DataSerializer.Deserialize<EnumRunSetting>(SettingPath);

            _startTime = DateTime.Now;

            //  ログ出力設定のセット
            Item.Logger = Function.SetLogger(_setting.LogsPath, ProcessName, _setting.DebugMode);
            if (_setting.DebugMode) { Item.Logger.Debug(Function.GetCmdletName(this.GetType().Name)); }
        }

        protected override void ProcessRecord()
        {
            if (_setting.RunOnce && !BootAndLogonSession.Check(ProcessName))
            {
                Item.Logger.Warn("RunOnce true: 1回実行済みの為、終了");
                return;
            }

            Item.Logger.Debug("{0}: 開始", ProcessName);

            Range range = _setting.Ranges.FirstOrDefault(x => x.Name.Equals(ProcessName, StringComparison.OrdinalIgnoreCase));
            if (range != null)
            {
                if (Directory.Exists(_setting.FilesPath))
                {
                    //  スクリプトファイルの列挙
                    List<Script> scriptList = new List<Script>();
                    foreach (string scriptFile in Directory.GetFiles(_setting.FilesPath))
                    {
                        Script script = new Script(scriptFile, range, _setting);
                        if (script.Enabled)
                        {
                            script.Process();
                        }
                    }
                }
            }

            Item.Logger.Debug("所要時間(ミリ秒):{0}", (DateTime.Now - _startTime).Milliseconds);
            Item.Logger.Debug("{0}: 終了", ProcessName);
        }
    }
}
