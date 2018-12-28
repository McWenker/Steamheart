using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PlayerOptions
{
    static float volume = 1f;

    public static float Volume
    {
        get
        {
            return volume;
        }
        set
        {
            volume = Mathf.Clamp(value, 0, 1);
        }
    }
}
