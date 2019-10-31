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
    [Cmdlet(VerbsCommon.Add, "EnumRunRange")]
    public class AddEnumRunRange : PSCmdlet
    {
        [Parameter(ValueFromPipeline = true)]
        public Range[] Range { get; set; }
        [Parameter(Position = 0)]
        public string Name { get; set; }
        [Parameter(Position = 1)]
        public int StartNumber { get; set; }
        [Parameter(Position = 2)]
        public int EndNumber { get; set; }
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
                Range[] ranges = _setting.GetRange(Name);
                if (ranges != null && ranges.Length > 0)
                {
                    //  すでに同じ名前のRangeがある為、追加不可
                    return;
                }
                else
                {
                    //  名前を指定留守場合は1つずつ追加
                    _setting.Ranges.Add(new Range()
                    {
                        Name = this.Name,
                        StartNumber = this.StartNumber,
                        EndNumber = this.EndNumber
                    });
                }
            }
            else if (Range != null)
            {
                foreach (Range range in Range)
                {
                    if (_setting.Ranges.Any(x => x.Name.Equals(range.Name, StringComparison.OrdinalIgnoreCase)))
                    {
                        //  すでに同じ名前のRangeがある為、追加不可
                        return;
                    }
                    else
                    {
                        _setting.Ranges.Add(range);
                    }
                }
            }
            DataSerializer.Serialize<EnumRunSetting>(_setting, SettingPath);

            WriteObject(_setting);
        }
    }
}
