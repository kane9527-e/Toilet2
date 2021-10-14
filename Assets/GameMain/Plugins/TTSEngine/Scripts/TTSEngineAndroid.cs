using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public static class TTSEngineAndroid
{
    private static AndroidJavaObject javaObject;

    public static bool Inited
    {
        get
        {
            if (javaObject == null) return false;
            return javaObject.Get<bool>("inited");
        }
    }

    public static void Init()
    {
        var UnityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        var currentActivity = UnityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
        javaObject = new AndroidJavaObject("com.msc.tts.TTSEngine");
        javaObject.Call("Init", "958c974c", currentActivity);
    }

    public static void Speak(string text)
    {
        if (!Inited) return;
        javaObject.Call("Speak", text, "xiaofeng");
    }
}