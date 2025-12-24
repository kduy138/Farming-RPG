using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Callbacks;
#endif

[CreateAssetMenu(fileName = "ItemDatabaseObject", menuName = "Database Object/Item")]
public class ItemDatabaseObject : ScriptableObject, ISerializationCallbackReceiver
{
    public ItemScriptableObject[] itemSO;

#if UNITY_EDITOR
    private static bool pendingFix = false;
#endif

    [ContextMenu("Update Item ID")]
    public void UpdateItemID()
    {
        if (itemSO == null || itemSO.Length == 0) return;

        HashSet<int> usedIds = new HashSet<int>();

        for (int i = 0; i < itemSO.Length; i++)
        {
            var item = itemSO[i];

            if (item == null) continue;

            item.data.ID = i;
            usedIds.Add(i);

#if UNITY_EDITOR
            EditorUtility.SetDirty(item);
#endif
        }
#if UNITY_EDITOR
        EditorUtility.SetDirty(this);
        AssetDatabase.SaveAssets();
#endif
    }

    public void OnAfterDeserialize() { }

    public void OnBeforeSerialize() { }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (!Application.isPlaying && !pendingFix)
        {
            pendingFix = true;
            EditorApplication.delayCall += () =>
            {
                if (this != null)
                {
                    UpdateItemID();
                    pendingFix = false;
                }
            };
        }
    }

    [DidReloadScripts]
    private static void OnScriptsReloaded()
    {
        string[] guids = AssetDatabase.FindAssets("t:ItemDatabaseObject");
        foreach (string guid in guids)
        {
            var path = AssetDatabase.GUIDToAssetPath(guid);
            var db = AssetDatabase.LoadAssetAtPath<ItemDatabaseObject>(path);
            if (db != null)
                db.UpdateItemID();
        }
    }
#endif
}
