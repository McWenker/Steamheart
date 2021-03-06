﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public const float stepHeight = 0.25f;
    public Point pos;
    public float height;
    public TerrainType terrain;
    public GameObject content;
    public GameObject contentPrefab;

    [HideInInspector] public Tile prev;
    [HideInInspector] public int distance;

    bool loadFromFile = false;

    public Vector3 center
    {
        get
        {
            return new Vector3(pos.x, (height * stepHeight) + .4f, pos.y);
        }
    }

    void FetchObstacle()
    {
        int score = Random.Range(0, terrain.obstacles.Length * 5);
        if(score < terrain.obstacles.Length)
        {
            PlaceObstacle(terrain.obstacles[score]);
        }
    }

    void PlaceObstacle(GameObject obstacle)
    {
        contentPrefab = obstacle;
        if(contentPrefab != null)
        {
            content = Instantiate(contentPrefab);
            content.transform.parent = this.transform;
            content.transform.localPosition = new Vector3(0, content.transform.localPosition.y, 0);
            Quaternion rot = Quaternion.Euler(new Vector3(0, Random.Range(0, 360), 0));
            content.transform.rotation = rot;
        }
    }

    void Match()
    {
        transform.localPosition = new Vector3(pos.x, (height * stepHeight / 2f) + stepHeight, pos.y);
        transform.localScale = new Vector3(1, (height * stepHeight) + stepHeight / 2f, 1);
        gameObject.name = terrain.name;
        gameObject.GetComponent<MeshRenderer>().sharedMaterial = terrain.material;
    }

    public void Grow()
    {
        height++;
        Match();
    }

    public void Shrink()
    {
        height--;
        Match();
    }

    public void Load(Point p, float h, TerrainType t)
    {
        pos = p;
        height = h;
        terrain = t;
        FetchObstacle();
        Match();
    }

    public void Load(Vector3 v, TerrainType t, GameObject o)
    {
        pos = new Point((int)v.x, (int)v.z);
        height = v.y;
        terrain = t;
        PlaceObstacle(o);
        Match();
    }
}
