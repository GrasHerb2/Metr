﻿using Metr.Classes;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace Metr
{
    /// <summary>
    /// Логика взаимодействия для NewDeviceWin.xaml
    /// </summary>
    public partial class DeviceWin : Window
    {
        
        public bool add = true;

        public List<Actions> actions { get; set; }
        public CurrentUser User { get; set; }
        MetrBaseEntities context = MetrBaseEntities.GetContext();
        public Device dev;
        public DeviceWin(bool adding = true, int i = 0)
        {
            InitializeComponent();
            objectCB.ItemsSource = context.Object.ToList();
            add = adding;
            dev = context.Device.Where(d=>d.Device_ID == i).FirstOrDefault();
            actions = new List<Actions>();
            bool noppr = false;
            

            if (!add) 
            { 
                this.Title = "Изменение";

                if (!string.IsNullOrEmpty(dev.NoteText))
                    noppr = dev.NoteText.Contains("^noPPR^");
                numTxt.Text = dev.FNum;
                expDatePicker.SelectedDate = dev.ExpDate;
                metrDataTxt.Text = dev.MetrData;
                nameTxt.Text = dev.Name;
                noteTxt.Text = dev.NoteText != null ? dev.NoteText.Split(':')[0] : "";
                paramTxt.Text = dev.Param;
                objectCB.Text = dev.Object.Name;
                chbPPR.IsChecked = noppr;
                periodTxt.Text = 
                    (dev.NoteText != null && dev.NoteText.Contains(":")) ? 
                    ((dev.NoteText.Split(':')[1]+"").Contains("^per+") ? 
                    dev.NoteText.Split(':')[1].Split('+')[1].Split('^')[0] : "") : "";
            }
        }

        private void saveBtn_Click(object sender, RoutedEventArgs e)
        {
            if (nameTxt.Text == "" || objectCB.Text == "")
            {
                MessageBox.Show("Поля \"Название\" и \"Объект\" обязательны для заполнения", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }

            if (add)
            {
                switch (DeviceData.NewDevice(nameTxt.Text, objectCB.Text, numTxt.Text, paramTxt.Text, metrDataTxt.Text, expDatePicker.SelectedDate, periodTxt.Text.Replace(':', ' ').Replace('+',' '), noteTxt.Text.Replace(':', ' ').Replace('+',' '), chbPPR.IsChecked.Value, User.Id))
                {
                    case MessageBoxResult.Yes:
                        DialogResult = true;
                        return;
                    case MessageBoxResult.No:
                        DialogResult = false;
                        return;
                    default: break;
                }
                    
            }
            else 
            {
                switch(DeviceData.DeviceEdit(dev,nameTxt.Text, objectCB.Text, numTxt.Text, paramTxt.Text, metrDataTxt.Text, expDatePicker.SelectedDate, periodTxt.Text.Replace(':',' ').Replace('+', ' '), noteTxt.Text.Replace(':', ' ').Replace('+', ' '), User.Id,!chbPPR.IsChecked.Value))
                {
                    case MessageBoxResult.Yes:
                        DialogResult = true;
                        return;
                    case MessageBoxResult.No:
                        DialogResult = false;
                        return;
                    default: break;
                }
            }

        }
        bool winPin = false;
        private void WindowPin_Click(object sender, RoutedEventArgs e)
        {
            winPin = !winPin;
            this.Topmost = winPin;
        }
    }
}
