using TMPro.EditorUtilities;
using UnityEditor;
using UnityEngine;

namespace TMPro_Extension.Editor
{
    [CustomEditor(typeof(TMP_SpriteAtlas))]
    public class TMP_SpriteAtlasEditor : TMP_SpriteAssetEditor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            serializedObject.Update();

            EditorGUILayout.PropertyField(serializedObject.FindProperty("atlas"));

            if (GUILayout.Button("Update Sprite Asset"))
            {
                TMP_SpriteAtlas spriteAtlas = (TMP_SpriteAtlas)target;
                spriteAtlas.UpdateSpriteAssetFromAtlas();
                EditorUtility.SetDirty(spriteAtlas);
                AssetDatabase.SaveAssetIfDirty(spriteAtlas);
            }

            // EditorGUILayout.Space();
            // EditorGUILayout.LabelField("Sprite Asset Settings", EditorStyles.boldLabel);

            // // Draw remaining TMP_SpriteAsset properties
            // SerializedProperty iterator = serializedObject.GetIterator();
            // bool enterChildren = true;
            // while (iterator.NextVisible(enterChildren))
            // {
            //     enterChildren = false;

            //     // Skip these properties as they are handled differently
            //     if (iterator.propertyPath == "m_Script" || iterator.propertyPath == "atlas")
            //         continue;

            //     EditorGUILayout.PropertyField(iterator, true);
            // }

            serializedObject.ApplyModifiedProperties();
        }
    }
}