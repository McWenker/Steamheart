﻿using System.Collections;
using UnityEngine;

public enum Alliances
{
    None = 0,
    Neutral = 1 << 0,
    Hero = 1 << 1,
    Enemy = 1 << 2,
    Ally = 1 << 3
}
