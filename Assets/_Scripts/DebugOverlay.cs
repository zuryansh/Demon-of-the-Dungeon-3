using UnityEngine;
using System.Text;

public class DebugOverlay : MonoBehaviour
{
    public static StringBuilder Log = new StringBuilder();
    const int MaxLines = 25;

    public static void Print(string msg)
    {
        Log.Insert(0, msg + "\n");
        // trim to last N lines
        var lines = Log.ToString().Split('\n');
        if (lines.Length > MaxLines)
            Log = new StringBuilder(string.Join("\n", lines, 0, MaxLines));
    }

    void OnGUI()
    {
        GUI.Label(new Rect(10, 10, 600, 600), Log.ToString());
    }
}