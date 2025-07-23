using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemoUI : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(this.gameObject);
        gameObject.SetActive(false);
        if (PlayerSavedData.instance == null)
        {
            return;
        }
        if(!PlayerSavedData.instance.demo)
        {
            gameObject.SetActive(false);
        }
        else
        {
            gameObject.SetActive(true);
        }
    }

}
