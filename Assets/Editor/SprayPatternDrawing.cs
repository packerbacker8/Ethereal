using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class SprayPatternDrawing : EditorWindow
{
    private static int numBulletsToDraw = 100000;
    private List<Vector2> pointsToDraw = new List<Vector2>();
    private List<float> sprayPatternX = new List<float>();
    private List<float> sprayPatternY = new List<float>();

    public static void ShowWindow(int numBullets)
    {
        GetWindow<SprayPatternDrawing>("Spray Pattern Creator");
        numBulletsToDraw = numBullets;
        Debug.Log("Recieved: " + numBullets);
    }

    private void OnGUI()
    {
        Event e = Event.current;
        int controlID = GUIUtility.GetControlID(FocusType.Passive);
        switch (e.GetTypeForControl(controlID))
        {
            case EventType.MouseDown:
                GUIUtility.hotControl = controlID;                
                float normalizedX = e.mousePosition.x / position.width;
                float normalizedY = e.mousePosition.y / position.height;
                pointsToDraw.Add(new Vector2(normalizedX, normalizedY));
                normalizedX = normalizedX * 2 - 1;
                normalizedY = (normalizedY * 2 - 1) * -1;
                Debug.Log("grid x pos: " + normalizedX);
                Debug.Log("grid y pos: " + normalizedY);
                sprayPatternX.Add(normalizedX);
                sprayPatternY.Add(normalizedY);
                numBulletsToDraw--;
                e.Use();
                if (numBulletsToDraw == 0)
                {
                    CloseWindow();
                }
                break;
            case EventType.MouseUp:
                GUIUtility.hotControl = 0;
                e.Use();
                break;
            case EventType.MouseDrag:
                GUIUtility.hotControl = controlID;
                e.Use();
                break;
        }
        AllDrawingStuff();
    }

    private void CloseWindow()
    {
        SprayPatternTool.ReceiveAndDisplayPatterns(sprayPatternX, sprayPatternY);
        this.Close();
    }

    private void AllDrawingStuff()
    {
        EditorGUI.DrawRect(new Rect(position.width / 2, 0, 1, position.height), Color.black);
        EditorGUI.DrawRect(new Rect(0, position.height / 2, position.width, 1), Color.black);
        //goes from 0 to max x from left to right
        //goes from 0 to max y from top to bottom
        foreach (Vector2 spot in pointsToDraw)
        {
            EditorGUI.DrawRect(new Rect(spot.x * position.width, spot.y * position.height, 5, 5), Color.red);
        }
    }
}
