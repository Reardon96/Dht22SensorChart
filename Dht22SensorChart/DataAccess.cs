using System;
using System.Collections.Generic;
using System.Text;
using LiveCharts;
using MySql.Data.MySqlClient;
using MySqlConnectionLibrary;

namespace Dht22SensorChart
{
    public class DataAccess
    {
        public MySqlMain SqlMain = new MySqlMain();
        public MySqlDataReader SqlReader;

        public DataAccess()
        {
            Connect();
        }

        public void Connect()
        {
            SqlMain.Connect("raspberrypi", "RASPBERRY_PI", "sa", "password");
        }

        public void Query(int maxValue)
        {
            SqlReader = SqlMain.Query($"SELECT * FROM Dht22SensorData ORDER BY DateTimeReading DESC LIMIT {maxValue}");
        }

        public List<ChartValues<double>> CleanData()
        {
            ChartValues<double> humidityCv = new ChartValues<double>();
            ChartValues<double> temperatureCv = new ChartValues<double>();
            double[] prev_read = { SqlReader.GetDouble(0), SqlReader.GetDouble(0) };
            bool unclean;
            while (SqlReader.Read())
            {
                unclean = false;
                for (int i = 0; i < prev_read.Length - 1; i++)
                {
                    unclean = (Math.Abs(prev_read[i] - SqlReader.GetDouble(i)) / prev_read[i]) > 0.1;
                    if (unclean)
                    {
                        break;
                    }
                }
                if (unclean)
                {
                    continue;
                }

                prev_read[0] = SqlReader.GetDouble(0);
                prev_read[1] = SqlReader.GetDouble(1);

                humidityCv.Add(SqlReader.GetDouble(0));
                temperatureCv.Add(SqlReader.GetDouble(1));
            }
            List<ChartValues<double>> chartValues = new List<ChartValues<double>> { humidityCv, temperatureCv };
            return chartValues;
        }
    }
}
