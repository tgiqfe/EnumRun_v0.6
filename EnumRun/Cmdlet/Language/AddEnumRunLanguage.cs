using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management.Automation;

namespace EnumRun.Cmdlet
{
    [Cmdlet(VerbsCommon.Add, "EnumRunLanguage")]
    public class AddEnumRunLanguage : PSCmdlet
    {
        [Parameter(ValueFromPipeline = true)]
        public Language[] Language { get; set; }
        [Parameter(Position = 0)]
        public string Name { get; set; }
        [Parameter]
        public string[] Extensions { get; set; }
        [Parameter]
        public string Command { get; set; }
        [Parameter]
        public string Command_x86 { get; set; }
        [Parameter]
        public string ArgsPrefix { get; set; }
        [Parameter]
        public string ArgsMidWithoutArgs { get; set; }
        [Parameter]
        public string ArgsMidWithArgs { get; set; }
        [Parameter]
        public string ArgsSuffix { get; set; }
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
                Language[] langs = Item.Setting.GetLanguage(Name);
                if (langs != null && langs.Length > 0)
                {
                    //  すでに同じ名前のLanguageがある為、追加不可
                    return;
                }
                else
                {
                    //  名前を指定の場合は1つずつ追加
                    Item.Setting.Languages.Add(new Language()
                    {
                        Name = this.Name,
                        Extensions = this.Extensions,
                        Command = this.Command,
                        Command_x86 = this.Command_x86,
                        ArgsPrefix = this.ArgsPrefix,
                        ArgsMidWithoutArgs = this.ArgsMidWithoutArgs,
                        ArgsMidWithArgs = this.ArgsMidWithArgs,
                        ArgsSuffix = this.ArgsSuffix
                    });
                }
            }
            else if (Language != null)
            {
                foreach (Language lang in Language)
                {
                    if(Item.Setting.Languages.Any(x => x.Name.Equals(lang.Name, StringComparison.OrdinalIgnoreCase)))
                    {
                        //  すでに同じ名前のLanguageがある為、追加不可
                        return;
                    }
                    else
                    {
                        Item.Setting.Languages.Add(lang);
                    }
                }
            }
            Item.Setting.Save(Path);
        }
    }
}
