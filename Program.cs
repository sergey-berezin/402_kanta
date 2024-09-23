using Algorytm;

class Program
{
    static void Main()
    {
        int NCities = 14;
        double bestResult = Double.MaxValue;

        var distanceMatrix = DistanceMatrix.Initialise(NCities);

        var algorytm = new GenAlgorytm(distanceMatrix, 5, 0.5);
        algorytm.AlgorytmOutput += Output;

        Console.CancelKeyPress += new ConsoleCancelEventHandler(myHandler);

        RouteData bestRoute = algorytm.Run(out bestResult);

        Console.WriteLine(bestRoute);
        Console.WriteLine($"Best result:  {bestResult:N4}");
        Console.WriteLine(distanceMatrix);

        void myHandler(object sender, ConsoleCancelEventArgs args)
        {
            Console.WriteLine($"\n  Key pressed: {args.SpecialKey}. Algorytm terminated \n");
            args.Cancel = true;
            algorytm.isRunning = false;
        }
    }
    private static void Output(int iteration, double bestDistance)
    {
        Console.WriteLine("epochs: {0}, best distance: {1:N3}", iteration, bestDistance);
    }
}
