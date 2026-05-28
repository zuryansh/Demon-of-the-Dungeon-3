using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(RoomEditorTool))]
public class RoomEditorToolEditor : Editor
{
    RoomEditorTool tool;

    private void OnEnable()
    {
        tool = (RoomEditorTool)target;
    }

    public void OnSceneGUI()
    {
        //current input event 
        Event e = Event.current;

        // Ray from mouse into world
        Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);

        Plane plane = new Plane(Vector3.forward, Vector3.zero);

        if (plane.Raycast(ray, out float dist))
        {
            Vector3 world = ray.GetPoint(dist);

            int x = Mathf.FloorToInt(world.x);
            int y = Mathf.FloorToInt(world.y);

            // Draw hover cell
            Handles.color = Color.green;
            Handles.DrawWireCube( new Vector3(x + 0.5f, y + 0.5f, 0), Vector3.one);
            Handles.Label(new Vector3(x + 0.5f, y + 0.5f, 0), $"({x}, {y})");




            // Paint on click
            if (e.type == EventType.MouseDown && !e.alt) // avoid camera conflict
            {
                Undo.RecordObject(tool, "Paint Tile");

                if(e.button ==0) tool.PaintTile(x, y);
                else if(e.button ==1) tool.RemoveTile(x, y);

                EditorUtility.SetDirty(tool);

                e.Use();
            }
        }

        if (e.type == EventType.MouseMove)
        {
            HandleUtility.Repaint();
        }

    }


}