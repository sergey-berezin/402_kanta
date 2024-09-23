using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algorytm
{
    public class GenAlgorytm
    {
        private Random random = new Random();
        private readonly int populationCount_;
        private readonly double mutationFrequency_;

        private readonly DistanceMatrix distanceMatrix_;
        private readonly int citiesCount_;

        private RouteData? _bestRoute = null;
        private double? _bestDistance = null;

        public bool isRunning = true;
        public event Action<int, double>? AlgorytmOutput;


        public GenAlgorytm(DistanceMatrix distancematrix, int populationCount, double mutationFrequency)
        {
            populationCount_ = populationCount;
            mutationFrequency_ = mutationFrequency;
            distanceMatrix_ = distancematrix;
            citiesCount_ = distancematrix.Size;
        }

        private List<RouteData> InitializePopulation()
        {
            List<RouteData> population = new List<RouteData>();

            for (int i = 0; i < populationCount_; i++)
            {
                RouteData newRoute = new RouteData(citiesCount_);
                population.Add(newRoute);
            }
            return population;
        }
        private RouteData Selection(List<RouteData> population, int Nparticipants)
        {
            var selection = new List<RouteData>();
            for (int i = 0; i < Nparticipants; i++)
            {
                int randomIndex = random.Next(populationCount_);
                selection.Add(population[randomIndex]);
            }
            return selection.OrderBy(route => route.TotalDistance(distanceMatrix_)).First();
        }

        private List<RouteData> Epoch(List<RouteData> population)
        {
            List<RouteData> newPopulation = new List<RouteData>();

            RouteData bestInGeneration = population.OrderBy(route => route.TotalDistance(distanceMatrix_)).First();
            newPopulation.Add(bestInGeneration);

            for (int i = 0; i < populationCount_; i++)
            {
                RouteData parent1 = Selection(population, 3);
                RouteData parent2 = Selection(population, 3);

                RouteData child = RouteData.Reproduction(parent1, parent2);
                child.Mutate(mutationFrequency_);
                newPopulation.Add(child);
            }

            foreach (var route in newPopulation)
            {
                double routeDistance = route.TotalDistance(distanceMatrix_);
                if (routeDistance < _bestDistance)
                {
                    _bestRoute = route;
                    _bestDistance = routeDistance;
                }
            }
            return newPopulation;
        }

        public RouteData? Run(out double bestDistance)
        {
            _bestRoute = null;
            _bestDistance = double.MaxValue;

            var epoch = 0;
            var population = InitializePopulation();

            while (true)
            {
                population = Epoch(population);
                if (!isRunning) break;
                epoch++;
                AlgorytmOutput?.Invoke(epoch, _bestDistance.Value);
            }

            bestDistance = _bestDistance.Value;
            return _bestRoute;
        }
    }
}
