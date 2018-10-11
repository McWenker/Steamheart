using System.IO;
using System.Collections;
using UnityEngine;
using System;

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
        return Create(recipe, level);
    }

    public static GameObject Create(UnitRecipe recipe, int level)
    {
        GameObject obj = InstantiatePrefab("Units/" + recipe.model);
        obj.name = recipe.name;
        obj.AddComponent<Unit>();
        AddStats(obj);
        AddRace(obj, recipe.race);
        AddLocomotion(obj, recipe.locomotion);
        obj.AddComponent<Status>();
        AddEquipment(obj, recipe.equipCatalog);
        AddRank(obj, level);
        obj.AddComponent<Health>();
        obj.AddComponent<Mana>();
        AddAttack(obj, recipe.attack);
        AddAbilityCatalog(obj);
        AddPerkCatalog(obj, recipe.perkCatalog);
        AddAlliance(obj, recipe.alliance);
        AddAttackPattern(obj, recipe.strategy);
        return obj;
    }

    static GameObject InstantiatePrefab(string name)
    {
        GameObject prefab = Resources.Load<GameObject>(name);
        if (prefab == null)
        {
            Debug.LogError("No Prefab for name: " + name);
            return new GameObject(name);
        }
        GameObject instance = GameObject.Instantiate(prefab);
        return instance;
    }

    static void AddStats(GameObject obj)
    {
        Stats s = obj.AddComponent<Stats>();
        s.SetValue(StatTypes.LVL, 1, false);
    }

    static void AddRace(GameObject obj, string name)
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

    static void AddEquipment(GameObject obj, string name)
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
            string perkName = string.Format("Perks/{0}/{1}", Enum.GetName(typeof(Cardinals), recipe.perks[i].perkCardinal), recipe.perks[i].name);
            GameObject perk = InstantiatePrefab(perkName);
            if (perk == null)
            {
                Debug.LogError("No Perk Found: " + perkName);
                return;
            }
            perk.name = recipe.perks[i].name;
            perk.transform.SetParent(main.transform);
            Perk thisPerk = perk.GetComponent<Perk>();
            thisPerk.Employ();
            thisPerk.LoadDefaultStats();
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
