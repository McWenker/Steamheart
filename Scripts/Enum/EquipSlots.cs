using UnityEngine;
using System.Collections;

[System.Flags]
public enum EquipSlots
{
	None = 0,
	Primary = 1 << 0, // usually a weapon
	Secondary = 1 << 1, // usually a shield, but could be dual-wield or 2h weapon
	Head = 1 << 2, // helmet, hat, etc
	Body = 1 << 3, // body armor, robe, etc
    Foot = 1 << 4, // boots, sandals, treads
	Accessory = 2 << 5 // ring, belt, etc
}
