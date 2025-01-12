using System;
using UnityEngine;

public static class AndroidTools
{
    public static bool CheckFilePermession()
    {
        AndroidJavaClass buildVersion = new AndroidJavaClass("android.os.Build$VERSION");
        int sdkInt = buildVersion.GetStatic<int>("SDK_INT");
        AndroidJavaClass environment = new AndroidJavaClass("android.os.Environment");
        bool isExternalStorageManager = environment.CallStatic<bool>("isExternalStorageManager");
        if (isExternalStorageManager)
        {
            Debug.Log("已获得所有权限");
            return true;
        }

        return false;
    }

    // open all file access settings dialogue
    public static void OpenAllFilesAccessSettings()
    {
        try
        {
            // Create a new intent with the action to manage all files access permission
            AndroidJavaObject intent = new AndroidJavaObject("android.content.Intent");
            intent.Call<AndroidJavaObject>("setAction", "android.settings.MANAGE_ALL_FILES_ACCESS_PERMISSION");

            // Get the current activity and start the intent
            AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            currentActivity.Call("startActivity", intent);
            
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to open all files access settings: " + e.Message);
        }
    }
}