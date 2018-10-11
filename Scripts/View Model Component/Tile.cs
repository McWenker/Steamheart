using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public const float stepHeight = 0.25f;
    public Point pos;
    public float height;
    public TerrainType terrain;
    public GameObject content;

    [HideInInspector] public Tile prev;
    [HideInInspector] public int distance;

    public Vector3 center
    {
        get
        {
            return new Vector3(pos.x, (height * stepHeight) + .4f, pos.y);
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
        Match();
    }

    public void Load(Vector3 v, TerrainType t)
    {
        Load(new Point((int)v.x, (int)v.z), v.y, t);
    }
}
