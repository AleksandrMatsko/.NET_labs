using CardLibrary;
using StrategyLibrary;

namespace Colosseum;

class Program
{
    private const int NumExperiments = 1_000_000;
    
    public static void Main(string[] args)
    {
        var experiment = new SimpleExperiment(
            new RandomDeckShuffler(), 
            new Player("Elon Mask", new PickFirstCardPickStrategy()), 
            new Player("Mark Zuckerberg", new PickFirstCardPickStrategy())
            );
        var worker = new ExperimentWorker(NumExperiments);
        var success = worker.Run(experiment);
        Console.WriteLine($"Success rate: {(double)success / NumExperiments}");
    }
}