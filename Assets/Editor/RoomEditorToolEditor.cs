using System;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(RoomEditorTool))]
public class RoomEditorToolEditor : Editor
{
    RoomEditorTool tool;
    TileTypes currentBrush;

    private void OnEnable()
    {
        tool = (RoomEditorTool)target;
    }

    public void OnSceneGUI()
    {

        HandleUtility.AddDefaultControl(
    GUIUtility.GetControlID(FocusType.Passive)
);
        //current input event 
        Event e = Event.current;

        // Ray from mouse into world
        Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);

        Plane plane = new Plane(Vector3.forward, Vector3.zero);

        DrawSelector();

        if (plane.Raycast(ray, out float dist) && tool.roomGenerator.Map!=null)
        {
            Vector3 world = ray.GetPoint(dist);

            int x = Mathf.FloorToInt(world.x);
            int y = Mathf.FloorToInt(world.y);

            // Draw hover cell

            Handles.color = Color.green;
            Handles.DrawWireCube( new Vector3(x + 0.5f, y + 0.5f, 0), Vector3.one);


            DrawOutlinedText(new Vector3(x + 1f, y + 0.5f, 0), $"({x}, {y})", Color.black, Color.white);
            if (x > 0 && y > 0 && x < tool.roomGenerator.MapWidth && y < tool.roomGenerator.MapHeight)
            DrawOutlinedText(new Vector3(x - 0.5f, y - 0.5f, 0), ((TileTypes)tool.roomGenerator.Map[x, y]).ToString(), Color.black, Color.white);



                // Paint on click
            if (e.type == EventType.MouseDrag && e.button == 0 && !e.alt) // avoid camera conflict
            {
                Undo.RecordObject(tool, "Paint Tile");

                if(e.button ==0) tool.PaintTile(x, y,currentBrush);

                EditorUtility.SetDirty(tool);

                e.Use();
            }
        }

        if (e.type == EventType.MouseMove)
        {
            HandleUtility.Repaint();
        }

    }

    void DrawSelector()
    {
        Handles.BeginGUI();

        GUILayout.BeginArea(
            new Rect(10, 10, 150, 200),
            GUI.skin.box
        );

        GUILayout.Label("Brush");

        foreach (TileTypes type in Enum.GetValues(typeof(TileTypes)))
        {
            if (GUILayout.Button(type.ToString()))
            {
                currentBrush = type;
            }
        }

        GUILayout.EndArea();

        Handles.EndGUI();
    }

    void DrawOutlinedText(Vector3 pos, string text, Color outlineColor, Color fillColor )
    {
        GUIStyle style = new GUIStyle(EditorStyles.boldLabel);
        style.normal.textColor = fillColor;

        GUIStyle outlineStyle = new GUIStyle(style);
        outlineStyle.normal.textColor = outlineColor;


        // Outline
        Handles.Label(pos + new Vector3(0.03f, 0.03f, 0), text, outlineStyle);
        Handles.Label(pos + new Vector3(-0.03f, 0.03f, 0), text, outlineStyle);
        Handles.Label(pos + new Vector3(0.03f, -0.03f, 0), text, outlineStyle);
        Handles.Label(pos + new Vector3(-0.03f, -0.03f, 0), text, outlineStyle);

        // Main text
        Handles.Label(pos, text, style);
    }
}