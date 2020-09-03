using System;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Cryptography.X509Certificates;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Dht22SensorChart;
using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Helpers;
using LiveCharts.Wpf;
using Microsoft.Windows.Input;
using MySqlConnectionLibrary;
using System.Collections.Generic;
using MySql.Data.MySqlClient;

namespace Wpf.CartesianChart.DateAxis
{
    public partial class DateAxisExample : UserControl
    {
        public int MaxValue;
        public SeriesCollection SeriesCollection { get; set; }

        public DateAxisExample()
        {
            InitializeComponent();

            ChartValues<double> humidityCv = new ChartValues<double>();
            ChartValues<double> temperatureCv = new ChartValues<double>();

            SeriesCollection = new SeriesCollection
            {
                new LineSeries
                {
                    Title = "Humidity",
                    Values = humidityCv,
                    ScalesYAt = 0,
                    Stroke = Brushes.Blue,
                    Fill = Brushes.Transparent,
                },

                new LineSeries
                {
                    Title = "Temperature",
                    Values = temperatureCv,
                    ScalesYAt = 1,
                    Stroke = Brushes.Red,
                    Fill = Brushes.Transparent,
                }
            };
            UpdateSeries(360);
            DataContext = this;
            TimeRange.Text = "TIME RANGE: 6 HOURS";
        }

        public void UpdateSeries(int maxValue)
        {
            DataAccess Da = new DataAccess();
            Da.Query(maxValue);
            Da.SqlReader.Read();
            List<ChartValues<double>> chartValues = Da.CleanData();
            SeriesCollection[0].Values = chartValues[0];
            SeriesCollection[1].Values = chartValues[1];
            MaxValue = SeriesCollection[0].Values.Count;
        }

        private void SixHours_Button(object sender, RoutedEventArgs e)
        {
            UpdateSeries(360);
            TimeRange.Text = "TIME RANGE: 6 HOURS";
        }

        private void TwelveHours_Button(object sender, RoutedEventArgs e)
        {
            UpdateSeries(720);
            TimeRange.Text = "TIME RANGE: 12 HOURS";
        }

        private void TwentyFourHours_Button(object sender, RoutedEventArgs e)
        {
            UpdateSeries(1440);
            TimeRange.Text = "TIME RANGE: 24 HOURS";
        }

        private void CustomRangeGo_Button(object sender, RoutedEventArgs e)
        {
            try
            {
                int hoursToScans = int.Parse(CustomRange_TextBox.Text) * 60;
                UpdateSeries(hoursToScans);
                TimeRange.Text = $"TIME RANGE: {CustomRange_TextBox.Text} HOURS";
            }
            catch
            {

            }
        }
    }  
}