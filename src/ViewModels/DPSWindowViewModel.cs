﻿using EQTool.Models;
using EQTool.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace EQTool.ViewModels
{
    public class DPSWindowViewModel : INotifyPropertyChanged
    {
        private readonly IAppDispatcher appDispatcher;
        private readonly ActivePlayer activePlayer;

        public DPSWindowViewModel(IAppDispatcher appDispatcher, ActivePlayer activePlayer)
        {
            this.appDispatcher = appDispatcher;
            this.activePlayer = activePlayer;
        }

        public ObservableCollection<EntittyDPS> _EntityList = new ObservableCollection<EntittyDPS>();
        public ObservableCollection<EntittyDPS> EntityList
        {
            get => _EntityList;
            set
            {
                _EntityList = value;
                OnPropertyChanged();
            }
        }

        public void UpdateStuff()
        {
            OnPropertyChanged(nameof(EntityList));
        }

        private long? _LastReadOffset = null;
        public long? LastReadOffset
        {
            get => _LastReadOffset;
            set
            {
                _LastReadOffset = value;
                OnPropertyChanged();
            }
        }

        public void UpdateDPS()
        {
            appDispatcher.DispatchUI(() =>
            {
                var itemstormove = new List<EntittyDPS>();
                var now = DateTime.Now;
                foreach (var item in _EntityList)
                {
                    var lasttime = item.LastDamageDone ?? item.StartTime;
                    if (Math.Abs((now - lasttime).TotalSeconds) > 20)
                    {
                        itemstormove.Add(item);
                    }
                    else
                    {
                        item.UpdateDps(activePlayer.Player.Level);
                    }
                }

                foreach (var items in _EntityList.GroupBy(a => a.TargetName))
                {
                    var allhidden = false;
                    foreach (var group in items)
                    {
                        if (group.ColumnVisiblity == System.Windows.Visibility.Collapsed)
                        {
                            allhidden = true;
                        }
                    }

                    if (allhidden)
                    {
                        foreach (var group in items)
                        {
                            group.HeaderVisibility = System.Windows.Visibility.Collapsed;
                        }
                    }
                    else
                    {
                        foreach (var group in items)
                        {
                            group.HeaderVisibility = System.Windows.Visibility.Visible;
                        }
                    }
                }
                var groups = _EntityList.GroupBy(a => a.TargetName).ToList();
                foreach (var item in groups)
                {
                    var totaldmg = item.Sum(a => a.TotalDamage);
                    foreach (var e in item)
                    {
                        e.TargetTotalDamage = totaldmg;
                    }
                }

                foreach (var item in itemstormove)
                {
                    _ = EntityList.Remove(item);
                }
            });
        }

        public void TargetDied(string target)
        {
            if (string.IsNullOrWhiteSpace(target))
            {
                return;
            }

            appDispatcher.DispatchUI(() =>
            {
                var t = target.ToLower();
                var itemstoremove = EntityList.Where(a => a.TargetName.ToLower() == t).ToList();
                foreach (var item in itemstoremove)
                {
                    item.DeathTime = DateTime.Now;
                }
            });
        }

        public void TryAdd(DPSParseMatch entitiy, int playerlevel)
        {
            if (entitiy == null)
            {
                return;
            }

            appDispatcher.DispatchUI(() =>
            {
                var item = EntityList.FirstOrDefault(a => a.SourceName == entitiy.SourceName && a.TargetName == entitiy.TargetName);
                if (item == null)
                {
                    item = new EntittyDPS
                    {
                        SourceName = entitiy.SourceName,
                        TargetName = entitiy.TargetName,
                        StartTime = entitiy.TimeStamp,
                        TotalDamage = entitiy.DamageDone
                    };
                    EntityList.Add(item);
                }

                item.AddDamage(new EntittyDPS.DamagePerTime
                {
                    TimeStamp = entitiy.TimeStamp,
                    Damage = entitiy.DamageDone
                }, playerlevel);
            });
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
