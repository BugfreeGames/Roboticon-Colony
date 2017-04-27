using NUnit.Framework;

/// <summary>
/// JBT created all new unit tests, as Bugfree chose to create their own testing framework instead of using an existing one
/// </summary>
public class ResourceGroupTest
{
    ResourceGroup TestGroup1 = new ResourceGroup(5,5,5);
    ResourceGroup TestGroup2 = new ResourceGroup(10, 10, 10);
    ResourceGroup TestGroup3 = new ResourceGroup(25, 25, 25);
    ResourceGroup TestGroup4 = new ResourceGroup(2,3,4);
    ResourceGroup TestGroup5 = new ResourceGroup(100, 100, 100);

    [Test]
    public void CreationTest()
    {
        Assert.AreEqual(TestGroup2.food, 10);
        Assert.AreEqual(TestGroup2.energy, 10);
        Assert.AreEqual(TestGroup2.ore, 10);
    }

    [Test]
    public void EqualityTest()
    {
        Assert.AreEqual(new ResourceGroup(25, 25, 25), TestGroup3);
    }

    [Test]
    public void AdditionTest()
    {
        Assert.AreEqual(new ResourceGroup(35, 35, 35), TestGroup2 + TestGroup3);
    }

    [Test]
    public void SubtractionTest()
    {
        Assert.AreEqual(new ResourceGroup(3,2,1), TestGroup1 - TestGroup4);
    }

    [Test]
    public void ScalarMultiplicationTest()
    {
        Assert.AreEqual(new ResourceGroup(200,200,200), TestGroup5 * 2);
    }

    [Test]
    public void ScalarMultiplicationCommutativityTest()
    {
        Assert.AreEqual(new ResourceGroup(200, 200, 200), 2 * TestGroup5);
    }

    [Test]
    public void ResourceGroupMultiplicationTest()
    {
        Assert.AreEqual(new ResourceGroup(250,250,250), TestGroup2 * TestGroup3);
    }
}
