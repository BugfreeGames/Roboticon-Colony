//Game executable hosted by JBT at: http://robins.tech/jbt/documents/assthree/GameExecutable.zip

using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Handles UI interaction for the Roboticon upgrades window
/// </summary>
public class roboticonUpgradesWindowScript : MonoBehaviour
{
    public canvasScript canvas;

    public Text foodUpgradesAmount;
    public Text energyUpgradesAmount;
    public Text oreUpgradesAmount;
    public Text upgradePrice;
    public Text installedText; // JBT

    private Roboticon roboticon;

    //Added by JBT to actually show whether a roboticon is installed or not in the roboticon upgrades window
    /// <summary>
    /// Shows the Roboticon upgrades window and populates the related information with the specified Roboticon.
    /// </summary>
    /// <param name="roboticon">The specified roboticon</param>
    public void Show(Roboticon roboticon)
    {
        ResourceGroup upgrades = roboticon.GetUpgrades();
        foodUpgradesAmount.text = upgrades.food.ToString();
        energyUpgradesAmount.text = upgrades.energy.ToString();
        oreUpgradesAmount.text = upgrades.ore.ToString();
        upgradePrice.text = "£" + Roboticon.UPGRADEVALUE;
        this.roboticon = roboticon;
        installedText.text = roboticon.IsInstalledToTile() ? "Yes" : "No";
        this.gameObject.SetActive(true);
    }

    public void Hide()
    {
        this.gameObject.SetActive(false);
    }

    public void OnUpgradeFoodClick()
    {
        canvas.UpgradeRoboticon(roboticon, new ResourceGroup(1, 0, 0));
    }

    public void OnUpgradeEnergyClick()
    {
        canvas.UpgradeRoboticon(roboticon, new ResourceGroup(0, 1, 0));
    }

    public void OnUpgradeOreClick()
    {
        canvas.UpgradeRoboticon(roboticon, new ResourceGroup(0, 0, 1));
    }
}
