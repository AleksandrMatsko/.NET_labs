using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CardStorage;

public class ExperimentConditionContext : DbContext
{
    public DbSet<ExperimentCondition> Conditions { get; set; }

    public ExperimentConditionContext(DbContextOptions<ExperimentConditionContext> options) : base(options) {}
}