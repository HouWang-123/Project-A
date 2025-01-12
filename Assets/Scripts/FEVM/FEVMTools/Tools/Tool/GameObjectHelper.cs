using UnityEngine;

public class GameObjectHelper : MonoBehaviour
{
    public static GameObject InstantiatePrefab(GameObject prefab, Transform parent = null)
    {
        GameObject instance = Instantiate(prefab, parent);
        instance.transform.localScale = Vector3.one;
        instance.SetActive(true);
        return instance;
    }

    public static void DestoryGameObject(Object go)
    {
        Destroy(go);
    }
}