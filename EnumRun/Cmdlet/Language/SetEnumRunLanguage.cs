using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management.Automation;

namespace EnumRun.Cmdlet
{
    [Cmdlet(VerbsCommon.Set, "EnumRunLanguage")]
    public class SetEnumRunLanguage : PSCmdlet
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
                    foreach (Language lang in langs)
                    {
                        if (Extensions != null) { lang.Extensions = Extensions; }
                        if (Command != null) { lang.Command = Command; }
                        if (Command_x86 != null) { lang.Command_x86 = Command_x86; }
                        if (ArgsPrefix != null) { lang.ArgsPrefix = ArgsPrefix; }
                        if (ArgsMidWithoutArgs != null) { lang.ArgsMidWithoutArgs = ArgsMidWithoutArgs; }
                        if (ArgsMidWithArgs != null) { lang.ArgsMidWithArgs = ArgsMidWithArgs; }
                        if (ArgsSuffix != null) { lang.ArgsSuffix = ArgsSuffix; }
                    }
                }
                else
                {
                    //  存在しない場合は何もしない
                    return;
                }
            }
            else if (Language != null)
            {
                //  パイプラインで渡されたLanguageと名前が一致している場合に上書き
                foreach (Language lang in Language)
                {
                    int index = Item.Setting.Languages.FindIndex(x => x.Name.Equals(lang.Name, StringComparison.OrdinalIgnoreCase));
                    if (index >= 0)
                    {
                        Item.Setting.Languages[index] = lang;
                    }
                    else
                    {
                        //  存在しない場合は何もしない
                        return;
                    }
                }
            }
            Item.Setting.Save(Path);
        }
    }
}
