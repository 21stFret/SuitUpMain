using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AirDropTimer : MonoBehaviour
{
    private float timeElapsed;
    public float airDropTime;
    public Image cover;
    public bool activated;
    public TMP_Text airDropText;

    // Start is called before the first frame update
    void Start()
    {
        ActivateButton(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(activated)
        {
            return;
        }
        timeElapsed += Time.deltaTime;
        var percentage = (timeElapsed / airDropTime);

        cover.fillAmount = percentage;
        if (timeElapsed >= airDropTime)
        {
            ActivateButton(true);
        }
    }

    private void ActivateButton(bool value)
    {
        activated = value;
        airDropText.enabled = value;
    }

    public void ResetAirDrop()
    {
        timeElapsed = 0;
        ActivateButton(false);
    }
}
