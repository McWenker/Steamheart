using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class InitBattleState : BattleState
{
	public override void Enter()
	{
		base.Enter ();
		StartCoroutine(Init());
	}

	IEnumerator Init()
	{
		board.Load (levelData);
		Point p = new Point ((int)levelData.positions[0].x, (int)levelData.positions[0].z);
		SelectTile(p);
		SpawnTestUnits(); // temporary for DEMO
        AddVictoryCondition();
        owner.round = owner.gameObject.AddComponent<TurnOrderController>().Round();
        yield return null;
		owner.ChangeState<CutSceneState>();
	}

	void SpawnTestUnits() // DEMO METHOD
    {
        // don't spawn on top of obstacles
        List<Tile> baseLocations = new List<Tile>(board.tiles.Values);
        List<Tile> locations = new List<Tile>();
        foreach(Tile t in baseLocations)
        {
            if (t.content == null)
                locations.Add(t);
        }

        // spawn AI units
        for (int j = 0; j < owner.enemies.Length; j++)
        {
            GameObject instance = UnitFactory.CreateUnit(owner.enemies[j]);
            int random = UnityEngine.Random.Range(locations.Count * 3 / 4, locations.Count);
            Tile randomTile = locations[random];
            locations.RemoveAt(random);
            Unit unit = instance.GetComponent<Unit>();
            unit.Place(randomTile);
            unit.dir = (Directions)UnityEngine.Random.Range(0, 4);
            unit.Match();
            units.Add(unit);
        }

        
        PartyEntry[] party = GameObject.FindObjectsOfType<PartyEntry>();
        // spawn player units
        for (int i = 0; i < party.Length; ++i)
        {
            GameObject instance = UnitFactory.CreateUnit(party[i].UnitInfo[0], party[i].UnitInfo[1], party[i].UnitInfo[2], party[i].PerkDict, party[i].UnitInfo[3], "Hero");
            int random = UnityEngine.Random.Range(0, locations.Count/3);
            Tile randomTile = locations[random];
            locations.RemoveAt(random);
            Unit unit = instance.GetComponent<Unit>();
            unit.Place(randomTile);
            unit.dir = (Directions)UnityEngine.Random.Range(0, 4);
            unit.Match();
            units.Add(unit);
            Destroy(party[i].gameObject);
        }
        SelectTile(units[0].tile.pos);
    }

    void AddVictoryCondition()
    {
        DefeatTargetVictoryCondition vc = owner.gameObject.AddComponent<DefeatTargetVictoryCondition>();
        Unit enemy = units[units.Count - 1];
        vc.target = enemy;
        Health health = enemy.GetComponent<Health>();
        health.MinHP = 10;
    }
}
