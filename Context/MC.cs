using Kingmaker.UnitLogic.Class.LevelUp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TabletopTweaks.Core.ModLogic;
using static UnityModManagerNet.UnityModManager;

namespace MinosRace.Context
{
    public class MC : ModContextBase
    {
        public static MC mc;
        public static void Log(string text)
        {
            mc.Logger.Log(text);
        }
        public MC(ModEntry modEntry) : base(modEntry)
        {
#if DEBUG
            Debug = false;
#endif
            LoadAllSettings();
        }
        public override void LoadAllSettings()
        {
          
            string path = $"{LocalizationFolder}{Path.DirectorySeparatorChar}{LocalizationFile}";
            if (!Directory.Exists(LocalizationFolder))
            {
                Directory.CreateDirectory(LocalizationFolder);
            }
            if (!File.Exists(path))
            {
                using (StreamWriter writer = new StreamWriter(path))
                {
                    using (StreamReader reader = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream("MinosRace.JSON.LocalizationPack.json")))
                    {
                        writer.Write(reader.ReadToEnd());
                        writer.Flush();
                    }
                }
            }
            LoadLocalization("MinosRace.JSON");
            LoadBlueprints("MinosRace.JSON", this);
        }



    }
}
