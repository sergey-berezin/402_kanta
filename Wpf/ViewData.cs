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
        public double metric;
        public RouteData route;

        public Output_Data(double metric, RouteData route)
        {
            this.metric = metric;
            this.route = route;
        }

        public override string ToString()
        {
            return metric.ToString() +" " + route.ToString();
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


        double bestResult;
        private RouteData _Result;
        public RouteData Result
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
        CancellationTokenSource cts;
        public void ProgramStart()
        {
            cts = new CancellationTokenSource();

            // Запускаем асинхронную операцию
            var task = Task.Run(() => Process(cts.Token), cts.Token);
        }

        public void ProgramCancel()
        {
            // Запрашиваем отмену операции
            cts.Cancel();
            cts.Dispose();
        }

        public void Process(CancellationToken cancellationToken)
        {
            try
            {
                OutputData = new List<double>();
                //for (var i = 0; i < 20; i++)
                //{
                //    OutputData.Add(i);
                //}

                bestResult = Double.MaxValue;

                var algorytm = new GenAlgorytm(citiescount, populationcount, mutationrate, selection);
                //algorytm.AlgorytmOutput += Output;

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
                        prev_distance = algorytm._bestDistance.Value;
                        OutputData.Add(algorytm._bestDistance.Value);
                    }
                }
                Result = algorytm._bestRoute;
                //RouteData bestRoute = algorytm.Run(out bestResult);
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
