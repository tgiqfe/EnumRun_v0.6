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
    [Cmdlet(VerbsCommon.Set, "EnumRunRange")]
    public class SetEnumRunRange : PSCmdlet
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
        [Parameter]
        public SwitchParameter Clear { get; set; }

        private EnumRunSetting _setting = null;

        protected override void BeginProcessing()
        {
            if (string.IsNullOrEmpty(SettingPath))
            {
                SettingPath = File.Exists(Item.CURRENT_DIR_SETTING) ? Item.CURRENT_DIR_SETTING : Item.PROGRAMDATA_SETTING;
            }
            _setting = DataSerializer.Deserialize<EnumRunSetting>(SettingPath);
        }

        protected override void ProcessRecord()
        {
            //  ClearもしくはEnumRunSettingがnullの場合、デフォルト設定で再作成
            if (_setting == null || Clear)
            {
                _setting = new EnumRunSetting();
                _setting.SetDefaultParameter();
            }

            if (Range == null && !string.IsNullOrEmpty(Name))
            {
                Range[] ranges = _setting.GetRange(Name);
                if(ranges != null && ranges.Length > 0)
                {
                    foreach(Range range in ranges)
                    {
                        range.StartNumber = StartNumber;
                        range.EndNumber = EndNumber;
                    }
                }
                else
                {
                    //  存在しない場合は何もしない
                    return;
                }
            }
            else if(Range != null)
            {
                //  パイプラインで渡されたRangeと名前が一致している場合に上書き
                foreach(Range range in Range)
                {
                    int index = _setting.Ranges.FindIndex(x => x.Name.Equals(range.Name, StringComparison.OrdinalIgnoreCase));
                    if(index >= 0)
                    {
                        _setting.Ranges[index] = range;
                    }
                    else
                    {
                        //  存在しない場合は何もしない
                        return;
                    }
                }
            }
            DataSerializer.Serialize<EnumRunSetting>(_setting, SettingPath);

            WriteObject(_setting.Ranges, true);
        }
    }
}
