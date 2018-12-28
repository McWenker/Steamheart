using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatorCardinalPerks : MonoBehaviour
{
    [SerializeField] Cardinals cardinal;
    [SerializeField] string[] perks;
    public Dictionary<string, string[]> cardinalPerksDict = new Dictionary<string, string[]>();

    private void Start()
    {
        GetComponentInParent<UnitCreatorMenu>().perkImportDict.Add(cardinal, perks);
    }
}
