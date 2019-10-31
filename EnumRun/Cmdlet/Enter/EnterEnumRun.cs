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
    [Cmdlet(VerbsCommon.Enter, "EnumRun")]
    public class EnterEnumRun : PSCmdlet
    {
        [Parameter(Mandatory = true, Position = 0)]
        public string ProcessName { get; set; }
        [Parameter(Position = 0), Alias("Path")]
        public string SettingPath { get; set; }

        private EnumRunSetting _setting = null;

        protected override void BeginProcessing()
        {
            if (string.IsNullOrEmpty(SettingPath))
            {
                string currentDirSetting = Path.Combine(Item.CURRENT_DIR, Item.CONFIG_JSON);
                string programdataSetting = Path.Combine(Item.DEFAULT_WORKDIR, Item.CONFIG_JSON);
                SettingPath = File.Exists(currentDirSetting) ? currentDirSetting : programdataSetting;
            }
            _setting = DataSerializer.Deserialize<EnumRunSetting>(SettingPath);

            Item.StartTime = DateTime.Now;
            Item.Logger = Function.SetLogger(ProcessName);

            //  パラメータをItemに格納
            Item.Setting = _setting;
        }

        protected override void ProcessRecord()
        {
            if (Item.Setting.RunOnce && !BootAndLogonSession.Check(ProcessName))
            {
                Item.Logger.Warn("RunOnce:true 2回目以降の為、終了");
                return;
            }

            Item.Logger.Debug("開始 {0}", ProcessName);

            Range range = Item.Setting.Ranges.FirstOrDefault(x => x.Name.Equals(ProcessName, StringComparison.OrdinalIgnoreCase));
            if (range != null)
            {
                if (Directory.Exists(Item.Setting.FilesPath))
                {
                    //  スクリプトファイルの列挙
                    List<Script> scriptList = new List<Script>();
                    foreach (string scriptFile in Directory.GetFiles(Item.Setting.FilesPath))
                    {
                        Script script = new Script(scriptFile, range.StartNumber, range.EndNumber);
                        if (script.Enabled)
                        {
                            script.Process();
                        }
                    }
                }
            }

            Item.Logger.Debug("所要時間(ミリ秒):{0}", (DateTime.Now - Item.StartTime).Milliseconds);
            Item.Logger.Debug("終了 {0}", ProcessName);
        }

        protected override void EndProcessing()
        {
            Item.Setting = null;
        }
    }
}
