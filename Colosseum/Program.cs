﻿using CardLibrary;
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
        var success = 0;
        for (int i = 0; i < NumExperiments; i++)
        {
            if (experiment.Do())
            {
                success += 1;
            }
        }
        Console.WriteLine($"Success rate: {(double)success / NumExperiments}");
    }
}