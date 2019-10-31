using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management.Automation;
using EnumRun.Serialize;
using System.IO;

namespace EnumRun.Cmdlet
{
    [Cmdlet(VerbsCommon.Get, "EnumRunSetting")]
    public class GetEnumRunSetting : PSCmdlet
    {
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
        }

        protected override void ProcessRecord()
        {
            WriteObject(_setting);
        }
    }
}
