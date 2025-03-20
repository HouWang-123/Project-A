using System;
using UnityEngine;
using YooAsset;

public class GameSpriteTool
{
    public static void LoadImageAsync(string uri, Action<Sprite> finishCallback)
    {
        if (YooAssets.CheckLocationValid(uri))
        {
            AssetHandle loadAssetAsync = YooAssets.LoadAssetAsync<Sprite>(uri);
            loadAssetAsync.Completed += handle =>
            {
                finishCallback.Invoke(handle.AssetObject as Sprite);
                loadAssetAsync.Release();
            };
        }
        else
        {
            Debug.LogWarning("图片 " + uri + " 没有找到");
        }
    }

    public static Sprite LoadImageSync(string uri)
    {
        Sprite MyReturn(Sprite sprite)
        {
            return sprite;
        }
        
        if (YooAssets.CheckLocationValid(uri))
        {
            AssetHandle loadAssetAsync = YooAssets.LoadAssetSync<Sprite>(uri);
            loadAssetAsync.Completed += handle =>
            {
                MyReturn(handle.AssetObject as Sprite);
            };
        }
        else
        {
            Debug.LogWarning("图片 " + uri + " 没有找到");
        }
        return null;
    }
}
