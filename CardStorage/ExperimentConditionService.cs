using CardLibrary;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CardStorage;

public class ExperimentConditionService
{
    private readonly ExperimentConditionContext _experimentConditionContext;
    private readonly ILogger<ExperimentConditionService> _logger;

    public ExperimentConditionService(
        IDbContextFactory<ExperimentConditionContext> contextFactory, 
        ILogger<ExperimentConditionService> logger)
    {
        _experimentConditionContext = contextFactory.CreateDbContext();
        _logger = logger;
        _experimentConditionContext.Database.EnsureCreated();
    }

    public void RecreateDb()
    {
        _experimentConditionContext.Database.EnsureDeleted();
        _experimentConditionContext.Database.EnsureCreated();
    }

    public void AddOne(CardDeck deck)
    {
        _experimentConditionContext.Conditions.Add(ExperimentCondition.FromDeck(deck));
        _experimentConditionContext.SaveChanges();
    }

    public IList<CardDeck> GetN(int n)
    {
        
        var conditions = _experimentConditionContext.Conditions.OrderBy(c => c.Id).Take(n);
        var decks = new List<CardDeck>();
        if (!conditions.Any())
        {
            return decks;
        }

        foreach (var cond in conditions)
        {
            _experimentConditionContext.Entry(cond).Collection(c => c.CardEntities).Load();
            decks.Add(cond.ToDeck());
        }

        return decks;
    }
}