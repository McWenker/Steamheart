using UnityEngine;
using System.Collections;

public static class DirectionsExtensions
{
	public static Directions GetDirection(this Tile t1, Tile t2)
	{
		if(t1.pos.y < t2.pos.y)
			return Directions.North;
		if(t1.pos.x < t2.pos.x)
			return Directions.East;
		if(t1.pos.y > t2.pos.y)
			return Directions.South;
		return Directions.West;
	}

    public static Directions GetDirection(this Tile t1, Point p2)
    {
        if (t1.pos.y < p2.y)
            return Directions.North;
        if (t1.pos.x < p2.x)
            return Directions.East;
        if (t1.pos.y > p2.y)
            return Directions.South;
        return Directions.West;
    }

    public static Directions GetDirection(this Point p1, Point p2)
    {
        if (p1.y < p2.y)
            return Directions.North;
        if (p1.x < p2.x)
            return Directions.East;
        if (p1.y > p2.y)
            return Directions.South;
        return Directions.West;
    }

    public static Directions GetDirection(this Point p)
	{
		if (p.y > 0)
			return Directions.North;
		if (p.x > 0)
			return Directions.East;
		if (p.y < 0)
			return Directions.South;
		return Directions.West;
	}

	public static Point GetNormal(this Directions dir)
	{
		switch(dir)
		{
		case Directions.North:
			return new Point(0,1);
		case Directions.East:
			return new Point(1,0);
		case Directions.South:
			return new Point (0,-1);
		default: //Directions.West:
			return new Point(-1,0);
		}
	}

	public static Vector3 ToEuler(this Directions d)
	{
		return new Vector3(0, (int)d * 90, 0);
	}
}
