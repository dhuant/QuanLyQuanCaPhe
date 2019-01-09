using QLCafe.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QLCafe.Strategy
{
    class ThongkeDoanhThuStrategy
    {
        private Strategy strategy;

        public Strategy Strategy
        {
            get { return strategy; }
            set { strategy = value; }
        }

        public void ThongKe(string typeDatepicker,List<ThongKeBill> thongKeBill ,System.Windows.Forms.DataVisualization.Charting.Chart chartBill)
        {
            switch (typeDatepicker)
            {
                case "Ngày":
                    this.Strategy = new ThongkeByColumn();
                    break;
                case "Năm":
                    this.Strategy = new ThongkeByColumn();
                    break;
                case "Tháng":
                    this.Strategy = new ThongkeByLine();
                    break;
                default:
                    break;
            }
            int i= 0;
            this.Strategy.AlgorithmInterface(chartBill);
            foreach (var item in thongKeBill)
            {
                var time = item.DateCheckout.Split(' ')[0];
                chartBill.Series["Bill"].Points.AddXY(time, item.TotalPrice);
                i++;
            }
        }
    }

    abstract class Strategy
    {
        public abstract void AlgorithmInterface(System.Windows.Forms.DataVisualization.Charting.Chart chartBill);
    }

    class ThongkeByColumn : Strategy
    {
        public override void AlgorithmInterface(System.Windows.Forms.DataVisualization.Charting.Chart chartBill)
        {
            chartBill.Series[0].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Column;
        }
    }

    class ThongkeByLine : Strategy
    {
        public override void AlgorithmInterface(System.Windows.Forms.DataVisualization.Charting.Chart chartBill)
        {
            chartBill.Series[0].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
        }
    }
  
}
