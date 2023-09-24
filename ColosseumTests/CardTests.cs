using System.Drawing;
using CardLibrary;

namespace ColosseumTests;

[TestFixture]
public class CardTests
{
    [Test]
    public void TestColorProperty()
    {
        var c = new Card(CardColor.Black, 1);
        Assert.That(c.Color, Is.EqualTo(CardColor.Black));
    }

    [Test]
    public void TestNumberProperty()
    {
        var num = 1;
        var c = new Card(CardColor.Black, num);
        Assert.That(c.Number, Is.EqualTo(num));
    }
    
    [Test]
    public void TestEqualsWithNull()
    {
        var c = new Card(CardColor.Black, 1);
        Assert.That(c, Is.Not.EqualTo(null));
    }

    [Test]
    public void TestEqualsWithSelf()
    {
        var c = new Card(CardColor.Black, 1);
        Assert.That(c, Is.EqualTo(c));

        var a = c;
        Assert.That(c, Is.EqualTo(a));
    }
}