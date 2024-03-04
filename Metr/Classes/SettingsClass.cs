using System.Collections.Generic;
using System.Linq;

namespace Metr.Classes
{
    public static class SettingsClass
    {
        public static string prelogin { get; set; }
        public static List<EClass> EPresets { get; set; }
        static string saveFile = @"./settings.txt";
        static string saveText = "";

        public static void FileCheck()
        {
            if (!System.IO.File.Exists(saveFile))
            {
                System.IO.File.Create(saveFile);
                System.IO.File.WriteAllText(saveFile, "╟↔├");
            }
            saveText = System.IO.File.ReadAllText(@"./settings.txt");
            saveText = saveText.Contains('╟') ? saveText : "╟↔" + saveText;
            saveText = saveText.Contains('├') ? saveText : saveText + "├";
        }

        public static void LoadSettings(bool logSave = false)
        {
            FileCheck();
            try
            {
                prelogin = saveText.Split('╟')[1][0] == '↔' ? "" : saveText.Split('╟')[1].Split('├')[0];


                //Name = "ППР на год",
                //CHeader = new List<string> { "Объект", "Название", "Метрологические данные", "Заводской номер", "Измеряемый параметр", "МП/ППР1", "ППР2", "ППР3", "ППР4" },
                //Field = new List<int> { 4, 2, 6, 3, 5, 9, 11, 12, 13 },
                //Settings = new List<int> { 0, 2, 1 }
                foreach (string p in saveText.Split('├')[1].Split('█'))
                {
                    EPresets.Add(new EClass()
                    {
                        Name = p.Split('▌')[0],
                        CHeader = p.Split('▌')[1].Split('▀').ToList(),
                        Field = p.Split('▌')[2].Split('▀').Select(int.Parse).ToList(),
                        Settings = p.Split('▌')[3].Split('▀').Select(int.Parse).ToList()
                    });
                }
            }
            catch
            {

            }
        }

        public static void SaveLogin(string LogString)
        {
            FileCheck();
            saveText = "╟" + LogString + "├" + saveText.Split('├')[1];
            System.IO.File.WriteAllText(saveFile, saveText);
        }

        static string presetText = "";
        public static void SavePreset(EClass preset)
        {
            FileCheck();
            presetText = '█' + preset.Name + '▌';

            foreach (string Header in preset.CHeader) presetText += Header + '▀';
            presetText = presetText.Remove(presetText.Length - 1) + '▌';

            foreach (int Field in preset.Field) presetText += Field + '▀';
            presetText = presetText.Remove(presetText.Length - 1) + '▌';

            foreach (int Settings in preset.Settings) presetText += Settings + '▀';
            presetText = presetText.Remove(presetText.Length - 1) + '▌';

            System.IO.File.WriteAllText(saveFile, saveText);
        }

        static EClass tpreset = new EClass();
        public static void DelPreset(EClass preset)
        {
            foreach (string p in saveText.Split('├')[1].Split('█'))
            {
                tpreset = new EClass()
                {
                    Name = p.Split('▌')[0],
                    CHeader = p.Split('▌')[1].Split('▀').ToList(),
                    Field = p.Split('▌')[2].Split('▀').Select(int.Parse).ToList(),
                    Settings = p.Split('▌')[3].Split('▀').Select(int.Parse).ToList()
                };
                if (preset == tpreset)
                    saveText.Remove(saveText.IndexOf(p)-1, p.Length+1);
            }
            System.IO.File.WriteAllText(saveFile, saveText);
        }
    }
}

