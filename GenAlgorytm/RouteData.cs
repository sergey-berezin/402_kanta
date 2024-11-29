using System;
using System.Text.Json.Serialization;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Algorytm
{
    public class RouteData
    {
        private static Random random = new Random();

        private int[] Cities_;
        public int[] Cities
        {
            get { return Cities_; }
        }

        //[JsonConstructor]
        //public RouteData() { }
        public RouteData(int[] Сities)
        {
            Cities_ = Сities;
        }

        public RouteData(int Cities)
        {
            Cities_ = Enumerable.Range(0, Cities).ToArray();
            Random.Shared.Shuffle(Cities_);
        }

        internal double TotalDistance(DistanceMatrix distanceMatrix)
        {
            var totalDistance = 0.0;
            for (int i = 0; i < distanceMatrix.Size - 1; i++)
            {
                totalDistance += distanceMatrix.GetDistance(Cities_[i], Cities_[i + 1]);
            }
            totalDistance += distanceMatrix.GetDistance(Cities_[Cities_.Length - 1], Cities_[0]);
            return totalDistance;
        }

        internal static RouteData Reproduction(RouteData parent1, RouteData parent2)
        {
            int index1 = random.Next(parent1.Cities_.Length / 2);
            int index2 = random.Next(index1 + 1, parent1.Cities_.Length);
            int[] childCities = new int[parent1.Cities_.Length];
            Array.Fill(childCities, -1);

            for (int i = index1; i <= index2; i++)
            {
                childCities[i] = parent1.Cities[i];
            }

            var j = 0;
            for (int i = 0; i < parent2.Cities_.Length; i++)
            {
                if (!childCities.Contains(parent2.Cities_[i]))
                {
                    while (childCities[j] != -1) j++;
                    childCities[j] = parent2.Cities_[i];
                }
            }
            return new RouteData(childCities);
        }

        internal void Mutate(double MutationRate)
        {
            if (MutationRate > random.NextDouble())
            {
                var index1 = random.Next(Cities_.Length);
                var index2 = random.Next(Cities_.Length);
                (Cities_[index1], Cities_[index2]) = (Cities_[index2], Cities_[index1]);
            }
        }

        public override string ToString()
        {
            string str = "Route: ";
            foreach (var city in Cities_)
            {
                str += city + ", ";
            }
            return str;
        }
    }
}
