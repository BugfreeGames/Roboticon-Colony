using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mainMenuManager : MonoBehaviour
{
    public GameObject guiCanvas;

    public GameObject playGameButton;
    public GameObject quitButton;
    public GameObject creditsButton;

    public GameObject playBackButton;
    public GameObject quitBackButton;
    public GameObject creditsBackButton;

    private static int LEFT_MOUSE_BUTTON = 0;
    private static float ANIM_LENGTH = 2;

    private static string ANIM_LEFT_MONITOR = "LookAtLeftMonitor";
    private static string ANIM_RIGHT_MONITOR = "LookAtRightMonitor";
    private static string ANIM_IDLE = "BackToIdle";
    private static string ANIM_MAIN_MONITOR = "LookAtMainMonitor";

    private Animator cameraAnimator;

	// Use this for initialization
	void Start ()
    {
        cameraAnimator = Camera.main.GetComponent<Animator>();
    }
	
	// Update is called once per frame
	void Update ()
    {
		if(Input.GetMouseButtonUp(LEFT_MOUSE_BUTTON))
        {
            CheckMonitorButtonsClicked();
        }
	}

    private void CheckMonitorButtonsClicked()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if(Physics.Raycast(ray, out hit))
        {
            switch(hit.collider.tag)
            {
                case TagManager.playGame:
                    SwitchToMainMonitor();
                    break;

                case TagManager.credits:
                    SwitchToCreditsMonitor();
                    break;

                case TagManager.quit:
                    SwitchToQuitMonitor();
                    break;

                case TagManager.back:
                    GoBackToCameraIdle();
                    break;
            }
        }
    }

    private void SwitchToMainMonitor()
    {
        cameraAnimator.SetTrigger(ANIM_MAIN_MONITOR);
        playGameButton.SetActive(false);
        playBackButton.SetActive(true);
        StartCoroutine(DelayShowMainMonitor());
    }

    private void SwitchToCreditsMonitor()
    {
        cameraAnimator.SetTrigger(ANIM_RIGHT_MONITOR);
        creditsButton.SetActive(false);
        creditsBackButton.SetActive(true);
    }

    private void SwitchToQuitMonitor()
    {
        cameraAnimator.SetTrigger(ANIM_LEFT_MONITOR);
        quitButton.SetActive(false);
        quitBackButton.SetActive(true);
    }

    private void GoBackToCameraIdle()
    {
        cameraAnimator.SetTrigger(ANIM_IDLE);

        playGameButton.SetActive(true);
        creditsButton.SetActive(true);
        quitButton.SetActive(true);

        playBackButton.SetActive(false);
        quitBackButton.SetActive(false);
        creditsBackButton.SetActive(false);
        guiCanvas.SetActive(false);
    }

    private IEnumerator DelayShowMainMonitor()
    {
        yield return new WaitForSeconds(ANIM_LENGTH);
        guiCanvas.SetActive(true);
    }
}
