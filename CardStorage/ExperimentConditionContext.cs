using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CardStorage;

public class ExperimentConditionContext : DbContext
{
    public DbSet<ExperimentCondition> Conditions { get; set; }
    public string DbPath { get; }

    private readonly ILogger<ExperimentConditionContext> _logger;

    public ExperimentConditionContext(ILogger<ExperimentConditionContext> logger)
    {
        var folder = Environment.SpecialFolder.LocalApplicationData;
        var path = Environment.GetFolderPath(folder);
        DbPath = Path.Join(path, "cards.db");
        _logger = logger;
        _logger.LogInformation($"Db path is: {DbPath}");
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        _logger.LogInformation($"Db configuring");
        optionsBuilder.UseSqlite($"Data Source={DbPath}");
    }
}