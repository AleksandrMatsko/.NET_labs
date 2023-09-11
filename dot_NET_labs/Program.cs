using StrategyLibrary;

namespace dot_NET_labs;

class Program
{
    private const int NumExperiments = 1_000_000;
    
    public static void Main(string[] args)
    {
        var experiment = new SimpleExperiment(new PickFirstStrategy(), new PickFirstStrategy());
        var success = 0;
        for (int i = 0; i < NumExperiments; i++)
        {
            if (experiment.Do())
            {
                success += 1;
            }
        }
        Console.WriteLine($"Success percent: {(double)success / NumExperiments}");
    }
}