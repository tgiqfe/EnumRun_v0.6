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
    [Cmdlet(VerbsCommon.Remove, "EnumRunRange")]
    public class RemoveEnumRunRange : PSCmdlet
    {
        [Parameter(ValueFromPipeline = true)]
        public Range[] Range { get; set; }
        [Parameter(Position = 0)]
        public string Name { get; set; }
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
            if (Range == null && !string.IsNullOrEmpty(Name))
            {
                foreach(Range range in _setting.GetRange(Name))
                {
                    _setting.Ranges.Remove(range);
                }
            }
            else if (Range != null)
            {
                //  名前判定せず、インスタンスの中身が一致したら削除
                foreach(Range range in Range)
                {
                    _setting.Ranges.Remove(range);
                }
            }
            DataSerializer.Serialize<EnumRunSetting>(_setting, SettingPath);
        }
    }
}
