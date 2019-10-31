using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management.Automation;

namespace EnumRun.Cmdlet
{
    [Cmdlet(VerbsCommon.Remove, "EnumRunLanguage")]
    public class RemoveEnumRunLanguage : PSCmdlet
    {
        [Parameter(ValueFromPipeline = true)]
        public Language[] Language { get; set; }
        [Parameter(Position = 0)]
        public string Name { get; set; }
        [Parameter]
        public string Path { get; set; }

        protected override void BeginProcessing()
        {
            Item.Setting = EnumRunSetting.Load(Path);
        }

        protected override void ProcessRecord()
        {
            if (Language == null && !string.IsNullOrEmpty(Name))
            {
                foreach (Language lang in Item.Setting.GetLanguage(Name))
                {
                    Item.Setting.Languages.Remove(lang);
                }
            }
            else if (Language != null)
            {
                //  名前判定せず、インスタンスの中身が一致したら削除
                foreach (Language lang in Language)
                {
                    Item.Setting.Languages.Remove(lang);
                }
            }
            Item.Setting.Save(Path);
        }
    }
}
