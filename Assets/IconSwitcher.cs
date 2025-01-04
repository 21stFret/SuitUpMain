using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IconSwitcher : MonoBehaviour
{
    public InputTracker inputTracker;
    public Image image;
    public Sprite spriteA, spriteB;

    void Start()
    {
        inputTracker = InputTracker.instance;
        inputTracker.OnInputChange += SwitchIcon;
    }

    public void SwitchIcon()
    {
        if (inputTracker.usingMouse)
        {
            image.sprite = spriteA;
        }
        else
        {
            image.sprite = spriteB;
        }
        if (image.sprite == null)
        {
            image.enabled = false;
        }
        else
        {
            image.enabled = true;
        }
    }
}
