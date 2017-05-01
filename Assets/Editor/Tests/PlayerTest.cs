using UnityEngine;
using NUnit.Framework;

/// <summary>
/// JBT created all new unit tests, as Bugfree chose to create their own testing framework instead of using an existing one
/// </summary>
public class PlayerTest {

    [Test]
    public void CreateHumanTest()
    {
        Human testHuman = new Human(ResourceGroup.Empty, "Test", 500);
        Assert.IsTrue(testHuman.IsHuman());
    }

    [Test]
    public void CreateAITest()
    {
        AI testAi = new AI(ResourceGroup.Empty, "bot", 400);
        Assert.IsFalse(testAi.IsHuman());
    }

    [Test]
    public void AcquireTileTest()
    {
        Human testHuman = new Human(ResourceGroup.Empty, "Test", 500);
        Tile t = new Tile(ResourceGroup.Empty, new Vector2(0, 0), 1);

        testHuman.AcquireTile(t);
        Assert.AreEqual(testHuman, t.GetOwner());
        Assert.AreEqual(testHuman.GetOwnedTiles()[0], t);
    }

    [Test]
    public void AcquireSameTileTest()
    {
        Human testHuman = new Human(ResourceGroup.Empty, "Test", 500);
        Tile t = new Tile(ResourceGroup.Empty, new Vector2(0, 0), 1);

        testHuman.AcquireTile(t);
        Assert.Throws<System.InvalidOperationException>(() => testHuman.AcquireTile(t));
    }

    public void AcquireRoboticon()
    {
        Human testHuman = new Human(ResourceGroup.Empty, "Test", 500);
        Roboticon r = new Roboticon();

        testHuman.AcquireRoboticon(r);
        Assert.AreEqual(testHuman.GetRoboticons()[0], r);
    }

    [Test]
    public void InstallRoboticonTest()
    {
        Human testHuman = new Human(ResourceGroup.Empty, "Test", 500);
        Tile t = new Tile(ResourceGroup.Empty, new Vector2(0, 0), 1);
        Roboticon r = new Roboticon();

        testHuman.AcquireTile(t);
        testHuman.AcquireRoboticon(r);
        InstallRoboticon(r, t);

        Assert.AreEqual(testHuman.GetOwnedTiles()[0].GetInstalledRoboticon(), r);
    }

    [Test]
    public void ScoreTest()
    {
        Human testHuman = new Human(ResourceGroup.Empty, "Test", 500);
        Tile t = new Tile(new ResourceGroup(3, 4, 7), new Vector2(0, 0), 1);
        Roboticon r = new Roboticon(new ResourceGroup(1, 0, 2));

        testHuman.AcquireTile(t);
        testHuman.AcquireRoboticon(r);
        InstallRoboticon(r, t);

        Assert.AreEqual(testHuman.CalculateScore(), 1964);
    }

    //Added by JBT to simulate the installation of a roboticon without interaction with any untestable gameobjects
    public void InstallRoboticon(Roboticon roboticon, Tile tile)
    {
        tile.InstallRoboticonTest(roboticon);
        roboticon.InstallRoboticonToTile(tile);
    }
}
