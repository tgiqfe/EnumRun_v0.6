using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;
using EnumRun.Serialize;

namespace EnumRun
{
    public class EnumRunSetting
    {
        public string FilesPath { get; set; }
        public string LogsPath { get; set; }
        public string OutputPath { get; set; }
        public bool DebugMode { get; set; }
        public bool RunOnce { get; set; }
        public List<Range> Ranges { get; set; }
        public List<Language> Languages { get; set; }

        public EnumRunSetting()
        {
            this.FilesPath = Path.Combine(Item.DEFAULT_WORKDIR, "Files");
            this.LogsPath = Path.Combine(Item.DEFAULT_WORKDIR, "Logs");
            this.OutputPath = Path.Combine(Item.DEFAULT_WORKDIR, "Output");
            this.DebugMode = false;
            this.RunOnce = false;
            this.Ranges = DefaultRangeSettings.Create();
            this.Languages = DefaultLanguageSetting.Create();
        }

        //  廃止予定
        public EnumRunSetting(bool loadDefault)
        {
            if (loadDefault)
            {
                string confDir = Environment.ExpandEnvironmentVariables("%PROGRAMDATA%");
                this.FilesPath = Path.Combine(confDir, "Files");
                this.LogsPath = Path.Combine(confDir, "Logs");
                this.OutputPath = Path.Combine(confDir, "Output");
                this.DebugMode = false;
                this.RunOnce = false;
                this.Ranges = DefaultRangeSettings.Create();
                this.Languages = DefaultLanguageSetting.Create();
            }
        }






        //  廃止予定
        /// <summary>
        /// 設定ファイルからEnumRunSettingパラメータをロード
        /// </summary>
        /// <returns>読み込んEnumRunConfigインスタンス</returns>
        public static EnumRunSetting Load(string confFile)
        {
            if (confFile == null)
            {
                confFile = Path.Combine(
                    Environment.ExpandEnvironmentVariables("%PROGRAMDATA%"),
                    Item.APPLICATION_NAME,
                    Item.CONFIG_JSON);
            }
            return !File.Exists(confFile) ?
                new EnumRunSetting(true) :
                DataSerializer.Deserialize<EnumRunSetting>(confFile);
        }

        //  廃止予定
        /// <summary>
        /// 設定を保存
        /// </summary>
        public void Save(string confFile)
        {
            if (confFile == null)
            {
                confFile = Path.Combine(
                    Environment.ExpandEnvironmentVariables("%PROGRAMDATA%"),
                    Item.APPLICATION_NAME,
                    Item.CONFIG_JSON);
            }
            if (!Directory.Exists(Path.GetDirectoryName(confFile)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(confFile));
            }
            DataSerializer.Serialize<EnumRunSetting>(this, confFile);
        }


        /// <summary>
        /// 一致する名前のLanguage配列を取得
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Language[] GetLanguage(string name)
        {
            string patternString = Regex.Replace(name, ".",
                x =>
                {
                    string y = x.Value;
                    if (y.Equals("?")) { return "."; }
                    else if (y.Equals("*")) { return ".*"; }
                    else { return Regex.Escape(y); }
                });
            if (!patternString.StartsWith("*")) { patternString = "^" + patternString; }
            if (!patternString.EndsWith("*")) { patternString = patternString + "$"; }
            Regex regPattern = new Regex(patternString, RegexOptions.IgnoreCase);

            return Languages.Where(x => regPattern.IsMatch(x.Name)).ToArray();
        }

        /// <summary>
        /// 一致する名前のRange配列を取得
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Range[] GetRange(string name)
        {
            string patternString = Regex.Replace(name, ".",
                x =>
                {
                    string y = x.Value;
                    if (y.Equals("?")) { return "."; }
                    else if (y.Equals("*")) { return ".*"; }
                    else { return Regex.Escape(y); }
                });
            if (!patternString.StartsWith("*")) { patternString = "^" + patternString; }
            if (!patternString.EndsWith("*")) { patternString = patternString + "$"; }
            Regex regPattern = new Regex(patternString, RegexOptions.IgnoreCase);

            return Ranges.Where(x => regPattern.IsMatch(x.Name)).ToArray();
        }
    }
}
