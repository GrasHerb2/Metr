using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using System.Windows;
using System.Xml.Linq;
using System.Windows.Controls;
using System.Security.AccessControl;
using System.Security.Cryptography;
using System.Reflection;
using System.Diagnostics.Tracing;

namespace Metr.Classes
{
    internal class DeviceData
    {
        public int ID { get; set; }
        public string FNum { get; set; }
        public string Name { get; set; }
        public string MetrData { get; set; }
        public int ObjectID { get; set; }
        public string ObjectName { get; set; }
        public string Param { get; set; }        
        public string Note { get; set; }        
        public DateTime? ExpDate { get; set; }
        public DateTime? pprDate1 { get; set; }
        public DateTime? pprDate2 { get; set; }
        public DateTime? pprDate3 { get; set; }
        public bool Defect { get; set; }
        public bool Delete { get; set; }
        public bool PPR { get; set; }
        public string Stroke { get; set; }
        public string Period { get; set; }

        public static List<DeviceData> deviceList = new List<DeviceData>();
        public static List<DeviceData> deviceListMain = new List<DeviceData>();
        public static List<DeviceData> deviceListPPR = new List<DeviceData>();
        public static List<DeviceData> deviceListExc = new List<DeviceData>();
        public static string infoMain { get; set; }
        public static string infoPPR { get; set; }
        public static string infoExc { get; set; }

        public static List<Device> devices = new List<Device>();

        public static List<DeviceData> dataUpdate()
        {
            deviceList.Clear();
            devices = MetrBaseEntities.GetContext().Device.ToList();
            string note = "";
            foreach (Device d in devices)
            {
                note += d.NoteText;
                deviceList.Add(new DeviceData()
                {
                    ID = d.Device_ID,
                    ExpDate = d.ExpDate,
                    pprDate1 = d.ExpDate != null ? d.ExpDate.Value.AddMonths(3) : new DateTime(),
                    pprDate2 = d.ExpDate != null ? d.ExpDate.Value.AddMonths(6) : new DateTime(),
                    pprDate3 = d.ExpDate != null ? d.ExpDate.Value.AddMonths(9) : new DateTime(),
                    Period = d.NoteText != null ? (d.NoteText.Contains("^per+") ? d.NoteText.Split('+')[1].Split('^')[0]+"" : "") : "",
                    Stroke = "None",
                    MetrData = d.MetrData,
                    FNum = d.FNum,
                    Name = d.Name,
                    ObjectID = d.Object.Object_ID,
                    ObjectName = d.Object.Name,
                    Param = d.Param,
                    Note = note.Split(':')[0],

                    Defect = note.Contains("^def^") ? true : false,
                    Delete = note.Contains("^del^") ? true : false,
                    PPR = !note.Contains("^noPPR^") ? true : false
                });
                note = "";
            }
            infoMain = "Приборы:" + deviceList.Count + " из " + devices.Count();

           Search(new List<string>() { "","",""}, DateTime.MinValue, DateTime.MaxValue, false, false);            
            
            return deviceList;
        }

