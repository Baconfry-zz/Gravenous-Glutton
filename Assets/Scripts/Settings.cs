using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Settings
{
    public static bool SaveEnabled { get; set; }

    static Settings()
    {
        SaveEnabled = true;
    }
}
