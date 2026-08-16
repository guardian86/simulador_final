using UnityEditor;
using UnityEngine;

// Asegura que los tags requeridos existan (solo en Editor)
public static class EnsureTags
{
    private static readonly string[] RequiredTags = new[] { "tagPersonas", "meta", "salida_cc", "muros" };

    [InitializeOnLoadMethod]
    private static void Ensure()
    {
        try
        {
            var tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
            var tagsProp = tagManager.FindProperty("tags");
            foreach (var tag in RequiredTags)
            {
                if (!HasTag(tagsProp, tag))
                {
                    int len = tagsProp.arraySize;
                    tagsProp.InsertArrayElementAtIndex(len);
                    tagsProp.GetArrayElementAtIndex(len).stringValue = tag;
                }
            }
            tagManager.ApplyModifiedProperties();
        }
        catch
        {
            // Ignorar si no se puede modificar (por ejemplo en modo batch sin permisos)
        }
    }

    private static bool HasTag(SerializedProperty tagsProp, string tag)
    {
        for (int i = 0; i < tagsProp.arraySize; i++)
        {
            var t = tagsProp.GetArrayElementAtIndex(i);
            if (t != null && t.stringValue == tag) return true;
        }
        return false;
    }
}
