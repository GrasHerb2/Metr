using Metr._Windows;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Office.Interop.Excel;
using System.Windows.Controls;
using Microsoft.Win32;
using System.Windows;
using System.Text.Json;
using System.Windows.Threading;

namespace Metr.Classes
{
    class Column
    {
        public string Header { get; set; }
        public int Field { get; set; }
    }
    class EClass
    {
        public string Name { get; set; }
        public List<string> CHeader { get; set; }
        public List<int> Field { get; set; }        
        public List<int> Settings { get; set; }

        public static List<EClass> Presets { get; set; }

        public static void UpdPresets()
        {
            Presets = new List<EClass>();
            Presets.AddRange(new List<EClass>()
            {
            new EClass()
            {
                Name = "ППР на год",
                CHeader = new List<string> { "Объект", "Название", "Метрологические данные", "Заводской номер", "Измеряемый параметр", "МП/ППР1", "ППР2", "ППР3", "ППР4" },
                Field = new List<int> { 4, 2, 6, 3, 5, 9, 11, 12, 13 },
                Settings = new List<int> { 0, 2, 1 }
            },

            new EClass()
            {
                Name = "ППР на месяц",
                CHeader = new List<string> { "Объект", "Название", "Метрологические данные", "Заводской номер", "Измеряемый параметр", "Дата" },
                Field = new List<int> { 4, 2, 6, 3, 5, 8 },
                Settings = new List<int> { 0, 2, 0 }
            },

            new EClass()
            {
                Name = "График поверки/калибровки",
                CHeader = new List<string> { "Название", "Метрологические данные", "Заводской номер", "Годен до", "Примечание", "Измеряемый параметр" },
                Field = new List<int> { 2, 6, 3, 7, 15, 5 },
                Settings = new List<int> { 0, 1, 0 }
            },

            new EClass()
            {
                Name = "Журнал сдачи приборов",
                CHeader = new List<string> { "Объект", "Название", "Измеряемый параметр", "Метрологические данные", "Заводской номер", "Годен до", "Дата отправки", "Годен до" },
                Field = new List<int> { 4, 2, 5, 6, 3, 7, 0, 0 },
                Settings = new List<int> { 0, 1, 0 }
            },
            new EClass()
            {
                Name = "Иной",
                CHeader = new List<string> { "", "", "", "", "" },
                Field = new List<int> { 0, 0, 0, 0, 0 },
                Settings = new List<int> { 0, 1, 0 }
            }
            });
            try
            {
                string presets = File.ReadAllText("//presets.txt");
                foreach (string set in presets.Split('\n'))
                {
                    EClass.Presets.Add(new EClass()
                    {
                        Name = set.Split(',')[0],
                        CHeader = set.Split(',')[1].Split(' ').ToList(),
                        Field = set.Split(',')[2].Split(' ').Select(int.Parse).ToList(),
                        Settings = set.Split(',')[3].Split(' ').Select(int.Parse).ToList()
                    });
                }
            }
            catch
            {

            }
        }
        public static void ExportExcel(EClass settings)
        {


            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Excel (*.xls or *.xlsx)|*.xls;*.xlsx|Txt|*.txt|Json|*.json";



            List<DeviceData> devs = new List<DeviceData>();

            if (settings.Settings[0] == 0)
            {
                DeviceData.Search(new List<string>() { "", "" }, new List<string>() { }, DateTime.MinValue, DateTime.MaxValue, false, false, settings.Field.Contains(8));
            }

            switch (settings.Settings[1])
            {
                case 0:
                    devs.AddRange(DeviceData.deviceListMain);
                    devs.AddRange(DeviceData.deviceListPPR);
                    devs.AddRange(DeviceData.deviceListExc);
                    devs = devs.Distinct().ToList();
                    break;
                case 1: devs = DeviceData.deviceListMain; break;
                case 2: devs = DeviceData.deviceListPPR; break;
                case 3: devs = DeviceData.deviceListExc; break;
            }


            devs = devs.OrderByDescending(x => x.ObjectName).ThenBy(x => x.Name).ThenBy(x => x.ExpDate).ToList();
            if (settings.Field.Contains(8)) devs = devs.OrderBy(x => x.pprMonthDate).ToList();

            string save = "";

            if (saveFileDialog.ShowDialog().Value)
            {
                if (saveFileDialog.FileName.Contains(".txt"))
                {
                    

                    if (settings.Settings[2] == 1)
                    {
                        foreach (List<DeviceData> devices in devs.GroupBy(d => d.ObjectName).Select(grp => grp.ToList()))
                        {

                            save = Path.GetDirectoryName(saveFileDialog.FileName) + "\\import\\";
                            Directory.CreateDirectory(save);

                            string fName = devices[0].ObjectName;

                            string content = "";

                            for (int h = 0; h < settings.CHeader.Count; h++)
                            {
                                content = settings.CHeader[h];
                            }

                            content += "\n";

                            for (int i = 0; i < devices.Count; i++)
                            {
                                content += DeviceData.DevString(settings.Field, devices[i]);
                                content += "\n";
                            }

                            File.WriteAllText(save+fName+".txt", content);
                        }
                    }
                    else
                    {


                        string content = "";
                        for (int h = 0; h < settings.CHeader.Count; h++)
                        {
                            content = settings.CHeader[h];
                        }

                        for (int i = 0; i < devs.Count; i++)
                        {
                            content += DeviceData.DevString(settings.Field, devs[i]);
                            content += "\n";
                        }
                        File.WriteAllText(saveFileDialog.FileName, content);
                    }
                }

                if (saveFileDialog.FileName.Contains(".json"))
                {
                    string text = JsonSerializer.Serialize(devs);
                    File.WriteAllText(saveFileDialog.FileName, text);
                }

                if (saveFileDialog.FileName.Contains(".xls") || saveFileDialog.FileName.Contains(".xlsx"))
                {

                    Microsoft.Office.Interop.Excel.Application ExcelApp = new Microsoft.Office.Interop.Excel.Application();
                    ExcelApp.Visible = false;
                    ExcelApp.DisplayAlerts = false;

                    Workbook book = ExcelApp.Workbooks.Add();


                    string exportString = "";

                    Worksheet sheet;

                    if (settings.Settings[2] == 1)
                    {
                        foreach (List<DeviceData> devices in devs.GroupBy(d => d.ObjectName).Select(grp => grp.ToList()))
                        {
                            sheet = book.Worksheets.Add();
                            sheet.Name = devices[0].ObjectName.Length > 30 ? devices[0].ObjectName.Remove(30) : devices[0].ObjectName;

                            for (int h = 0; h < settings.CHeader.Count; h++)
                            {
                                sheet.Cells[1, h + 1].Value2 = settings.CHeader[h];
                            }

                            for (int i = 0; i < devices.Count; i++)
                            {
                                exportString = DeviceData.DevString(settings.Field, devices[i]);
                                for (int j = 0; j < exportString.Split('\t').Count(); j++)
                                {
                                    sheet.Cells[i + 2, j + 1].Value2 = exportString.Split('\t')[j];
                                    sheet.Cells[i + 2, j + 1].Style.WrapText = true;
                                }
                            }


                        }
                    }
                    else
                    {
                        sheet = book.Worksheets.Add();
                        for (int h = 0; h < settings.CHeader.Count; h++)
                        {
                            sheet.Cells[1, h + 1].Value2 = settings.CHeader[h];
                        }

                        for (int i = 0; i < devs.Count; i++)
                        {
                            exportString = DeviceData.DevString(settings.Field, devs[i]);
                            for (int j = 0; j < exportString.Split('\t').Count(); j++)
                            {
                                sheet.Cells[i + 2, j + 1].Value2 = exportString.Split('\t')[j];
                            }
                        }
                    }


                    ExcelApp.Application.ActiveWorkbook.SaveAs(saveFileDialog.FileName);
                    ExcelApp.Quit();
                }
                
            }
        }
    }
}
