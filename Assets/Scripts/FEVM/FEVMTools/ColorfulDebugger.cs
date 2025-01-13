using System;
using UnityEngine;

public class ColorfulDebugger : MonoBehaviour
{
    public static ColorfulDebugger Instance;
    public  Color Camera;
    public  Color Collision;
    public  Color Battle;
    public  Color Data;
    public  Color Error;
    public  Color File;
    public  Color GameObject;
    public  Color Info;
    public  Color Resource;
    public  Color Scene;
    public  Color Time;
    public  Color UI;
    public  Color Warning;

    private void Awake()
    {
        Instance = this;
    }

    public static void Debug(string str,Color color)
    {
        int r,g,b,a;
        if (color.r < 1 || color.b < 1 || color.g < 1 || color.a < 1)
        {
            r = Mathf.Clamp((int)(color.r * 255), 0, 255);
            g = Mathf.Clamp((int)(color.g * 255), 0, 255);
            b = Mathf.Clamp((int)(color.b * 255), 0, 255);
            a = Mathf.Clamp((int)(color.a * 255), 0, 255);
        }
        else if (color.r > 1 || color.b > 1 || color.g > 1 || color.a > 1)
        {
            r = Mathf.Clamp((int)(color.r), 0, 255);
            g = Mathf.Clamp((int)(color.g), 0, 255);
            b = Mathf.Clamp((int)(color.b), 0, 255);
            a = Mathf.Clamp((int)(color.a), 0, 255);
        }
        else
        {
            r = g = b = a = 255;
        }
        string hex = $"{r:X2}{g:X2}{b:X2}{a:X2}".ToLower();
        UnityEngine.Debug.Log("<color=#"+hex+">" + str + "</color>");
    }
}
