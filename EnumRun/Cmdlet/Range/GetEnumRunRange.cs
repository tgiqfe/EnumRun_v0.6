using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management.Automation;

namespace EnumRun.Cmdlet
{
    [Cmdlet(VerbsCommon.Get, "EnumRunRange")]
    public class GetEnumRunRange : PSCmdlet
    {
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
            if (Name == null)
            {
                WriteObject(Item.Setting.Ranges);
            }
            else
            {
                WriteObject(Item.Setting.GetRange(Name));
            }
        }
    }
}
