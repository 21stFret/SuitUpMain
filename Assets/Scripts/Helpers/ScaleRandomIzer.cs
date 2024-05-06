using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

#if UNITY_EDITOR
public class ScaleRandomIzer : EditorWindow
{
    float minScale = 0.5f;
    float maxScale = 1.5f;

    [MenuItem("Tools/Scale Randomizer")]
    public static void ShowWindow()
    {
        GetWindow<ScaleRandomIzer>("Scale Randomizer");
    }

    private void OnGUI()
    {
        GUILayout.Label("Scale Randomizer", EditorStyles.boldLabel);
        GUILayout.BeginHorizontal();
        minScale = EditorGUILayout.FloatField("Min Scale", minScale);
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        maxScale = EditorGUILayout.FloatField("Max Scale", maxScale);
        GUILayout.EndHorizontal();
        if (GUILayout.Button("Randomize Scale"))
        {
            foreach (GameObject obj in Selection.gameObjects)
            {
                float randomScale = Random.Range(minScale, maxScale);
                obj.transform.localScale = new Vector3(randomScale, randomScale, randomScale);
            }
        }
    }
}
#endif
