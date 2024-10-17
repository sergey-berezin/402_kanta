using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Algorytm
{
    public class GenAlgorytm
    {

        public readonly int populationCount_;
        public readonly double mutationFrequency_;

        public readonly DistanceMatrix distanceMatrix_;
        public readonly int citiesCount_;
        public readonly int selection_;

        public RouteData? _bestRoute = null;
        public double? _bestDistance = null;

        //public bool isRunning = true;
        //public event Action<double, RouteData>? AlgorytmOutput;

        public GenAlgorytm(int NCities, int populationCount, double mutationFrequency, int selection_)
        {
            populationCount_ = populationCount;
            mutationFrequency_ = mutationFrequency;
            distanceMatrix_ = DistanceMatrix.Initialise(NCities);
            citiesCount_ = distanceMatrix_.Size;
            this.selection_ = selection_;
        }

        public List<RouteData> InitializePopulation()
        {
            List<RouteData> population = new List<RouteData>();

            for (int i = 0; i < populationCount_; i++)
            {
                RouteData newRoute = new RouteData(citiesCount_);
                population.Add(newRoute);
            }
            return population;
        }
        public RouteData Selection(List<RouteData> population, int Nparticipants)
        {
            Random random = new Random();
            var selection = new List<RouteData>();
            for (int i = 0; i < Nparticipants; i++)
            {
                int randomIndex = random.Next(populationCount_);
                selection.Add(population[randomIndex]);
            }
            return selection.OrderBy(route => route.TotalDistance(distanceMatrix_)).First();
        }

        public List<RouteData> Epoch(List<RouteData> population)
        {
            ConcurrentBag<RouteData> newPopulation = new ConcurrentBag<RouteData>();

            RouteData bestInGeneration = population.OrderBy(route => route.TotalDistance(distanceMatrix_)).First();
            newPopulation.Add(bestInGeneration);

            Parallel.For(0, populationCount_, i =>
            {
                RouteData parent1 = Selection(population, selection_);
                RouteData parent2 = Selection(population, selection_);

                RouteData child = RouteData.Reproduction(parent1, parent2);
                child.Mutate(mutationFrequency_);
                newPopulation.Add(child);
            });

            foreach (var route in newPopulation)
            {
                double routeDistance = route.TotalDistance(distanceMatrix_);
                if (routeDistance < _bestDistance)
                {
                    _bestRoute = route;
                    _bestDistance = routeDistance;
                }
            }
            return newPopulation.ToList();
        }

        //public RouteData? Run(out double bestDistance)
        //{
        //    _bestRoute = null;
        //    _bestDistance = double.MaxValue;

        //    var epoch = 0;
        //    var population = InitializePopulation();
        //    var prev_distance = double.MaxValue;

        //    Task task = new Task(() =>
        //    {
        //        while (true)
        //        {
        //            population = Epoch(population);
        //            if (cancellationToken.IsCancellationRequested) break;
        //            epoch++;
        //            if (_bestDistance.Value != prev_distance)
        //            {
        //                prev_distance = _bestDistance.Value;
        //                AlgorytmOutput?.Invoke(_bestDistance.Value, _bestRoute);
        //            }
        //        }
        //    }, token);
        //    task.Start();

        //    bestDistance = _bestDistance.Value;
        //    return _bestRoute;
        //}
    }
}
