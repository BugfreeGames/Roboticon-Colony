//Game executable hosted at: http://www-users.york.ac.uk/~jwa509/executable.exe

using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class roboticonGuiElementScript : MonoBehaviour
{
    public Text roboticonNameObject;
    public GameObject installButton;
    public GameObject removeButton; // JBT
    public GameObject upgradeButton;
    public Roboticon roboticon;
    private roboticonWindowScript roboticonWindow;

    public void SetRoboticon(Roboticon roboticon)
    {
        this.roboticon = roboticon;
        roboticonNameObject.text = roboticon.GetName();
    }

    public void SetButtonEventListeners(roboticonWindowScript roboticonWindow)
    {
        this.roboticonWindow = roboticonWindow;
        installButton.GetComponent<Button>().onClick.AddListener(OnInstallClick);
        removeButton.GetComponent<Button>().onClick.AddListener(OnRemoveClick);
        upgradeButton.GetComponent<Button>().onClick.AddListener(OnUpgradeClick);
    }

    public void ShowInstallButton()
    {
        installButton.SetActive(true);
        upgradeButton.SetActive(false);
    }

    //Added by JBT to fix a bug where the install button would still be visiable when a roboticon was already installed
    /// <summary>
    /// Show Remove button 
    /// </summary>
    public void ShowRemoveButton()
    {
        removeButton.gameObject.SetActive(true);
    }

    public void ShowUpgradeButton()
    {
        installButton.SetActive(false);
        upgradeButton.SetActive(true);
    }

    public void HideButtons()
    {
        installButton.SetActive(false);
        upgradeButton.SetActive(false);
        removeButton.gameObject.SetActive(false);
    }

    public void OnInstallClick()
    {
        roboticonWindow.InstallRoboticon(roboticon);
    }

    public void OnRemoveClick()
    {
        roboticonWindow.UninstallRoboticon(roboticon);
    }

    public void OnUpgradeClick()
    {
        roboticonWindow.UpgradeRoboticon(roboticon);
    }
}
