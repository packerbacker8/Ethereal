using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class SprayPatternTool : EditorWindow
{
    private static List<float> desiredXPattern;
    private static List<float> desiredYPattern;
    private int numBullets = 0;
    [MenuItem("Ethereal Tools/Spray Pattern Creator")]
    public static void ShowWindow()
    {
        GetWindow<SprayPatternTool>("Spray Pattern Creator");
        desiredXPattern = new List<float>();
        desiredYPattern = new List<float>();
    }

    private void OnGUI()
    {
        numBullets = EditorGUILayout.IntField("Number of bullets to spray", numBullets);
        if(desiredXPattern.Count == 0 && desiredYPattern.Count == 0)
        {
            if (GUILayout.Button("Start Drawing Pattern"))
            {
                StartPatternDrawing(numBullets);
            }
        }
        else
        {
            string patternText = "The X pattern values:\n";
            int countShown = 0;
            foreach(float val in desiredXPattern)
            {
                patternText += val.ToString() + "f";
                countShown++;
                if(countShown == 5)
                {
                    patternText += "\n";
                    countShown = 0;
                }
                else
                {
                    patternText += ", ";
                }
            }
            patternText += "\nThe Y pattern values:\n";
            foreach (float val in desiredYPattern)
            {
                patternText += val.ToString() + "f";
                countShown++;
                if (countShown == 5)
                {
                    patternText += "\n";
                    countShown = 0;
                }
                else
                {
                    patternText += ", ";
                }
            }
            EditorGUILayout.TextArea(patternText);
        }
    }

    private void StartPatternDrawing(int num)
    {
        desiredXPattern = new List<float>(num);
        desiredYPattern = new List<float>(num);
        Debug.Log("Num sending: " + num);
        SprayPatternDrawing.ShowWindow(num);
    }

    public static void ReceiveAndDisplayPatterns(List<float> xPattern, List<float> yPattern)
    {
        desiredXPattern = xPattern;
        desiredYPattern = yPattern;
    }
}
