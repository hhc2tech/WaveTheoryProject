﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Diagnostics;
using OxyPlot;

namespace WaveTheoryProject
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private PlotWindowModel viewModel;
        WaveController c;

        public MainWindow()
        {
            WindowReferences.main = this;
            viewModel = new PlotWindowModel();
            DataContext = viewModel;
            InitializeComponent();
            labsList.SelectionChanged += LabsList_SelectionChanged;

            labsList.Items.Add("Плоские стоячие\nволны");
            labsList.Items.Add("Плоские прогрес-\nсивные волны");
            labsList.Items.Add("Движение волны в\nканале");
            labsList.Items.Add("Группа волн");
            labsList.Items.Add("Затухающая волна");
            labsList.SelectedIndex = 0;

            datagrid.ColumnWidth = new DataGridLength(20, DataGridLengthUnitType.Star);

            x0Box.Text = Settings.Init.x0.ToString();
            z0Box.Text = Settings.Init.z0.ToString();
            aBox.Text = Settings.a.ToString();
            kBox.Text = Settings.k.ToString();
            sigmaBox.Text = c.sigma.ToString(Settings.Format);
            tminBox.Text = Settings.InitTimeFrom.ToString(Settings.Format);
            tmaxBox.Text = Settings.InitTimeTo.ToString(Settings.Format);
            hBox.Text = Settings.Time_h.ToString(Settings.Format);
            p0Box.Text = Settings.p0.ToString(Settings.Format);
            roBox.Text = Settings.ro.ToString(Settings.Format);

            x0Box.TextChanged += paramBox_TextChanged;
            z0Box.TextChanged += paramBox_TextChanged;
            kBox.TextChanged += paramBox_TextChanged;
            aBox.TextChanged += paramBox_TextChanged;
            tminBox.TextChanged += paramBox_TextChanged;
            tmaxBox.TextChanged += paramBox_TextChanged;
            hBox.TextChanged += paramBox_TextChanged;
            p0Box.TextChanged += paramBox_TextChanged;
            roBox.TextChanged += paramBox_TextChanged;
        }

        private void LabsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (c != null) { while (!c.IsDrawingAvaliable) ; }
            switch (labsList.SelectedIndex)
            {
                case 0:
                    Settings.InitX0From = -10;
                    Settings.InitX0To = 10;
                    c = new PlaneWaveController();
                    kBlock.Text = "k =";
                    aBlock.Text = "a =";
                    sigmaBlock.Visibility = Visibility.Visible;
                    sigmaBox.Visibility = Visibility.Visible;
                    kBox.Text = Settings.k.ToString(Settings.Format);
                    aBox.Text = Settings.a.ToString(Settings.Format);
                    break;
                case 1:
                    Settings.InitX0From = -10;
                    Settings.InitX0To = 10;
                    c = new ProgressiveWaveController();
                    kBlock.Text = "k =";
                    aBlock.Text = "a =";
                    sigmaBlock.Visibility = Visibility.Visible;
                    sigmaBox.Visibility = Visibility.Visible;
                    kBox.Text = Settings.k.ToString(Settings.Format);
                    aBox.Text = Settings.a.ToString(Settings.Format);
                    break;
                case 2:
                    Settings.InitX0From = 0;
                    Settings.InitX0To = Settings.Canal.delta;
                    c = new CanalWaveController();
                    kBlock.Text = "h =";
                    aBlock.Text = "δ =";
                    sigmaBlock.Visibility = Visibility.Hidden;
                    sigmaBox.Visibility = Visibility.Hidden;
                    kBox.Text = Settings.Canal.h.ToString(Settings.Format);
                    aBox.Text = Settings.Canal.delta.ToString(Settings.Format); 
                    break;
                case 3:
                    Settings.InitX0From = -10;
                    Settings.InitX0To = 10;
                    c = new WaveGroupController();
                    kBlock.Text = "k =";
                    aBlock.Text = "a =";
                    sigmaBlock.Visibility = Visibility.Visible;
                    sigmaBox.Visibility = Visibility.Visible;
                    kBox.Text = Settings.k.ToString(Settings.Format);
                    aBox.Text = Settings.a.ToString(Settings.Format);
                    break;
                case 4:
                    Settings.InitX0From = -20;
                    Settings.InitX0To = 20;
                    c = new DecayingWaveController();
                    kBlock.Text = "k =";
                    aBlock.Text = "a =";
                    sigmaBlock.Visibility = Visibility.Visible;
                    sigmaBox.Visibility = Visibility.Visible;
                    kBox.Text = Settings.k.ToString(Settings.Format);
                    aBox.Text = Settings.a.ToString(Settings.Format);
                    break;
            }
            datagrid.ItemsSource = c.WavePointsFixedX;
        }

        private void paramBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox tmp = (sender as TextBox);
            try
            {
                switch (tmp.Name)
                {
                    case "x0Box":
                        c.x0fixed = Convert.ToDouble(tmp.Text.Replace('.', ','));           
                        break;
                    case "z0Box":
                        //double tmp_z0 = Convert.ToDouble(tmp.Text.Replace('.', ','));
                        c.z0fixed = Convert.ToDouble(tmp.Text.Replace('.', ','));
                        //if (tmp_z0 <= 0)
                        //{
                        //    c.z0fixed = tmp_z0;
                        //}
                        //else
                        //{
                        //    throw new ArgumentException();
                        //}
                        break;
                    case "kBox":
                        if (labsList.SelectedIndex == 0 || labsList.SelectedIndex == 1 || labsList.SelectedIndex == 3 || labsList.SelectedIndex == 4)
                        {
                            c.k = Convert.ToDouble(tmp.Text.Replace('.', ','));
                            sigmaBox.Text = c.sigma.ToString(Settings.Format);
                        }
                        else if (labsList.SelectedIndex == 2)
                        {
                            (c as CanalWaveController).h = Convert.ToDouble(tmp.Text.Replace('.', ','));
                        }
                        break;
                    case "aBox":
                        if (labsList.SelectedIndex == 0 || labsList.SelectedIndex == 1 || labsList.SelectedIndex == 3 || labsList.SelectedIndex == 4)
                        {
                            c.a = Convert.ToDouble(tmp.Text.Replace('.', ','));
                        }
                        else if (labsList.SelectedIndex == 2)
                        {
                            (c as CanalWaveController).Delta = Convert.ToDouble(tmp.Text.Replace('.', ','));
                        }
                        break;
                    case "p0Box":
                        c.p0 = Convert.ToDouble(tmp.Text.Replace('.', ','));
                        break;
                    case "roBox":
                        double tmp_ro = Convert.ToDouble(tmp.Text.Replace('.', ','));
                        if (tmp_ro > 0)
                        {
                            c.ro = tmp_ro;
                        }
                        else
                        {
                            throw new ArgumentException();
                        }
                        break;
                    case "tminBox":
                        double tmp_tmin = Convert.ToDouble(tmp.Text.Replace('.', ','));
                        if (tmp_tmin >= 0 && tmp_tmin < Settings.InitTimeTo)
                        {
                            Settings.InitTimeFrom = tmp_tmin;
                            c.Refresh();
                        }
                        else
                        {
                            throw new ArgumentException();
                        }
                        break;
                    case "tmaxBox":
                        double tmp_tmax = Convert.ToDouble(tmp.Text.Replace('.', ','));
                        if (tmp_tmax >= 0 && tmp_tmax > Settings.InitTimeFrom)
                        {
                            Settings.InitTimeTo = tmp_tmax;
                            c.Refresh();
                        }
                        else
                        {
                            throw new ArgumentException();
                        }
                        break;
                    case "hBox":
                        double tmp_h = Convert.ToDouble(tmp.Text.Replace('.', ','));
                        if (tmp_h > 0 && tmp_h < Settings.InitTimeTo-Settings.InitTimeFrom)
                        {
                            Settings.Time_h = tmp_h;
                            c.Refresh();
                        }
                        else
                        {
                            throw new ArgumentException();
                        }
                        break;
                }
                datagrid.Items.Refresh();
            }
            catch
            {
                return;
            }
        }

        private void ico_MouseEnter(object sender, MouseEventArgs e)
        {
            switch ((sender as Image).Name)
            {
                case "plotImage": plotContainer.Margin = new Thickness(1,1,1,1); return;
                case "exitImage": exitImage.Source = Settings.exitIcoSelectedSource; return;
            }
        }

        private void ico_MouseLeave(object sender, MouseEventArgs e)
        {
            switch ((sender as Image).Name)
            {
                case "plotImage": plotContainer.Margin = new Thickness(5,5,5,5); return;
                case "exitImage": exitImage.Source = Settings.exitIcoSource; return;
            }
        }

        private void ico_MouseDown(object sender, MouseButtonEventArgs e)
        {
            switch ((sender as Image).Name)
            {
                case "plotImage":
                    WindowReferences.plot = new PlotWindow(c);
                    WindowReferences.plot.Show();
                    Hide();
                    break;
                case "exitImage": Process.GetCurrentProcess().Kill(); return;
            }
        }
    }
}
