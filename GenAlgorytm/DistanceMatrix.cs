using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Algorytm
{
    public class DistanceMatrix
    {
        private double[][] _matrix;
        private readonly int _size;
        public int Size
        {
            get { return _size; }
        }

        public DistanceMatrix(int numCities)
        {
            _matrix = new double[numCities][];
            _size = numCities;
        }

        public DistanceMatrix(int numCities, double[][] matrix)
        {
            _matrix = matrix;
            _size = numCities;
        }

        public static DistanceMatrix Initialise(int NCities)
        {
            var distanceMatrix = new DistanceMatrix(NCities);

            Random random = new Random();
            for (int i = 0; i < NCities - 1; i++)
            {
                double[] newRow = new double[NCities - i - 1];
                for (int j = 0; j < NCities - i - 1; j++)
                {
                    var new_elem = random.NextDouble() * 100;
                    for (int k = 0; k < i; k++)
                        new_elem = Math.Min(new_elem, distanceMatrix.GetDistance(k, i) + distanceMatrix.GetDistance(k, j));
                    newRow[j] = new_elem;
                }

                distanceMatrix._matrix[i] = newRow;
            }
            return distanceMatrix;
        }

        public double GetDistance(int i, int j)
        {
            if (i == j)
                return 0.0;
            else if (i > j)
                return _matrix[j][i - j - 1];
            else
                return _matrix[i][j - i - 1];
        }

        public override string ToString()
        {
            string result = "\nDistance Matrix:\n";
            for (int i = 0; i < Size - 1; i++)
            {
                for (int j = 0; j < _matrix[i].Length; j++)
                {
                    result += _matrix[i][j].ToString("N3") + "\t";
                }
                result += "\n";
            }
            return result;
        }
    }
}
