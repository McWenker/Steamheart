using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EquipmentRecipe", menuName = "Factory/EquipmentRecipe", order = 2)]
public class EquipmentRecipe : ScriptableObject
{
    [System.Serializable]
    public class Slot
    {
        public string name;
        public EquipSlots slotType;
        public string category;
    }

    public Slot[] slots;
}
