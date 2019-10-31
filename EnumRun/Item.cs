using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;
using NLog;

namespace EnumRun
{
    internal class Item
    {
        //  静的パラメータ
        public const string APPLICATION_NAME = "EnumRun";
        public static readonly string DEFAULT_WORKDIR = Path.Combine(
            Environment.ExpandEnvironmentVariables("%PROGRAMDATA%"),
            APPLICATION_NAME);
        public static readonly string CURRENT_DIR = 
            Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        //  複数オブジェクトからアクセスする予定のあるパラメータ
        public static EnumRunSetting Setting = null;
        public static Logger Logger = null;
        public static DateTime StartTime;

        //  ファイル名関連
        public const string SESSION_FILE = "session.json";
        public const string CONFIG_JSON = "Setting.json";
        public const string CONFIG_XML = "Setting.xml";
        public const string CONFIG_YML = "Setting.yml";

        //  データタイプ
        public const string JSON = "Json";
        public const string XML = "Xml";
        public const string YML = "Yml";
    }
}
