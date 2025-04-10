namespace Editor.GameSettings
{
    using UnityEngine;
    using UnityEditor;
    using System.IO;
    using UnityEditor.SceneManagement;

    public class DoorLayerChecker : EditorWindow
    {
        private string folderPath = "Assets/ArtAssets/Prefabs/Map/Rooms";
        private int targetLayer = 11;

        [MenuItem("游戏工具/检查并修正门对象层级")]
        public static void ShowWindow()
        {
            GetWindow<DoorLayerChecker>("门对象层级检查器");
        }

        private void OnGUI()
        {
            GUILayout.Label("门对象层级检查并自动修正", EditorStyles.boldLabel);

            folderPath = EditorGUILayout.TextField("Prefab 文件夹路径", folderPath);
            targetLayer = EditorGUILayout.LayerField("目标 Layer", targetLayer);

            if (GUILayout.Button("检查并修正所有 Prefab"))
            {
                CheckAndFixDoorLayersInPrefabs();
            }
        }

        private void CheckAndFixDoorLayersInPrefabs()
        {
            string[] prefabGuids = AssetDatabase.FindAssets("t:Prefab", new[] { folderPath });

            foreach (string guid in prefabGuids)
            {
                string prefabPath = AssetDatabase.GUIDToAssetPath(guid);
                GameObject prefabRoot = PrefabUtility.LoadPrefabContents(prefabPath);

                if (prefabRoot == null)
                    continue;

                bool hasChanged = false;
                DoorMono[] doors = prefabRoot.GetComponentsInChildren<DoorMono>(true);

                foreach (DoorMono door in doors)
                {
                    if (door.gameObject.layer != targetLayer)
                    {
                        Debug.LogWarning($"修正层级: [{prefabPath}] 中 [{door.name}] 原层级: {LayerMask.LayerToName(door.gameObject.layer)} → {LayerMask.LayerToName(targetLayer)}");
                        door.gameObject.layer = targetLayer;
                        hasChanged = true;
                    }
                }

                if (hasChanged)
                {
                    PrefabUtility.SaveAsPrefabAsset(prefabRoot, prefabPath);
                    Debug.Log($"已保存修改后的 Prefab：{prefabPath}");
                }

                PrefabUtility.UnloadPrefabContents(prefabRoot);
            }

            Debug.Log("检查并修正完成！");
        }
    }
}
