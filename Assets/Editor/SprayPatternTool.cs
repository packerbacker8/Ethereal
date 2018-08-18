using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SprayPatternTool))]
[CanEditMultipleObjects]
public class SprayPatternTool : EditorWindow
{
    private List<float> desiredXPattern;
    private List<float> desiredYPattern;
    private int numBullets = 0;
    [MenuItem("Ethereal Tools/Spray Pattern Creator")]
    public static void ShowWindow()
    {
        GetWindow<SprayPatternTool>("Spray Pattern Creator");
    }

    private void OnGUI()
    {
        int.TryParse(EditorGUILayout.TextField("How many bullets for this spray pattern?"), out numBullets);
        if(GUILayout.Button("Start Drawing Pattern"))
        {
            StartPatternDrawing(numBullets);
        }
    }

    private void StartPatternDrawing(int num)
    {
        desiredXPattern = new List<float>(num);
        desiredYPattern = new List<float>(num);
    }
}
