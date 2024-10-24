using OxyPlot;
using OxyPlot.Series;
using OxyPlot.Legends;
using System.Windows;
using OxyPlot.Axes;
using Wpf;

namespace WpfApp
{
    public class OXYPlot
    {
        List<double> data;

        public PlotModel plot { get; private set; }

        public OXYPlot(List<double> data)
        {
            this.plot = new PlotModel { Title = "Metric" };
            this.data = data;
            AddSeries();
        }
        public PlotModel addMarkers(PlotModel model, List<double> d, OxyColor colorValue)
        {
            var lineSeries = new ScatterSeries { MarkerType = MarkerType.Circle, MarkerStroke = colorValue, MarkerFill = colorValue };
            for (int i = 0; i < d.Count; i++)
            {
                lineSeries.Points.Add(new ScatterPoint(i, d[i]));
            }
            model.Legends.Add(new Legend());
            model.Series.Add(lineSeries);
            return model;
        }
        public PlotModel addLine(PlotModel model, List<double> d, double lineThick, OxyColor colorValue)
        {
            var lineSeries = new LineSeries { StrokeThickness = lineThick, Color = colorValue };
            for (int i = 0; i < d.Count; i++)
            {
                lineSeries.Points.Add(new DataPoint(i, d[i]));
            }
            model.Series.Add(lineSeries);
            return model;
        }

        public void AddSeries()
        {
            try
            {
                var RefinedGridLine = new PlotModel();
                RefinedGridLine.Axes.Add(new LinearAxis
                {
                    Position = AxisPosition.Left,
                    Title = "Metric",
                });
                RefinedGridLine.Axes.Add(new LinearAxis
                {
                    Position = AxisPosition.Bottom,
                    Title = "X",
                });
                RefinedGridLine = addLine(RefinedGridLine, data, 2, OxyColors.Green);
                RefinedGridLine.Legends.Add(new OxyPlot.Legends.Legend());
                RefinedGridLine = addMarkers(RefinedGridLine, data, OxyColors.Red);
                plot = RefinedGridLine;
            }
            catch (Exception ex) { MessageBox.Show("from RefinedGridLine"); MessageBox.Show(ex.Message); }
        }
    }
}
