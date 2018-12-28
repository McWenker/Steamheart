using System.IO;
using System.Collections;
using UnityEngine;
using System;
using System.Collections.Generic;

public static class UnitFactory
{
    public static GameObject Create(string name, int level)
    {
        UnitRecipe recipe = Resources.Load<UnitRecipe>("Unit Recipes/" + name);
        if (recipe == null)
        {
            Debug.LogError("No Unit Recipe for name: " + name);
            return null;
        }
        return CreateUnit(recipe);
    }

    public static GameObject CreateUnit(UnitRecipe recipe)
    {
        GameObject obj = InstantiatePrefab("Units/" + recipe.model);
        obj.name = recipe.name.TrimStart(new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '_' });
        obj.AddComponent<Unit>();
        AddStats(obj);
        AddRace(obj, recipe.race);
        AddLocomotion(obj, recipe.locomotion);
        obj.AddComponent<Status>();
        AddEquipment(obj, recipe.equipCatalog);
        obj.AddComponent<Health>();
        obj.AddComponent<Mana>();
        AddAttack(obj, recipe.attack);
        AddAbilityCatalog(obj);
        AddPerkCatalog(obj, recipe.perkCatalog);
        AddRank(obj, obj.GetComponentsInChildren<Perk>().Length);
        AddAlliance(obj, recipe.alliance);
        AddAttackPattern(obj, recipe.strategy);
        return obj;
    }

    public static GameObject CreateUnit(string name, string race, string cardinal, DictPerkBoolDict perkDict, string level, string model)
    {
        GameObject obj = InstantiatePrefab("Units/" + model);
        obj.name = name.TrimStart(new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '_' });
        obj.AddComponent<Unit>();
        AddStats(obj);
        AddRace(obj, race);
        AddLocomotion(obj, Locomotions.Walk);
        obj.AddComponent<Status>();
        AddEquipment(obj, cardinal);
        AddRank(obj, Int32.Parse(level));
        obj.AddComponent<Health>();
        obj.AddComponent<Mana>();
        AddAttack(obj, "");
        AddAbilityCatalog(obj);
        AddPerkCatalog(obj, perkDict);
        AddAlliance(obj, Alliances.Hero);
        AddAttackPattern(obj, "");
        return obj;
    }

    public static GameObject InstantiatePrefab(string name)
    {
        GameObject prefab = Resources.Load<GameObject>(name);
        if (prefab == null)
        {
            Debug.LogError("No Prefab for name: " + name);
            return new GameObject(name);
        }
        GameObject instance = GameObject.Instantiate(prefab);
        instance.name = instance.name.Replace("(Clone)", "");
        return instance;
    }

    static void AddStats(GameObject obj)
    {
        Stats s = obj.AddComponent<Stats>();
        s.SetValue(StatTypes.LVL, 1, false);
    }

    public static void AddRace(GameObject obj, string name)
    {
        string raceName = string.Format("Races/" + name);
        GameObject thisRace = InstantiatePrefab(raceName);
        if (thisRace == null)
        {
            Debug.LogError("No Race Found: " + name);
            return;
        }
        thisRace.transform.SetParent(obj.transform);
        thisRace.GetComponent<Race>().Employ();
        thisRace.GetComponent<Race>().LoadDefaultStats();
    }

    public static void AddEquipment(GameObject obj, string name)
    {
        Equipment thisEquip = obj.AddComponent<Equipment>();
        GameObject main = new GameObject("Equipment Catalog");
        main.transform.SetParent(obj.transform);

        EquipmentRecipe recipe = Resources.Load<EquipmentRecipe>("Equipment Recipes/" + name);
        if (recipe == null)
        {
            Debug.LogError("No Equipment Recipe Found: " + name);
            return;
        }

        for (int i = 0; i < recipe.slots.Length; ++i)
        {
            string itemName = string.Format("Items/{0}/{1}", recipe.slots[i].category, recipe.slots[i].name);
            GameObject item = InstantiatePrefab(itemName);
            item.name = item.name.Replace("(Clone)", "").Trim();
            if (item == null)
            {
                Debug.LogError("No Item Found: " + itemName);
                return;
            }

            if(item.GetComponent<Equippable>() == null)
            {
                Debug.LogError("No Equippable on item: " + itemName);
                GameObject.Destroy(item);
                return;
            }

            thisEquip.Equip(item.GetComponent<Equippable>(), recipe.slots[i].slotType, main.transform);
        }
    }

    public static void AddPerk(GameObject obj, string cardinal, string name)
    {
        string perkFile = string.Format("Perks/{0}/{1}", cardinal, name);
        GameObject perk = InstantiatePrefab(perkFile);
        if (perk == null)
        {
            Debug.LogError("No Perk Found: " + perkFile);
            return;
        }
        perk.name = name;
        perk.transform.SetParent(obj.transform.Find("Perk Catalog"));
        Perk thisPerk = perk.GetComponent<Perk>();
        thisPerk.Employ();
        thisPerk.LoadDefaultStats();
    }

    public static void AddPerk(GameObject obj, string cardinal, string name, GameObject parent)
    {
        string perkFile = string.Format("Perks/{0}/{1}", cardinal, name);
        GameObject perk = InstantiatePrefab(perkFile);
        if (perk == null)
        {
            Debug.LogError("No Perk Found: " + perkFile);
            return;
        }
        perk.name = name;
        perk.transform.SetParent(parent.transform);
        Perk thisPerk = perk.GetComponent<Perk>();
        thisPerk.Employ();
        thisPerk.LoadDefaultStats();
    }

    static void AddPerkCatalog(GameObject obj, DictPerkBoolDict dict)
    {
        GameObject main = new GameObject("Perk Catalog");
        main.transform.SetParent(obj.transform);
        main.AddComponent<PerkCatalog>();

        foreach (KeyValuePair<DictPerk, bool> entry in dict)
        {
            if (entry.Value == true)
            {
                AddPerk(obj, entry.Key.CardinalName, entry.Key.PerkName, main);
            }
        }
    }

    static void AddPerkCatalog(GameObject obj, string name)
    {
        GameObject main = new GameObject("Perk Catalog");
        main.transform.SetParent(obj.transform);
        main.AddComponent<PerkCatalog>();

        PerkCatalogRecipe recipe = Resources.Load<PerkCatalogRecipe>("Perk Catalog Recipes/" + name);
        if (recipe == null)
        {
            Debug.LogError("No Perk Catalog Recipe Found: " + name);
            return;
        }

        for (int i = 0; i < recipe.perks.Length; ++i)
        {
            AddPerk(obj, recipe.perks[i].perkCardinal.ToString(), recipe.perks[i].name);
        }
    }

    static void AddAbilityCatalog(GameObject obj)
    {
        GameObject main = new GameObject("Ability Catalog");
        main.transform.SetParent(obj.transform);
        main.AddComponent<AbilityCatalog>();
    }

    static void AddAbilityCatalog(GameObject obj, string name)
    {
        GameObject main = new GameObject("Ability Catalog");
        main.transform.SetParent(obj.transform);
        main.AddComponent<AbilityCatalog>();
        AbilityCatalogRecipe recipe = Resources.Load<AbilityCatalogRecipe>("Ability Catalog Recipes/" + name);
        if (recipe == null)
        {
            Debug.LogError("No Ability Catalog Recipe Found: " + name);
            return;
        }
        for (int i = 0; i < recipe.categories.Length; ++i)
        {
            GameObject category = new GameObject(recipe.categories[i].name);
            category.transform.SetParent(main.transform);
            for (int j = 0; j < recipe.categories[i].entries.Length; ++j)
            {
                string abilityName = string.Format("Abilities/{0}/{1}/{2}/{3}", recipe.categories[i].perkTier, recipe.categories[i].perk, recipe.categories[i].name, recipe.categories[i].entries[j]);
                GameObject ability = InstantiatePrefab(abilityName);
                if (ability == null)
                {
                    Debug.LogError("No Ability Found: " + abilityName);
                    return;
                }
                ability.name = recipe.categories[i].entries[j];
                ability.transform.SetParent(category.transform);
            }
        }
    }

    static void AddLocomotion(GameObject obj, Locomotions type)
    {
        switch (type)
        {
            case Locomotions.Walk:
                obj.AddComponent<WalkMovement>();
                break;
            case Locomotions.Fly:
                obj.AddComponent<FlyMovement>();
                break;
            case Locomotions.Teleport:
                obj.AddComponent<TeleMovement>();
                break;
        }
    }

    static void AddAlliance(GameObject obj, Alliances type)
    {
        Alliance alliance = obj.AddComponent<Alliance>();
        alliance.type = type;
    }

    static void AddRank(GameObject obj, int level)
    {
        Rank rank = obj.AddComponent<Rank>();
        rank.Init(level);
    }

    static void AddAttack(GameObject obj, string name)
    {
        GameObject instance = InstantiatePrefab("Abilities/Common/Attack");
        instance.name = "Unarmed Attack";
        //GameObject instance = InstantiatePrefab("Abilities/" + name);
        instance.transform.SetParent(obj.transform);
    }

    static void AddAttackPattern(GameObject obj, string name)
    {
        Driver driver = obj.AddComponent<Driver>();
        if(string.IsNullOrEmpty(name))
        {
            driver.normal = Drivers.Human;
        }
        else
        {
            driver.normal = Drivers.Computer;
            GameObject instance = InstantiatePrefab("Attack Patterns/" + name);
            instance.name = name + " Attack Pattern";
            instance.transform.SetParent(obj.transform);
        }
    }
}
