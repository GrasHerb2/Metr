using Microsoft.Office.Interop.Excel;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.Json;

namespace Metr.Classes
{
    class JsonObject
    {
        public string Name { get; set; }
        public List<DeviceData> devices1 { get; set; }
    }
    class Column
    {
        public string Header { get; set; }
        public int Field { get; set; }
    }
    public class EClass
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
                Presets.AddRange(SettingsClass.EPresets);
            }
            catch
            {

            }
        }
        public static void ExportExcel(EClass settings)
        {


            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Excel (*.xlsx)|*.xlsx|Txt|*.txt|Json|*.json";



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

            if (!settings.Field.Contains(8) ||
                !settings.Field.Contains(9) ||
                !settings.Field.Contains(10) ||
                !settings.Field.Contains(11) ||
                !settings.Field.Contains(12) ||
                !settings.Field.Contains(13))
            {
                devs = devs.Where(d => d.FNum != "Н/Д").ToList();
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

                            File.WriteAllText(save + fName + ".txt", content);
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
                    string text = "";
                    if (settings.Settings[2] == 1)
                    {
                        foreach (List<DeviceData> devices in devs.GroupBy(d => d.ObjectName).Select(grp => grp.ToList()))
                        {
                            text = JsonSerializer.Serialize(new JsonObject() { Name = devices[0].ObjectName, devices1 = devices });
                        }
                    }
                    else
                    {
                        text = JsonSerializer.Serialize(devs);
                    }


                    File.WriteAllText(saveFileDialog.FileName, text);
                }

                if (saveFileDialog.FileName.Contains(".xlsx"))
                {
                    Microsoft.Office.Interop.Excel.Application ExcelApp = null;
                    Workbook book = null;
                    Worksheet sheet = null;
                    try
                    {
                        ExcelApp = new Microsoft.Office.Interop.Excel.Application();
                        ExcelApp.Visible = false;
                        ExcelApp.DisplayAlerts = false;

                        book = ExcelApp.Workbooks.Add();


                        string exportString = "";

                        if (settings.Settings[2] == 1)
                        {
                            foreach (List<DeviceData> devices in devs.GroupBy(d => d.ObjectName).Select(grp => grp.ToList()))
                            {
                                sheet = book.Worksheets.Add();
                                sheet.Name = devices[0].ObjectName.Length > 30 ? devices[0].ObjectName.Remove(30) : devices[0].ObjectName;


                                sheet.Range[sheet.Cells[1, 1], sheet.Cells[1, settings.Field.Count()]].Merge();
                                sheet.Range[sheet.Cells[1, 1], sheet.Cells[1, settings.Field.Count()]].Value2 = devices[0].ObjectName;
                                sheet.Range[sheet.Cells[1, 1], sheet.Cells[1, settings.Field.Count()]].HorizontalAlignment = XlHAlign.xlHAlignCenter;


                                for (int h = 0; h < settings.CHeader.Count; h++)
                                {
                                    sheet.Cells[2, h + 1].Value2 = settings.CHeader[h];
                                    sheet.Cells[2, h + 1].Borders.LineStyle = XlLineStyle.xlContinuous;
                                }

                                for (int i = 0; i < devices.Count; i++)
                                {
                                    exportString = DeviceData.DevString(settings.Field, devices[i]);
                                    for (int j = 0; j < exportString.Split('\t').Count() - 1; j++)
                                    {
                                        sheet.Cells[i + 3, j + 1].Value2 = exportString.Split('\t')[j];
                                        sheet.Cells[i + 3, j + 1].Borders.LineStyle = XlLineStyle.xlContinuous;
                                    }
                                }
                                sheet.Range[sheet.Cells[1, 1], sheet.Cells[devices.Count + 2, settings.Field.Count()]].Columns.AutoFit();
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
                        Marshal.ReleaseComObject(sheet);
                        Marshal.ReleaseComObject(book);

                        ExcelApp.Application.ActiveWorkbook.SaveAs(saveFileDialog.FileName, Type.Missing);
                        ExcelApp.Quit();

                        Marshal.ReleaseComObject(ExcelApp);
                    }
                    finally
                    {
                        var processes = from p in Process.GetProcessesByName("EXCEL")
                                        select p;

                        foreach (var process in processes)
                        {
                            if (process.MainWindowTitle == "")
                                process.Kill();
                        }
                    }

                }

            }
        }
    }
}
