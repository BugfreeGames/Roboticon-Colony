//Game executable hosted at: http://www-users.york.ac.uk/~jwa509/executable.exe

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Assessment 4:- Handles the new 3D menu and animations associated with it.
/// </summary>
public class mainMenuManager : MonoBehaviour
{
    public mainMenuScript mainMenuScript;

    public GameObject playGameButton;
    public GameObject quitButton;
    public GameObject creditsButton;

    public GameObject playBackButton;
    public GameObject quitBackButton;
    public GameObject creditsBackButton;

    public GameObject quitConfirmButton;
    public GameObject creditsImg;

    public GameObject shipGameObject;
    public GameObject fadeToBlackGameObject;

    public static float SHIP_ANIM_LENGTH = 7f;
    public static float FADE_TO_BLACK_AFTER_SECONDS = 4.7f;
    public static float CAMERA_ANIM_LENGTH = 2;

    private static int LEFT_MOUSE_BUTTON = 0;

    private static string ANIM_LEFT_MONITOR = "LookAtLeftMonitor";
    private static string ANIM_RIGHT_MONITOR = "LookAtRightMonitor";
    private static string ANIM_IDLE = "BackToIdle";
    private static string ANIM_MAIN_MONITOR = "LookAtMainMonitor";
    private static string ANIM_SHIP_FLY = "fly";

    private Animator cameraAnimator;

    private bool canCheckClick = true;

	// Use this for initialization
	void Start ()
    {
        cameraAnimator = Camera.main.GetComponent<Animator>();
    }
	
	// Update is called once per frame
	void Update ()
    {
		if(Input.GetMouseButtonUp(LEFT_MOUSE_BUTTON) && canCheckClick)
        {
            CheckMonitorButtonsClicked();
        }
	}

    public void PlayShipLandAnimation()
    {
        canCheckClick = false;

        GoBackToCameraIdle();
        StartCoroutine(DelayShipFlyAnimation());
    }

    private void CheckMonitorButtonsClicked()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if(Physics.Raycast(ray, out hit))
        {
            StartCoroutine(DelayNextClick());

            switch (hit.collider.tag)
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

                case TagManager.confirmQuit:
                    Application.Quit();
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
        creditsImg.SetActive(true);
    }

    private void SwitchToQuitMonitor()
    {
        cameraAnimator.SetTrigger(ANIM_LEFT_MONITOR);
        quitButton.SetActive(false);
        quitBackButton.SetActive(true);
        quitConfirmButton.SetActive(true);
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
        mainMenuScript.HideMenu();

        creditsImg.SetActive(false);
        quitConfirmButton.SetActive(false);
    }

    private IEnumerator DelayShowMainMonitor()
    {
        yield return new WaitForSeconds(CAMERA_ANIM_LENGTH);
        mainMenuScript.ShowMenu();
    }

    private IEnumerator DelayNextClick()
    {
        canCheckClick = false;
        yield return new WaitForSeconds(CAMERA_ANIM_LENGTH);
        canCheckClick = true;
    }

    private IEnumerator DelayShipFlyAnimation()
    {
        yield return new WaitForSeconds(CAMERA_ANIM_LENGTH);
        shipGameObject.GetComponent<Animator>().SetTrigger(ANIM_SHIP_FLY);
        StartCoroutine(DelayFadeToBlackStart());
    }

    private IEnumerator DelayFadeToBlackStart()
    {
        yield return new WaitForSeconds(FADE_TO_BLACK_AFTER_SECONDS);
        fadeToBlackGameObject.GetComponent<fadeToBlackScript>().BeginFadeToBlack();        
    }
}