        public static List<DeviceData> Search(List<string> dSearch, DateTime searchStart, DateTime searchEnd, bool Def, bool Del, bool pprDate = false, bool Exp = false)
        {
            deviceListMain = deviceList;
                int total = deviceList.Count;

            deviceListExc = deviceListMain.Where(d =>
                d.Defect || d.Delete
                ).ToList();

            infoExc = " из " + deviceListExc.Count();

            deviceListPPR = deviceListMain.Where(d => d.PPR && !d.Defect && !d.Delete && d.ExpDate != null).ToList();

            infoPPR = " из " + deviceListPPR.Count();

            if (dSearch[0] != "")
            {
                deviceListMain = deviceListMain.Where(d =>
                    d.FNum == dSearch[0]
                    ).ToList();

                deviceListPPR = deviceListPPR.Where(d =>
                    d.FNum == dSearch[0]
                    ).ToList();

                deviceListExc = deviceListExc.Where(d =>
                    d.FNum == dSearch[0]
                    ).ToList();
            }
                             

            if (dSearch[2] != "")
            {
                deviceListMain = deviceListMain.Where(d =>
                d.ObjectName == dSearch[2]
                ).ToList();

                deviceListPPR = deviceListPPR.Where(d =>
                d.ObjectName == dSearch[2]
                ).ToList();

                deviceListExc = deviceListExc.Where(d =>
                d.ObjectName == dSearch[2]
                ).ToList();
            }

            string expInfo = "";
            int expCount = 0;
            if (Exp)
            {
                foreach (DeviceData a in deviceListMain)
                {
                    a.Stroke = a.ExpDate != null ? (DateTime.Compare(a.ExpDate.Value, DateTime.Now) < 0 ? "Underline" : "None") : "None";
                    expCount += a.ExpDate != null ? (DateTime.Compare(a.ExpDate.Value, DateTime.Now) < 0 ? 1 : 0) : 0;
                    expInfo = expCount + " просроченных приборов";
                }
            }

            deviceListMain = deviceListMain.Where(d =>
            d.Name.Contains(dSearch[1])
            ).ToList();

            deviceListPPR = deviceListPPR.Where(d =>
            d.Name.Contains(dSearch[1])
            ).ToList();

            deviceListExc = deviceListExc.Where(d =>
            d.Name.Contains(dSearch[1])
            ).ToList();
            //Конец поиска исключённых


            

            

            

            if (searchStart != new DateTime())
            {
                deviceListMain = deviceListMain.Where(d =>
                    d.ExpDate == null ||
                    DateTime.Compare(d.ExpDate.Value, searchStart) >= 0
                    ).ToList();
                deviceListPPR = !pprDate ? deviceListPPR.Where(d =>
                    DateTime.Compare(d.ExpDate.Value, searchStart) >= 0
                    ).ToList() : deviceListPPR;
            }

            if (searchEnd != new DateTime())
            {
                deviceListMain = deviceListMain.Where(d =>
                    d.ExpDate == null ||
                    DateTime.Compare(d.ExpDate.Value, searchEnd) <= 0
                    ).ToList();
                deviceListPPR = !pprDate ? deviceListPPR.Where(d =>
                    DateTime.Compare(d.ExpDate.Value, searchEnd) <= 0
                    ).ToList() : deviceListPPR;
            }
                

            deviceListPPR = pprDate ? deviceListPPR.Where(d =>
                (d.ExpDate.Value.Month == DateTime.Now.Month && d.ExpDate.Value.Year == DateTime.Now.Year) ||
                (d.pprDate1.Value.Month == DateTime.Now.Month && d.pprDate1.Value.Year == DateTime.Now.Year) ||
                (d.pprDate2.Value.Month == DateTime.Now.Month && d.pprDate2.Value.Year == DateTime.Now.Year) ||
                (d.pprDate3.Value.Month == DateTime.Now.Month && d.pprDate3.Value.Year == DateTime.Now.Year)
                ).ToList() : deviceListPPR;

            deviceListPPR.OrderBy(d => d.ExpDate);

            if (Def)
                deviceListMain = deviceListMain.Where(d =>
                d.Defect
                ).ToList();
            else
                deviceListMain = deviceListMain.Where(d =>
                !d.Defect
                ).ToList();

            if (Del)
                deviceListMain = deviceListMain.Where(d =>
                d.Delete
                ).ToList();
            else
                deviceListMain = deviceListMain.Where(d =>
                !d.Delete
                ).ToList();

            infoMain = "Приборы:" + deviceListMain.Count + " из " + total + (Exp ? "\n" + expInfo:"");
            infoPPR = "Приборы: " + deviceListPPR.Count() + infoPPR;
            infoExc = "Приборы: " + deviceListExc.Count() + infoExc;

            
            deviceListMain = Exp ? deviceListMain.OrderByDescending(d => d.Stroke).ToList() : deviceListMain;

            return deviceListMain;
            
        }
        public static List<Actions> actions = new List<Actions>();
        public static MessageBoxResult NewDevice(string Name, string ObjectName, string FNum, string Param, string MetrData, DateTime? ExpDate, string Period, string NoteText, bool PPR, int user )
        {
            MetrBaseEntities context = MetrBaseEntities.GetContext();

            actions.Clear();

            string tags = (!PPR ? "^noPPR^" : "") + (Period != "" ? "^per+"+Period+"^" : "");

            string log = "";

            int objId = -1;

            if (context.Object.Where(o => o.Name.ToLower() == ObjectName.ToLower()).Count() != 0)
                objId = context.Object.Where(o => o.Name.ToLower() == ObjectName.ToLower()).FirstOrDefault().Object_ID;

            if (objId == -1)
                if (MessageBox.Show("В базе данных не был найден объект \"" + ObjectName + "\".\n Добавить новый объект в базу данных?", "Добавление", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    context.Object.Add(new Object() { Name = ObjectName });
                    actions.Add(new Actions() { UserID = user, ActionDate = DateTime.Now, ActionText = "Добавление объекта " + ObjectName, ComputerName = Environment.MachineName });
                    context.SaveChanges();
                }
                else
                    return MessageBoxResult.Cancel;


            log = "Номер:" + FNum + "\nНазвание:" + Name + "\nИзмеряемый параметр:" + Param + "\nЕдиницы измерения:" + MetrData + "\nОбъект:" + ObjectName + "\nСрок годности:" + ExpDate + "\nДоп. информация:" + NoteText;


            switch (MessageBox.Show("Вы хотите добавить:\n\"" + log + "\"?", "Добавление", MessageBoxButton.YesNoCancel, MessageBoxImage.Question))
            {
                case MessageBoxResult.Yes:

                    Device device = new Device()
                    {
                        FNum = FNum,
                        Name = Name,
                        IDObject = context.Object.Where(o => o.Name == ObjectName).FirstOrDefault().Object_ID,
                        Param = Param,
                        MetrData = MetrData,
                        ExpDate = ExpDate,
                        NoteText = NoteText + ":" + tags
                    };
                    actions.Add(new Actions() { UserID = user, ActionDate = DateTime.Now, ActionText = "Добавление прибора\n" + log, ComputerName = Environment.MachineName });

                    context.Actions.AddRange(actions);
                    context.Device.Add(device);
                    context.SaveChanges();

                    return MessageBoxResult.Yes;

                case MessageBoxResult.No:
                    return MessageBoxResult.No;

                case MessageBoxResult.Cancel:
                    return MessageBoxResult.Cancel;
            }
            return MessageBoxResult.None;
        }
        public static MessageBoxResult DeviceEdit(Device dev, string Name, string ObjectName, string FNum, string Param, string MetrData, DateTime? ExpDate, string Period, string NoteText, int user, bool? PPR = null)
        {
            MetrBaseEntities context = MetrBaseEntities.GetContext();

            string tags = "";

            if (!string.IsNullOrEmpty(dev.NoteText))
                tags = dev.NoteText.Contains(':') ? "" + dev.NoteText.Split(':')[1] : "";

            int objId = -1;

            string log = "";

            if (context.Object.Where(o => o.Name.ToLower() == ObjectName).Count() != 0)
                objId = context.Object.Where(o => o.Name.ToLower() == ObjectName).FirstOrDefault().Object_ID;

            if (objId == -1)
                if (MessageBox.Show("В базе данных не был найден объект \"" + ObjectName + "\".\n Добавить новый объект в базу данных?", "Добавление", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    context.Object.Add(new Object() { Name = ObjectName });
                    context.SaveChanges();
                    actions.Add(new Actions() { UserID = user, ActionDate = DateTime.Now, ActionText = "Добавление объекта " + ObjectName, ComputerName = Environment.MachineName });
                }
                else
                    return MessageBoxResult.Cancel;

            log += tags.Contains("^per+") ?
                    (tags.Split('+')[1].Split('^')[0] == Period ? "" : tags.Split('+')[1].Split('^')[0] + "->" + Period + "\n") : (Period != "" ? "Период: " + Period + "\n" : "");

            tags = tags.Contains("^per+") ?
                tags.Split('+')[1].Split('^')[0] + "" == Period ?
                    tags
                    : tags.Remove(tags.IndexOf("^per+"), 6 + tags.Split('+')[1].Split('^')[0].Length) + "^per+" + Period + "^"
                : Period != "" ?
                    tags + "^per+" + Period + "^"
                    : tags;

            if (!string.IsNullOrEmpty(tags) && PPR != null)
            {                    
                log += tags.Contains("^noPPR^") && PPR == true ? "\nППР: Включён\n" : !tags.Contains("^noPPR^") && PPR == false ? "\nППР: Исключён\n" : "";
                                        
                tags += tags.Contains("^noPPR^") && PPR == true ? tags.Replace("^noPPR^", "") : !tags.Contains("^noPPR^") && PPR == false ? "^noPPR^" : "";
            }            

            log +=
                (Name + "" != dev.Name.ToString() + "" && !(string.IsNullOrEmpty(Name) && string.IsNullOrEmpty(dev.Name)) ? "Название:\n" + dev.Name + "->" + Name + "\n" : "") +

                (FNum + "" != dev.FNum + "" && !(string.IsNullOrEmpty(FNum) && string.IsNullOrEmpty(dev.FNum)) ? "Номер:\n" + dev.FNum + "->" + FNum + "\n" : "") +
                
                (ObjectName + "" != dev.Object.Name + "" && !(string.IsNullOrEmpty(ObjectName) && string.IsNullOrEmpty(dev.Object.Name)) ? "Объект:\n" + dev.Object.Name + "->" + ObjectName : "") +
                
                (Param + "" != dev.Param + "" && !(string.IsNullOrEmpty(Param) && string.IsNullOrEmpty(dev.Param)) ? "Измеряемый параметр:\n" + dev.Param + "->" + Param + "\n" : "") +
                
                (MetrData + "" != dev.MetrData + "" && !(string.IsNullOrEmpty(MetrData) && string.IsNullOrEmpty(dev.MetrData)) ? "Единицы измерения:\n" + dev.MetrData + "->" + MetrData + "\n" : "") +
                
                (NoteText+"" != (dev.NoteText + "").Split(':')[0]  && !(string.IsNullOrEmpty(NoteText) && string.IsNullOrEmpty(dev.NoteText)) ? "Примечание:\n" + dev.NoteText.Split(':')[0] + "->" + NoteText + "\n" : "") +
                
                (ExpDate.ToString() + "" != dev.ExpDate.ToString() + "" && !(string.IsNullOrEmpty(ExpDate.ToString()) && string.IsNullOrEmpty(dev.ExpDate.ToString())) ? ":\n" + dev.ExpDate + "->" + ExpDate : "") +
                "\n"
                ;

            if (log == "\n") return MessageBoxResult.None;

            switch (MessageBox.Show("Будут проведены следующие изменения:\n" + log + "Сохранить?", "Изменение", MessageBoxButton.YesNoCancel, MessageBoxImage.Question))
            {
                case MessageBoxResult.Yes:
                    actions.Add(new Actions() { UserID = user, ActionDate = DateTime.Now, ActionText = "Изменение прибора " + log, ComputerName = Environment.MachineName });
                    dev = context.Device.Where(d => d.Device_ID == dev.Device_ID).FirstOrDefault();

                    dev.FNum = FNum;
                    dev.ExpDate = ExpDate;
                    dev.MetrData = MetrData;
                    dev.Name = Name;
                    dev.NoteText = NoteText + ":" + tags;
                    dev.Param = Param;
                    dev.IDObject = context.Object.Where(o => o.Name == ObjectName).FirstOrDefault().Object_ID;

                    context.Actions.AddRange(actions);
                    
                    context.SaveChanges();
                    return MessageBoxResult.Yes;
                case MessageBoxResult.No:
                    return MessageBoxResult.No;
                case MessageBoxResult.Cancel:
                    return MessageBoxResult.Cancel;
            }
            return MessageBoxResult.None;
        }

        public static void deviceDel(List<Device> devices, MetrBaseEntities context, int user)
        {
            List<Actions> actions = new List<Actions>();
            List<Device> devs = new List<Device>();
            string devstring = "";
            if (devices.Count() != 0)
            {
                foreach (Device d in devices)
                {
                    if (!string.IsNullOrEmpty(d.NoteText))
                    {
                        if (d.NoteText.Contains("^del^"))
                            MessageBox.Show(d.Name + " " + d.FNum + " уже удалён, для восстановления необходимо перейти во вкладку 'Исключённые'", "Удаление", MessageBoxButton.OK, MessageBoxImage.Information);
                        else
                        {
                            devstring +="\n"+d.Name + " " + d.FNum;
                            devs.Add(d);
                        }
                    }         
                }
                if (MessageBox.Show("Будут исключены из списка следующие приборы:"+devstring+"\nПрименить?\n Данные приборы можно будет найти и восстановить на вкладке 'Исключённые'.","Удаление",MessageBoxButton.YesNo,MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    foreach(Device d in devs)
                    {
                        d.NoteText = string.IsNullOrEmpty(d.NoteText) ? ":^del^" : d.NoteText.Contains(':') ? d.NoteText + "^del^": ":^del^";
                        context.Actions.Add(new Actions() { UserID = user, ActionDate = DateTime.Now, ActionText = "Исключение прибора " + d.Name + " " + d.FNum, ComputerName = Environment.MachineName });
                    }
                    context.SaveChanges();
                    MessageBox.Show("Приборы исключены", "Удаление", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            else
            {
                MessageBox.Show("Выделите приборы для удаления");
            }
        }
    }
    
}
