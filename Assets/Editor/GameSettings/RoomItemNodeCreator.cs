namespace Editor.GameSettings
{
    using UnityEngine;
    using UnityEditor;

    public class RoomItemNodeCreator : EditorWindow
    {
        private string folderPath = "Assets/ArtAssets/Prefabs/Map/Rooms";
        private string gameObjectName = "RoomItemNode";

        [MenuItem("游戏工具/布置所有场景中道具对象的位置")]
        public static void ShowWindow()
        {
            GetWindow<RoomItemNodeCreator>("布置所有场景中道具的位置");
        }

        private void OnGUI()
        {
            EditorGUILayout.TextArea("这个工具用于整理所有拖拽到场景中的物品，每次更新场景中的物品后都需要执行这个方法！");
            folderPath = EditorGUILayout.TextField("Prefab 文件夹路径", folderPath);
            gameObjectName = EditorGUILayout.TextField("存储道具的游戏对象名称", gameObjectName);

            if (GUILayout.Button("执行"))
            {
                CheckAndFixAllRoomItem();
            }
        }

        private void CheckAndFixAllRoomItem()
        {
            string[] prefabGuids = AssetDatabase.FindAssets("t:Prefab", new[] { folderPath });

            foreach (string guid in prefabGuids)
            {
                string prefabPath = AssetDatabase.GUIDToAssetPath(guid);
                GameObject prefabRoot = PrefabUtility.LoadPrefabContents(prefabPath);

                if (prefabRoot == null)
                    continue;

                bool hasChanged = false;

                // 查找或创建 RoomItemNode
                Transform itemNode = prefabRoot.transform.Find(gameObjectName);
                if (itemNode == null)
                {
                    GameObject itemNodeGO = new GameObject(gameObjectName);
                    itemNodeGO.transform.SetParent(prefabRoot.transform);
                    itemNodeGO.transform.localPosition = Vector3.zero;
                    itemNode = itemNodeGO.transform;
                    hasChanged = true;
                }

                // 获取 RoomMono 并绑定引用
                RoomMono roomMono = prefabRoot.GetComponent<RoomMono>();
                if (roomMono != null && roomMono.RoomItemNode != itemNode)
                {
                    roomMono.RoomItemNode = itemNode;
                    hasChanged = true;
                }

                // 查找所有 ItemBase，并将其移入 itemNode 下（如果不在其下）
                ItemBase[] items = prefabRoot.GetComponentsInChildren<ItemBase>(true);
                foreach (ItemBase item in items)
                {
                    if (item.transform.parent != itemNode)
                    {
                        item.transform.SetParent(itemNode);
                        hasChanged = true;
                    }
                }

                // 保存修改
                if (hasChanged)
                {
                    PrefabUtility.SaveAsPrefabAsset(prefabRoot, prefabPath);
                    Debug.Log($"✅ 已保存修改后的 Prefab：{prefabPath}");
                }

                PrefabUtility.UnloadPrefabContents(prefabRoot);
            }

            Debug.Log("✅ 所有场景道具整理完毕！");
        }
    }
}
