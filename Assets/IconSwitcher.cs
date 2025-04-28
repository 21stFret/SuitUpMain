using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IconSwitcher : MonoBehaviour
{
    private InputTracker inputTracker;
    public Image image;
    public Sprite pcIcon, gamepadIcon;

    void Start()
    {
        inputTracker = InputTracker.instance;
        inputTracker.OnInputChange += SwitchIcon;
        SwitchIcon();
    }

    public void SwitchIcon()
    {
        if (inputTracker.usingMouse)
        {
            image.sprite = pcIcon;
        }
        else
        {
            image.sprite = gamepadIcon;
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
