using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class Board : MonoBehaviour
{
	#region Fields
	[SerializeField] GameObject tilePrefab;
	public Dictionary<Point, Tile> tiles = new Dictionary<Point, Tile>();

	Point[] dirs = new Point[4]
	{
		new Point(0,1),
		new Point(0,-1),
		new Point(1,0),
		new Point(-1,0)
	};

	public Point min { get { return _min; }}
	public Point max { get { return _max; }}
	Point _min;
	Point _max;

	Color selectedTileColor = new Color(0,0.3f,1,1);
	Color defaultTileColor = new Color(1,1,1,1);
	#endregion

	#region Public
	public void Load (LevelData data)
	{
		_min = new Point(int.MaxValue, int.MaxValue);
		_max = new Point(int.MinValue, int.MinValue);

		for (int i = 0; i < data.positions.Count; ++i)
		{
			GameObject instance = Instantiate(tilePrefab) as GameObject;
			Tile t = instance.GetComponent<Tile>();
			t.Load(data.positions[i], data.terrains[i], data.obstacles[i]);
			tiles.Add(t.pos, t);
            instance.transform.parent = transform;

            _min.x = Mathf.Min(_min.x, t.pos.x);
			_min.y = Mathf.Min(_min.y, t.pos.y);
			_max.x = Mathf.Max(_max.x, t.pos.x);
			_max.y = Mathf.Max(_max.y, t.pos.y);
		}
	}

	// return list of tiles starting from specified tile that return certain criteria
	// criteria is passed via func which takes a segment of potential path
	// and returns a bool whether or not to allow movement (is in range, is accessible, etc)
	public List<Tile> Search (Tile start, Func<Tile, Tile, bool> addTile, bool showStart)
	{
		List<Tile> retValue = new List<Tile>();
        if(showStart)
		    retValue.Add(start);

		ClearSearch();
		Queue<Tile> checkNext = new Queue<Tile>();
		Queue<Tile> checkNow = new Queue<Tile>();

		start.distance = 0;
		checkNow.Enqueue(start);

		while (checkNow.Count > 0)
		{
			Tile t = checkNow.Dequeue ();
			for (int i = 0; i < 4; ++i)
			{
				Tile next = GetTile(t.pos + dirs[i]);
				if (next == null || next.distance <= t.distance + 1)
					continue;
				if (addTile(t, next))
				{
					next.distance = t.distance + 1;
					next.prev = t;
					checkNext.Enqueue (next);
					retValue.Add (next);
				}
			}

			if (checkNow.Count == 0)
				SwapReference(ref checkNow, ref checkNext);
		}

		return retValue;
	}

	public Tile GetTile(Point p)
	{
		return tiles.ContainsKey (p) ? tiles[p] : null;
	}

	public void SelectTiles(List<Tile> tiles)
	{
		for (int i = tiles.Count - 1; i >= 0; --i)
			tiles[i].GetComponent<Renderer>().material.SetColor("_Color", selectedTileColor);
	}

	public void DeSelectTiles(List<Tile> tiles)
	{
		for (int i = tiles.Count - 1; i >= 0; --i)
			tiles[i].GetComponent<Renderer>().material.SetColor("_Color", defaultTileColor);
	}

	#endregion

	#region Private
	void ClearSearch()
	{
		foreach (Tile t in tiles.Values)
		{
			t.prev = null;
			t.distance = int.MaxValue;
		}
	}

	void SwapReference(ref Queue<Tile> a, ref Queue<Tile> b)
	{
		Queue<Tile> temp = a;
		a = b;
		b = temp;
	}
	#endregion
}
