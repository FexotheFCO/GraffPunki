using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuFade : MonoBehaviour
{
    public Animator Fade;

    public void FadeMenu()
    {
        Fade.SetTrigger("FadeMenu");
    }
}
