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
        public static readonly string CURRENT_DIR_SETTING = Path.Combine(
            Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
            CONFIG_JSON);
        public static readonly string PROGRAMDATA_SETTING = Path.Combine(
            Environment.ExpandEnvironmentVariables("%PROGRAMDATA%"),
            APPLICATION_NAME,
            CONFIG_JSON);

        //  複数オブジェクトからアクセスする予定のあるパラメータ
        //public static EnumRunSetting Setting = null;
        public static Logger Logger = null;
        //public static DateTime StartTime;

        //  ファイル名関連
        public const string SESSION_FILE = "session.json";
        public const string CONFIG_JSON = "Setting.json";
        public const string CONFIG_XML = "Setting.xml";
        public const string CONFIG_YML = "Setting.yml";
    }
}
