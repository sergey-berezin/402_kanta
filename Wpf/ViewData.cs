using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using Algorytm;
using Newtonsoft.Json;
using OxyPlot;
using WpfApp;

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

    public struct Algorytm_data
    {
        public int selection { get; set; }
        public int citiescount { get; set; }
        public int populationcount { get; set; }
        public double mutationrate { get; set; }
        public List<string> result { get; set; }
        public List<double> OutputData { get; set; }
        public int[] bestRoute { get; set; }
        public double? bestDistance { get; set; }
        public List<int[]> population { get; set; }
        public int epoch { get; set; }
        public double[][] matrix { get; set; }
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

        private Algorytm_data _data_to_json;
        public Algorytm_data data_to_json
        {
            get { return _data_to_json; }
            set
            {
                _data_to_json = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("data_to_json"));
            }
        }

        private bool _loaded { get; set; } = false;
        public bool loaded
        {
            get { return _loaded; }
            set
            {
                _loaded = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("loaded"));
            }
        }

        public void Process(CancellationToken cancellationToken)
        {
            try
            {
                if (!loaded)
                {
                    OutputData = new List<double>();
                }

                var algorytm = new GenAlgorytm(citiescount, populationcount, mutationrate, selection);

                algorytm._bestRoute = null;
                algorytm._bestDistance = double.MaxValue;


                var epoch = 0;
                var population = algorytm.InitializePopulation();
                var prev_distance = double.MaxValue;

                if (loaded)
                {
                    //Load Algorytm state
                    algorytm._bestRoute = new RouteData(data_to_json.bestRoute);
                    algorytm._bestDistance = data_to_json.bestDistance;
                    algorytm.distanceMatrix_ = new DistanceMatrix(citiescount, data_to_json.matrix);

                    population = new List<RouteData>();
                    foreach (var el in data_to_json.population)
                    {
                        population.Add(new RouteData(el));
                    }
                    epoch = data_to_json.epoch;
                    prev_distance = algorytm._bestDistance.Value;
                }
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

                var pop = new List<int[]>();
                foreach (var el in population)
                {
                    pop.Add(el.Cities);
                }
                data_to_json = new Algorytm_data()
                {
                    selection = selection,
                    citiescount = citiescount,
                    populationcount = populationcount,
                    mutationrate = mutationrate,
                    OutputData = OutputData,
                    bestRoute = algorytm._bestRoute.Cities,
                    bestDistance = algorytm._bestDistance,
                    population = pop,
                    epoch = epoch,
                    matrix = algorytm.distanceMatrix_.matrix,
                    result = Result
                };

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error while processing algorytm: {ex.Message}", "Calculation Error");
            }
        }

        public void LoadJson(string filename)
        {
            try
            {
                using (StreamReader r = new StreamReader(filename))
                {
                    string json = r.ReadToEnd();
                    data_to_json = JsonConvert.DeserializeObject<Algorytm_data>(json);
                    loaded = true;

                    //Load algorytm parameters
                    OutputData = data_to_json.OutputData;
                    citiescount = data_to_json.citiescount;
                    populationcount = data_to_json.populationcount;
                    mutationrate = data_to_json.mutationrate;
                    selection = data_to_json.selection;

                    //Screen update
                    Result = data_to_json.result;
                    oxyPlotModel = new OXYPlot(OutputData);
                    plot = oxyPlotModel.plot;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading data: {ex.Message}", "Load Error", MessageBoxButton.OK, MessageBoxImage.Error);
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
