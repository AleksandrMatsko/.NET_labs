using System.Drawing;
using CardLibrary;

namespace ColosseumTests;

[TestFixture]
public class CardTests
{
    [Test]
    public void Color_Has_SameColorPassedToConstructor()
    {
        var c = new Card(CardColor.Black, 1);
        Assert.That(c.Color, Is.EqualTo(CardColor.Black));
    }

    [Test]
    public void Number_Has_SameNumberPassedToConstructor()
    {
        var num = 1;
        var c = new Card(CardColor.Black, num);
        Assert.That(c.Number, Is.EqualTo(num));
    }
    
    [Test]
    public void Equals_CardWithNull_False()
    {
        var c = new Card(CardColor.Black, 1);
        Assert.That(c, Is.Not.EqualTo(null));
    }

    [Test]
    public void Equals_CardWithSelf_True()
    {
        var c = new Card(CardColor.Black, 1);
        Assert.That(c, Is.EqualTo(c));

        var a = c;
        Assert.That(c, Is.EqualTo(a));
    }
    
    [Test]
    public void Equals_CardWithDifferentColor_False()
    {
        var c1 = new Card(CardColor.Black, 1);
        var c2 = new Card(CardColor.Red, 1);
        
        Assert.That(c1, Is.Not.EqualTo(c2));
    }
    
    [Test]
    public void Equals_CardWithDifferentNumber_False()
    {
        var c1 = new Card(CardColor.Black, 1);
        var c2 = new Card(CardColor.Black, 2);
        
        Assert.That(c1, Is.Not.EqualTo(c2));
    }
    
    [Test]
    public void Equals_CardWithSameNumberAndColor_True()
    {
        var c1 = new Card(CardColor.Black, 1);
        var c2 = new Card(CardColor.Black, 1);
        
        Assert.That(c1, Is.EqualTo(c2));
    }
}