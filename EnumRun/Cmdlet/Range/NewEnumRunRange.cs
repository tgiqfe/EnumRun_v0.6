using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management.Automation;

namespace EnumRun.Cmdlet
{
    [Cmdlet(VerbsCommon.New, "EnumRunRange")]
    public class NewEnumRunRange : PSCmdlet
    {
        [Parameter(Mandatory = true, Position = 0)]
        public string Name { get; set; }
        [Parameter(Mandatory = true, Position = 1)]
        public int StartNumber { get; set; }
        [Parameter(Mandatory = true, Position = 2)]
        public int EndNumber { get; set; }

        /*
        protected override void BeginProcessing()
        {
            Item.Config = EnumRunConfig.Load();
        }
        */

        protected override void ProcessRecord()
        {
            WriteObject(new Range()
            {
                Name = this.Name,
                StartNumber = this.StartNumber,
                EndNumber = this.EndNumber
            });
        }
    }
}
