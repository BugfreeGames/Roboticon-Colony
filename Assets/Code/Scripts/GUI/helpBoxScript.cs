//Game executable hosted at: http://www-users.york.ac.uk/~jwa509/executable.exe

using UnityEngine;
using UnityEngine.UI;

// Edited by JBT to stop displaying the helpbox after a user has clicked the hide button
/// <summary>
/// Script used to show/hide the help box for each phase
/// </summary>
public class helpBoxScript : MonoBehaviour
{
    public Animator helpBoxAnimator;
    public Text helpBoxText;
    public bool hide = false;

    /// <summary>
    /// Show the Help box window and initialise with the specified help text
    /// </summary>
    /// <param name="text">The specifiede help text</param>
    public void ShowHelpBox(string text = "")
    {
        if (hide)
            return;

        helpBoxText.text = text;
        helpBoxAnimator.SetBool("helpBoxVisible", true);
    }

    /// <summary>
    /// Hides the HelpBox 
    /// </summary>
    public void HideHelpBox()
    {
        helpBoxAnimator.SetBool("helpBoxVisible", false);
        hide = true;
    }
}
