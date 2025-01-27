using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuMech : MonoBehaviour
{
    public Material LightsMat;
    public Animator MechAnim;
    public AudioClip LoadUpSound;

    public void Start()
    {
        LightsMat.SetFloat("_Emmssive_Strength", 0);
    }

    public void LoadUpMech()
    {
        MechAnim.SetTrigger("LoadUp");
        StartCoroutine(FadeLights(true));
        AudioManager.instance.PlaySFXFromClip(LoadUpSound);
    }

    public void LoadDownMech()
    {
        MechAnim.SetTrigger("LoadDown");
        StartCoroutine(FadeLights(false));
    }

    public IEnumerator FadeLights(bool on)
    {

        if (on)
        {
            float alpha = 0;
            while (alpha < 2)
            {
                alpha += Time.deltaTime;
                LightsMat.SetFloat("_Emmssive_Strength", alpha);
                yield return null;
            }
        }
        else
        {
            float alpha = 2;
            while (alpha > 0)
            {
                alpha -= Time.deltaTime;
                LightsMat.SetFloat("_Emmssive_Strength", alpha);
                yield return null;
            }
        }

        SceneLoader.instance.LoadScene(2);
    }
}
