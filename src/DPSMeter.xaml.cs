﻿using EQTool.Models;
using EQTool.Services;
using EQTool.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace EQTool
{
    /// <summary>
    /// Interaction logic for DPSMeter.xaml
    /// </summary>
    public partial class DPSMeter : Window
    {
        private readonly System.Timers.Timer UITimer;
        private readonly LogParser logParser;
        private readonly DPSWindowViewModel dPSWindowViewModel;
        private readonly EQToolSettings settings;
        private readonly EQToolSettingsLoad toolSettingsLoad;

        public DPSMeter(LogParser logParser, DPSWindowViewModel dPSWindowViewModel, EQToolSettings settings, EQToolSettingsLoad toolSettingsLoad, LoggingService loggingService)
        {
            loggingService.Log(string.Empty, App.EventType.OpenDPS);
            this.settings = settings;
            this.logParser = logParser;
            this.logParser.FightHitEvent += LogParser_FightHitEvent;
            this.logParser.DeadEvent += LogParser_DeadEvent;
            this.dPSWindowViewModel = dPSWindowViewModel;
            this.dPSWindowViewModel.EntityList = new System.Collections.ObjectModel.ObservableCollection<EntittyDPS>();
            DataContext = dPSWindowViewModel;
            InitializeComponent();
            WindowExtensions.AdjustWindow(settings.DpsWindowState, this);
            Topmost = Properties.Settings.Default.GlobalDpsWindowAlwaysOnTop;
            UITimer = new System.Timers.Timer(1000);
            UITimer.Elapsed += PollUI;
            UITimer.Enabled = true;
            DpsList.ItemsSource = dPSWindowViewModel.EntityList;
            var view = (ListCollectionView)CollectionViewSource.GetDefaultView(dPSWindowViewModel.EntityList);
            view.GroupDescriptions.Add(new PropertyGroupDescription(nameof(EntittyDPS.TargetName)));
            view.LiveGroupingProperties.Add(nameof(EntittyDPS.TargetName));
            view.IsLiveGrouping = true;
            view.SortDescriptions.Add(new SortDescription(nameof(EntittyDPS.TargetName), ListSortDirection.Ascending));
            view.SortDescriptions.Add(new SortDescription(nameof(EntittyDPS.TotalDamage), ListSortDirection.Descending));
            view.IsLiveSorting = true;
            view.LiveSortingProperties.Add(nameof(EntittyDPS.TotalDamage));
            this.toolSettingsLoad = toolSettingsLoad;
            SizeChanged += DPSMeter_SizeChanged;
            StateChanged += SpellWindow_StateChanged;
            LocationChanged += DPSMeter_LocationChanged;
            settings.DpsWindowState.Closed = false;
            SaveState();
        }

        private void LogParser_FightHitEvent(object sender, LogParser.FightHitEventArgs e)
        {
            dPSWindowViewModel.TryAdd(e.HitInformation);
        }

        private void LogParser_DeadEvent(object sender, LogParser.DeadEventArgs e)
        {
            dPSWindowViewModel.TargetDied(e.Name);
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            UITimer.Stop();
            UITimer.Dispose();
            SizeChanged -= DPSMeter_SizeChanged;
            StateChanged -= SpellWindow_StateChanged;
            LocationChanged -= DPSMeter_LocationChanged;
            logParser.DeadEvent -= LogParser_DeadEvent;
            logParser.FightHitEvent -= LogParser_FightHitEvent;
            SaveState();
            base.OnClosing(e);
        }

        private void CloseWindow(object sender, RoutedEventArgs e)
        {
            settings.DpsWindowState.Closed = true;
            Close();
        }

        private void SaveState()
        {
            WindowExtensions.SaveWindowState(settings.DpsWindowState, this);
            toolSettingsLoad.Save(settings);
        }

        private void SpellWindow_StateChanged(object sender, EventArgs e)
        {
            SaveState();
        }

        private void DPSMeter_LocationChanged(object sender, EventArgs e)
        {
            SaveState();
        }

        private void DPSMeter_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            SaveState();
        }


        private void PollUI(object sender, EventArgs e)
        {
            dPSWindowViewModel.UpdateDPS();
        }

        public void DragWindow(object sender, MouseButtonEventArgs args)
        {
            DragMove();
        }

        private void MinimizeWindow(object sender, RoutedEventArgs e)
        {
            WindowState = System.Windows.WindowState.Minimized;
        }

        private void MaximizeWindow(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState == System.Windows.WindowState.Maximized ? System.Windows.WindowState.Normal : System.Windows.WindowState.Maximized;
        }

        private void opendps(object sender, RoutedEventArgs e)
        {
            (App.Current as App).OpenDPSWindow();
        }

        private void opensettings(object sender, RoutedEventArgs e)
        {
            (App.Current as App).OpenSettingsWindow();
        }

        private void openmap(object sender, RoutedEventArgs e)
        {
            (App.Current as App).OpenMapWindow();
        }

        private void openspells(object sender, RoutedEventArgs e)
        {
            (App.Current as App).OpenSpellsWindow();
        }

        private void openmobinfo(object sender, RoutedEventArgs e)
        {
            (App.Current as App).OpenMobInfoWindow();
        }

        private void copytoclipboard(object sender, RoutedEventArgs e)
        {
            var name = ((sender as Button).DataContext as dynamic)?.Name as string;

            var items = dPSWindowViewModel.EntityList.Where(a => a.TargetName == name);
            var fights = new List<string>();
            foreach (var item in items.OrderByDescending(a => a.PercentOfTotalDamage))
            {
                fights.Add($"{item.SourceName} {item.PercentOfTotalDamage}% DPS:{item.TotalDPS} DMG:{item.TotalDamage}");
            }
            var fightdetails = "Fight Details: " + name + " Dmg: " + (items.FirstOrDefault()?.TargetTotalDamage ?? 0) + "    " + string.Join(" / ", fights);
            System.Windows.Forms.Clipboard.SetText(fightdetails);
        }

        private void MoveCurrentToLastSession(object sender, RoutedEventArgs e)
        {
            dPSWindowViewModel.SessionPlayerDamage.LastSessionPlayerDamage = dPSWindowViewModel.SessionPlayerDamage.CurrentSessionPlayerDamage;
            dPSWindowViewModel.SessionPlayerDamage.CurrentSessionPlayerDamage = new PlayerDamage();
        }

        private void RemoveLastSession(object sender, RoutedEventArgs e)
        {
            dPSWindowViewModel.SessionPlayerDamage.LastSessionPlayerDamage = null;
        }

    }
}
