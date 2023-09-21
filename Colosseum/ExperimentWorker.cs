namespace Colosseum;

public class ExperimentWorker
{
    public int ExperimentCount { get; }

    public ExperimentWorker(int experimentCount)
    {
        ExperimentCount = experimentCount;
    }

    public int Run(IExperiment experiment)
    {
        var success = 0;
        for (int i = 0; i < ExperimentCount; i++)
        {
            if (experiment.Do())
            {
                success += 1;
            }
        }
        
        return success;
    }
}