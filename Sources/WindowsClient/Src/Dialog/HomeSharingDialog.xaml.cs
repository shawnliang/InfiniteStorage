﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Waveface.Client.Src.Dialog
{
    /// <summary>
    /// HomeSharingDialog.xaml 的互動邏輯
    /// </summary>
    public partial class HomeSharingDialog : Window
    {
        public HomeSharingDialog()
        {
            InitializeComponent();
        }

        private void Image_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
        	Process.Start("https://play.google.com/store/apps/details?id=com.waveface.uploader");
        }

        private void btnHelp_Click(object sender, System.Windows.RoutedEventArgs e)
        {
        	Process.Start(MainWindow.HELP_URL);
        }

        private void tbtnSwitch_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            AdjustStatus();
        }

        private void AdjustStatus()
        {
            ClientFramework.Client.Default.HomeSharingEnabled = tbtnSwitch.IsChecked.Value;
            tbtnSwitch.Content = tbtnSwitch.IsChecked.Value ? "關閉快送":"開啟快送";
            tbxSwitchStatus.Text = tbtnSwitch.IsChecked.Value ? "快送：已開啟" : "快送：已關閉";
        }

        private void tbtnSwitch_Unchecked(object sender, System.Windows.RoutedEventArgs e)
        {
            AdjustStatus();
        }

        private void Window_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            tbtnSwitch.IsChecked = ClientFramework.Client.Default.HomeSharingEnabled;
            AdjustStatus();
        }
    }
}