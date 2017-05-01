using UnityEngine;
using NUnit.Framework;

/// <summary>
/// JBT created all new unit tests, as Bugfree chose to create their own testing framework instead of using an existing one
/// </summary>
public class TileTest {

    [Test]
    public void InitialiseTile()
    {
        Tile tile = new Tile(new ResourceGroup(10, 10, 10), new Vector2(3, 3), 7);

        Assert.AreEqual(new Vector2(1, 2), tile.GetTileObject().GetTilePosition());
    }

    [Test]
    public void InstallRoboticon()
    {
        Tile tile = new Tile(new ResourceGroup(10, 10, 10), new Vector2(3, 3), 7);
        Roboticon r = new Roboticon();
        tile.InstallRoboticonTest(r);
        Assert.Throws<System.InvalidOperationException>(() => tile.InstallRoboticon(r));
    }

    [Test]
    public void UninstallRoboticon()
    {
        Tile tile = new Tile(new ResourceGroup(10, 10, 10), new Vector2(3, 3), 7);
        Roboticon r = new Roboticon();
        tile.InstallRoboticonTest(r);
        tile.UninstallRoboticonTest();
        Assert.Throws<System.InvalidOperationException>(() => tile.UninstallRoboticon());
    }

    [Test]
    public void GetPriceTest()
    {
        Tile tile = new Tile(new ResourceGroup(10, 10, 10), new Vector2(3, 3), 7);
        int price = tile.GetPrice();

        Assert.AreEqual(300, tile.GetPrice());
    }

    [Test]
    public void GetTotalResourceGeneratedTest()
    {
        Tile tile = new Tile(new ResourceGroup(10, 10, 10), new Vector2(3, 3), 7);
        Roboticon r = new Roboticon(new ResourceGroup(2, 0, 1));
        tile.InstallRoboticonTest(r);

        ResourceGroup resources = tile.GetTotalResourcesGenerated();
        ResourceGroup actualResources = tile.GetBaseResourcesGenerated() + tile.GetInstalledRoboticon().GetUpgrades() * Tile.ROBOTICON_UPGRADE_WEIGHT;

        Assert.AreEqual(resources, actualResources);
    }
}
