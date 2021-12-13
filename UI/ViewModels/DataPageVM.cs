﻿using Core.Librarys;
using Core.Librarys.Image;
using Core.Servicers.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using UI.Controls;
using UI.Controls.Charts.Model;
using UI.Librays;
using UI.Models;
using UI.Views;

namespace UI.ViewModels
{
    public class DataPageVM : DataPageModel
    {
        public Command ToDetailCommand { get; set; }
        private readonly IData data;
        private readonly MainViewModel main;

        public DataPageVM(IData data, MainViewModel main)
        {
            this.data = data;
            this.main = main;

            ToDetailCommand = new Command(new Action<object>(OnTodetailCommand));

            Init();
        }

        private void Init()
        {
            LoadData(DateTime.Now.Date);
            PropertyChanged += DataPageVM_PropertyChanged;

            TabbarData = new System.Collections.ObjectModel.ObservableCollection<string>()
            {
                "按天查看","按月查看"
            };

            TabbarSelectedIndex = 0;
            DatePickerType = Controls.DatePickerBar.DatePickerShowType.Day;
        }

        private void OnTodetailCommand(object obj)
        {
            var data = obj as ChartsDataModel;

            if (data != null)
            {
                main.Data = data;
                main.Uri = nameof(DetailPage);
            }
        }

        private void DataPageVM_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Date))
            {
                LoadData(Date);
            }

            if (e.PropertyName == nameof(TabbarSelectedIndex))
            {
                if (TabbarSelectedIndex == 0)
                {
                    DatePickerType = Controls.DatePickerBar.DatePickerShowType.Day;
                }
                else
                {
                    DatePickerType = Controls.DatePickerBar.DatePickerShowType.Month;
                }
            }
        }



        #region 读取数据




        private void LoadData(DateTime date)
        {
            DateTime dateStart = date, dateEnd = date;
            if (TabbarSelectedIndex == 1)
            {
                dateStart = new DateTime(date.Year, date.Month, 1);
                dateEnd = new DateTime(date.Year, date.Month, DateTime.DaysInMonth(date.Year, date.Month));
            }
            var list = data.GetDateRangelogList(dateStart, dateEnd);
            Data = MapToChartsData(list);
        }

        #region 处理数据
        private List<ChartsDataModel> MapToChartsData(List<Core.Models.DailyLogModel> list)
        {
            var resData = new List<ChartsDataModel>();

            foreach (var item in list)
            {
                var bindModel = new ChartsDataModel();
                bindModel.Name = string.IsNullOrEmpty(item.ProcessDescription) ? item.ProcessName : item.ProcessDescription;
                bindModel.Value = item.Time;
                bindModel.Tag = Timer.Fromat(item.Time);
                bindModel.PopupText = item.File;
                bindModel.Icon = Iconer.Get(item.ProcessName, item.ProcessDescription);
                resData.Add(bindModel);
            }

            return resData;
        }
        #endregion
        #endregion
    }
}