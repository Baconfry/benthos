using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Settings
{
    public static float TurnDelay { get; set; }
    public static bool SandboxMode { get; set; }
    public static bool AutopilotMode { get; set; }
    public static int TileSetIndex { get; set; }
    public static int MapIndex { get; set; }

    static Settings()
    {
        TurnDelay = 0.5f;
        SandboxMode = false;
        AutopilotMode = false;
        TileSetIndex = 0;
        MapIndex = 0;
    }

}
