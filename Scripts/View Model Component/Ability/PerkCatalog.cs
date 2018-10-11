using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Assumes that all direct children are categories
/// and that the direct children of categories
/// are abilities
/// </summary>
public class PerkCatalog : MonoBehaviour
{
    public int CategoryCount()
    {
        return transform.childCount;
    }

    public GameObject GetCategory(int index)
    {
        if (index < 0 || index >= transform.childCount)
            return null;
        return transform.GetChild(index).gameObject;
    }

    public int PerkCount(GameObject category)
    {
        return category != null ? category.transform.childCount : 0;
    }

    public Perk GetPerk(int categoryIndex, int perkIndex)
    {
        GameObject category = GetCategory(categoryIndex);
        if (category == null || perkIndex < 0 || perkIndex >= category.transform.childCount)
            return null;
        return category.transform.GetChild(perkIndex).GetComponent<Perk>();
    }
}
