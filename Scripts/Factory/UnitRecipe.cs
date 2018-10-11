using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UnitRecipe", menuName = "Factory/UnitRecipe", order = 1)]
public class UnitRecipe : ScriptableObject
{
    public string model;
    public string race;
    public string attack;
    public string strategy;
    public string perkCatalog;
    public string equipCatalog;
    public Locomotions locomotion;
    public Alliances alliance;
}
