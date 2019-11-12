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
    [Cmdlet(VerbsCommon.Remove, "EnumRunLanguage")]
    public class RemoveEnumRunLanguage : PSCmdlet
    {
        [Parameter(ValueFromPipeline = true)]
        public EnumRun.Language.Language[] Language { get; set; }
        [Parameter(Position = 0)]
        public string Name { get; set; }
        [Parameter(Position = 0), Alias("Path")]
        public string SettingPath { get; set; }

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
            if (Language == null && !string.IsNullOrEmpty(Name))
            {
                foreach (EnumRun.Language.Language lang in _setting.GetLanguage(Name))
                {
                    _setting.Languages.Remove(lang);
                }
            }
            else if (Language != null)
            {
                //  名前判定せず、インスタンスの中身が一致したら削除
                foreach (EnumRun.Language.Language lang in Language)
                {
                    _setting.Languages.Remove(lang);
                }
            }
            DataSerializer.Serialize<EnumRunSetting>(_setting, SettingPath);
        }
    }
}
