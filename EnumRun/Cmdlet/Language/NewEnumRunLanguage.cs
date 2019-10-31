using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management.Automation;

namespace EnumRun.Cmdlet
{
    [Cmdlet(VerbsCommon.New, "EnumRunLanguage")]
    public class NewEnumRunLanguage : PSCmdlet
    {
        [Parameter(Mandatory = true, Position = 0)]
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

        /*
        protected override void BeginProcessing()
        {
            Item.Config = EnumRunConfig.Load();
        }
        */

        protected override void ProcessRecord()
        {
            WriteObject(new Language()
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
}
