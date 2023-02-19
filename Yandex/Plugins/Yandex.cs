using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class Yandex : MonoBehaviour
{
    [DllImport("__Internal")]
    private static extern void ShowAdv();

    private static GameObject instance;

    public void Start()
    {
        if (instance == null)
        {
            instance = gameObject;
            DontDestroyOnLoad(instance);
        }
        else
        {
            Destroy(gameObject);
        }
        ShowAdvSDK();
    }

    public void ShowAdvSDK()
    {
        ShowAdv();
    }
}
