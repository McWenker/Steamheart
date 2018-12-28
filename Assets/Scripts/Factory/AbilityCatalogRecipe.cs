using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AbilityCatalog", menuName = "Factory/AbilityCatalog", order = 2)]
public class AbilityCatalogRecipe : ScriptableObject
{
    [System.Serializable]
    public class Category
    {
        public string name;
        public string perk;
        public string perkTier;
        public string[] entries;
    }

    public Category[] categories;
}
