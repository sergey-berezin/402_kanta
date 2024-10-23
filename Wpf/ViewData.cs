using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Linq;
using Algorytm;
using OxyPlot;

namespace Wpf
{
    		public class Output_Data
	{
		public int epoch;
		public string metric;
		public string route;

		public Output_Data(int epoch, double metric, RouteData route)
		{
			this.epoch = epoch;
			this.metric = string.Format("Metric: {0:f3}", metric);
			this.route = route.ToString();
		}

		public override string ToString()
		{
			return "Epoch: " + epoch + " " + metric + "  " + route;
		}
	}

	public class ViewData : INotifyPropertyChanged, IDataErrorInfo
	{
		public event PropertyChangedEventHandler PropertyChanged;

		protected void RaisePropertyChanged([CallerMemberName] String propertyName = "") =>
		  PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

		public int selection_ { get; set; } = 5;
		public int selection
		{
			get { return selection_; }
			set
			{
				selection_ = value;
				if (PropertyChanged != null)
					PropertyChanged(this, new PropertyChangedEventArgs("selection"));
			}
		}

		public int citiescount_ { get; set; } = 15;
		public int citiescount
		{
			get { return citiescount_; }
			set
			{
				citiescount_ = value;
				if (PropertyChanged != null)
					PropertyChanged(this, new PropertyChangedEventArgs("citiescount"));
			}
		}

		public int populationcount_ { get; set; } = 10;
		public int populationcount
		{
			get { return populationcount_; }
			set
			{
				populationcount_ = value;
				if (PropertyChanged != null)
					PropertyChanged(this, new PropertyChangedEventArgs("populationcount"));
			}
		}

		private double mutationrate_ { get; set; } = 0.3;
		public double mutationrate
		{
			get { return mutationrate_; }
			set
			{
				mutationrate_ = value;
				if (PropertyChanged != null)
					PropertyChanged(this, new PropertyChangedEventArgs("mutationrate"));
			}
		}

		OXYPlot oxyPlotModel;

		private PlotModel _plot;
		public PlotModel plot
		{
			get { return _plot; }
			set
			{
				_plot = value;
				if (PropertyChanged != null)
					PropertyChanged(this, new PropertyChangedEventArgs("Plot"));
			}
		}

		private List<string> _Result;
		public List<string> Result
		{
			get { return _Result; }
			set
			{
				_Result = value;
				if (PropertyChanged != null)
					PropertyChanged(this, new PropertyChangedEventArgs("Result"));
			}
		}

		private List<double> _OutputData;

		public List<double> OutputData
		{
			get { return _OutputData; }
			set
			{
				_OutputData = value;
				if (PropertyChanged != null)
					PropertyChanged(this, new PropertyChangedEventArgs("OutputData"));
			}
		}

		public void Process(CancellationToken cancellationToken)
		{
			try
			{
				OutputData = new List<double>();

				var algorytm = new GenAlgorytm(citiescount, populationcount, mutationrate, selection);

				algorytm._bestRoute = null;
				algorytm._bestDistance = double.MaxValue;


				var epoch = 0;
				var population = algorytm.InitializePopulation();
				var prev_distance = double.MaxValue;

				while (true)
				{
					population = algorytm.Epoch(population);
					if (cancellationToken.IsCancellationRequested) break;
					epoch++;
					if (algorytm._bestDistance.Value != prev_distance)
					{
						Result = [(new Output_Data(epoch, algorytm._bestDistance.Value, algorytm._bestRoute).ToString())];
						prev_distance = algorytm._bestDistance.Value;
						OutputData.Add(algorytm._bestDistance.Value);
						oxyPlotModel = new OXYPlot(OutputData);
						plot = oxyPlotModel.plot;
					}
				}
				MessageBox.Show("Algorytm Terminated");
				MessageBox.Show($"{algorytm._bestDistance.Value} \n {algorytm._bestRoute}");
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Error while processing algorytm: {ex.Message}", "Calculation Error");
			}
		}

		public string Error { get { return "Error Text"; } }

		public string this[string property]
		{
			get
			{
				string msg = null;

				switch (property)
				{
					case "celection":
						if (selection <= 0 || selection > populationcount) msg = "Selection must be less than population and greater than 0";
						break;
					case "citiescount":
						if (citiescount <= 3) msg = "Cities must be greater than 2";
						break;
					case "populationcount":
						if (populationcount <= 0) msg = "Population must be greater than 0";
						break;
					case "mutationrate":
						if (mutationrate <= 0.0 || mutationrate > 1.0) msg = "Mutation Rate must be > 0 and <= 1";
						break;
					default:
						break;
				}
				if (msg != null)
					MessageBox.Show(msg);
				return msg;
			}
		}
	}
}
