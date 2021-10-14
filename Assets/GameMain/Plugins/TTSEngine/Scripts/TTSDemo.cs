using System.Collections;
using System.Collections.Generic;
using Narrative.Runtime.Scripts.MonoBehaviour;
using UnityEngine;

public class TTSDemo : MonoBehaviour
{
    public void Init()
    {
        TTSEngineAndroid.Init();
    }

    public void Speak()
    {
        TTSEngineAndroid.Speak("你好世界");
    }
}
