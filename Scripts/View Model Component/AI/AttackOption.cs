using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackOption
{
    class Mark
    {
        public Tile tile;
        public bool isMatch;

        public Mark(Tile tile, bool isMatch)
        {
            this.tile = tile;
            this.isMatch = isMatch;
        }
    }

    public Tile target;
    public Directions direction;
    public List<Tile> areaTargets = new List<Tile>();
    public bool isCasterMatch;
    public Tile bestMoveTile { get; private set; }
    public int bestAngleBasedScore { get; private set; }
    List<Mark> marks = new List<Mark>();
    List<Tile> moveTargets = new List<Tile>();

    public void AddMoveTarget(Tile tile)
    {
        if (!isCasterMatch && areaTargets.Contains(tile))
            return;
        moveTargets.Add(tile);
    }

    public void AddMark(Tile tile, bool isMatch)
    {
        marks.Add(new Mark(tile, isMatch));
    }

    public int GetScore(Unit caster, Ability ability)
    {
        GetBestMoveTarget(caster, ability);
        if (bestMoveTile == null)
            return 0;

        int score = 0;
        for(int i = 0; i < marks.Count; ++i)
        {
            if (marks[i].isMatch)
                score++;
            else
                score--;
        }

        if (isCasterMatch && areaTargets.Contains(bestMoveTile))
            score++;

        return score;
    }

    void GetBestMoveTarget(Unit caster, Ability ability)
    {
        if (moveTargets.Count == 0)
            return;

        if(IsAbilityAngleBased(ability))
        {
            bestAngleBasedScore = int.MinValue;
            Tile startTile = caster.tile;
            Directions startDirection = caster.dir;
            caster.dir = direction;

            List<Tile> bestOptions = new List<Tile>();
            for(int i = 0; i < moveTargets.Count; ++i)
            {
                caster.Place(moveTargets[i]);
                int score = GetAngleBasedScore(caster);
                if(score > bestAngleBasedScore)
                {
                    bestAngleBasedScore = score;
                    bestOptions.Clear();
                }

                if(score == bestAngleBasedScore)
                {
                    bestOptions.Add(moveTargets[i]);
                }
            }

            caster.Place(startTile);
            caster.dir = startDirection;

            FilterBestMove(bestOptions);
            bestMoveTile = bestOptions[UnityEngine.Random.Range(0, bestOptions.Count)];
        }
        else
        {
            bestMoveTile = moveTargets[UnityEngine.Random.Range(0, moveTargets.Count)];
        }
    }

    bool IsAbilityAngleBased(Ability ability)
    {
        bool isAngleBased = false;
        for(int i = 0; i < ability.transform.childCount; ++i)
        {
            HitRate hr = ability.transform.GetChild(i).GetComponent<HitRate>();
            if(hr.IsAngleBased)
            {
                isAngleBased = true;
                break;
            }
        }
        return isAngleBased;
    }

    int GetAngleBasedScore(Unit caster)
    {
        int score = 0;
        for(int i = 0; i < marks.Count; ++i)
        {
            int value = marks[i].isMatch ? 1 : -1;
            int multiplier = MultiplierForAngle(caster, marks[i].tile);
            score += value * multiplier;
        }
        return score;
    }

    int MultiplierForAngle(Unit caster, Tile tile)
    {
        if (tile.content == null)
            return 0;

        Unit defender = tile.content.GetComponentInChildren<Unit>();
        if (defender == null)
            return 0;

        Facings facing = caster.GetFacing(defender);
        if (facing == Facings.Back)
            return 90;
        if (facing == Facings.Side)
            return 75;
        return 50;
    }

    void FilterBestMove(List<Tile> list)
    {
        if (!isCasterMatch)
            return;

        bool canTargetSelf = false;
        for(int i = 0; i < list.Count; ++i)
        {
            if(areaTargets.Contains(list[i]))
            {
                canTargetSelf = true;
                break;
            }
        }
        
        if(canTargetSelf)
        {
            for(int i = list.Count - 1; i >= 0; --i)
            {
                if (!areaTargets.Contains(list[i]))
                    list.RemoveAt(i);
            }
        }
    }
}
