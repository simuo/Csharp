using LiveCharts;
using LiveCharts.Configurations;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using xxw.utilities;

namespace OQC_OUT
{
    public class InterfaceTimeChart : BaseModel
    {
        public ChartValues<MeasureModel> GetBandChartValues { get; set; }
        public ChartValues<MeasureModel> ProcessControlChartValues { get; set; }
        public ChartValues<MeasureModel> PostTraceChartValues { get; set; }
        public ChartValues<MeasureModel> PostJGPChartValues { get; set; }
        public ChartValues<MeasureModel> PostIFactoryChartValues { get; set; }
        public Func<double, string> DateTimeFormatter { get; set; }
        public double AxisStep { get; set; }
        public double AxisUnit { get; set; }
        private double _axisMax;
        private double _axisMin;
        public double AxisMax
        {
            get { return _axisMax; }
            set
            {
                _axisMax = value;
                OnPropertyChanged("AxisMax");
            }
        }
        public double AxisMin
        {
            get { return _axisMin; }
            set
            {
                _axisMin = value;
                OnPropertyChanged("AxisMin");
            }
        }
        public DateTime LastRefreshDateTime { get; set; }
        public void Init()
        {
            var mapper = Mappers.Xy<MeasureModel>()
                .X(model => model.DateTime.Ticks)   //use DateTime.Ticks as X
                .Y(model => model.Value);           //use the value property as Y

            //lets save the mapper globally.
            Charting.For<MeasureModel>(mapper);

            //the values property will store our values array
            GetBandChartValues = new ChartValues<MeasureModel>();
            ProcessControlChartValues = new ChartValues<MeasureModel>();
            PostTraceChartValues = new ChartValues<MeasureModel>();
            PostJGPChartValues = new ChartValues<MeasureModel>();
            PostIFactoryChartValues = new ChartValues<MeasureModel>();

            //lets set how to display the X Labels
            DateTimeFormatter = value => new DateTime((long)value).ToString("mm:ss");

            //AxisStep forces the distance between each separator in the X axis
            AxisStep = TimeSpan.FromSeconds(1).Ticks;
            //AxisUnit forces lets the axis know that we are plotting seconds
            //this is not always necessary, but it can prevent wrong labeling
            AxisUnit = TimeSpan.TicksPerSecond;

            Refresh(DateTime.Now.AddSeconds(-15), DateTime.Now);
        }
        private void SetAxisLimits(DateTime begin, DateTime end)
        {
            AxisMax = end.Ticks + TimeSpan.FromSeconds(1).Ticks; // lets force the axis to be 1 second ahead
            AxisMin = begin.Ticks - TimeSpan.FromSeconds(1).Ticks; // and 30 seconds behind
        }
        public void Refresh(DateTime begin, DateTime end)
        {
            GetBandChartValues.Clear();
            ProcessControlChartValues.Clear();
            PostTraceChartValues.Clear();
            PostJGPChartValues.Clear();
            PostIFactoryChartValues.Clear();
            //查询数据
            var list = new DbContext().Read(db => db.InterfaceTimeDb
            .GetList(p => SqlFunc.Between(p.CreateTime, begin, end)));
            //var db = new DbContext().InterfaceTimeDb;
            //var list = db.GetList(p => SqlFunc.Between(p.CreateTime, begin, end));
            foreach (var one in list)
            {
                switch (one.InterfaceType)
                {
                    case "GetBand":
                        GetBandChartValues.Add(new MeasureModel
                        {
                            DateTime = one.CreateTime,
                            Value = one.RequestTime
                        });
                        break;
                    case "ProcessControl":
                        ProcessControlChartValues.Add(new MeasureModel
                        {
                            DateTime = one.CreateTime,
                            Value = one.RequestTime
                        });
                        break;
                    case "PostTrace":
                        PostTraceChartValues.Add(new MeasureModel
                        {
                            DateTime = one.CreateTime,
                            Value = one.RequestTime
                        });
                        break;
                    case "PostJGP":
                        PostJGPChartValues.Add(new MeasureModel
                        {
                            DateTime = one.CreateTime,
                            Value = one.RequestTime
                        });
                        break;
                    case "PostIFactory":
                        PostIFactoryChartValues.Add(new MeasureModel
                        {
                            DateTime = one.CreateTime,
                            Value = one.RequestTime
                        });
                        break;
                }
            }
            SetAxisLimits(begin, end);
        }
    }
    public class MeasureModel
    {
        public DateTime DateTime { get; set; }
        public double Value { get; set; }
    }
}
