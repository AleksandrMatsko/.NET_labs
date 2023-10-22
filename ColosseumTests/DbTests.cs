using CardLibrary;
using CardLibrary.Abstractions;
using CardLibrary.Impl;
using CardStorage;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace ColosseumTests;

[TestFixture]
public class DbTests
{
    private SqliteConnection _connection;
    private DbContextOptions<ExperimentConditionContext> _contextOptions;
    private readonly IDeckShuffler _shuffler = new RandomDeckShuffler();
    
    [SetUp]
    public void SetUp()
    {
        _connection = new SqliteConnection("Data source=:memory:");
        _connection.Open();

        _contextOptions = new DbContextOptionsBuilder<ExperimentConditionContext>()
            .UseSqlite(_connection)
            .Options;
    }

    private ExperimentConditionContext CreateContext() => new ExperimentConditionContext(_contextOptions);

    [TearDown]
    public void Dispose()
    {
        _connection.Dispose();
    }

    private Mock<IDbContextFactory<ExperimentConditionContext>> DbFactoryMock()
    {
        var factoryMock = new Mock<IDbContextFactory<ExperimentConditionContext>>();
        factoryMock.Setup(f => f.CreateDbContext()).Returns(CreateContext());
        return factoryMock;
    }
    
    [Test]
    public void ExperimentConditionService_CallsCreateDbContext_OnlyOnce()
    {
        var factoryMock = DbFactoryMock();

        var service = new ExperimentConditionService(
            factoryMock.Object,
            new Logger<ExperimentConditionService>(new LoggerFactory()));
        
        Assert.DoesNotThrow(() =>
        {
            factoryMock.Verify(f => f.CreateDbContext(), Times.Once);
        });
    }
    
    [Test]
    public void ExperimentConditionService_AddOne_AddOneExperimentCondition()
    {
        var factoryMock = DbFactoryMock();
        var service = new ExperimentConditionService(
            factoryMock.Object,
            new Logger<ExperimentConditionService>(new LoggerFactory()));
        var deck = Shuffleable36CardDeck.CreateCardDeck();
        _shuffler.Shuffle(deck);
        
        service.AddOne(deck);

        var context = CreateContext();
        context.Database.EnsureCreated();
        
        var condition = context.Find<ExperimentCondition>(1);
        
        Assert.That(condition, Is.Not.Null);
        
        context.Entry(condition!).Collection(c => c.CardEntities).Load();
        
        var entities = condition!.CardEntities;
        
        for (var i = 0; i < deck.Length; i++)
        {
            if (deck[i].Color != entities[i].Color || deck[i].Number != entities[i].Number)
            {
                Assert.Fail("card values don't match");
            }
        }
        Assert.Pass();
    }

    [TestCase(1)]
    [TestCase(2)]
    [TestCase(100)]
    public void ExperimentConditionService_GetFirstN_ReturnsNSameCardDecks(int n)
    {
        var conditionsList = new List<ExperimentCondition>();
        for (var i = 0; i < n; i++)
        {
            var deck = Shuffleable36CardDeck.CreateCardDeck();
            _shuffler.Shuffle(deck);
            conditionsList.Add(ExperimentCondition.FromDeck(deck));
        }
        var factoryMock = DbFactoryMock();
        
        var context = CreateContext();
        context.Database.EnsureCreated();
        context.AddRange(conditionsList);
        context.SaveChanges();
        
        var service = new ExperimentConditionService(
            factoryMock.Object,
            new Logger<ExperimentConditionService>(new LoggerFactory()));

        var decks = service.GetFirstN(n);
        
        Assert.That(decks, Has.Count.EqualTo(n));

        for (var i = 0; i < decks.Count; i++)
        {
            if (decks[i].Length != conditionsList[i].CardEntities.Count)
            {
                Assert.Fail($"deck lengths are not equal on iteration {i}");
            }

            for (var j = 0; j < decks[i].Length; j++)
            {
                var card = decks[i][j];
                var entity = conditionsList[i].CardEntities[j];
                if (card.Color != entity.Color || card.Number != entity.Number)
                {
                    Assert.Fail($"card[{j}] is not equal to relative entity in deck {i}");
                }
            }
        }
        Assert.Pass();
    }
}