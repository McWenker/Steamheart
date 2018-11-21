using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PerkCatalog", menuName = "Factory/PerkCatalog", order = 3)]
public class PerkCatalogRecipe : ScriptableObject
{
    [System.Serializable]
    public class CatalogPerk
    {
        [SerializeField] public string name;
        [SerializeField] public Cardinals perkCardinal;
    }
    public CatalogPerk[] perks;
}
