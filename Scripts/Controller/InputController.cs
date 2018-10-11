using UnityEngine;
using System;
using System.Collections;

class Repeater
{
	const float threshold = 0.5f;
	const float rate = 0.25f;
    const float zoomRate = .5f;
	float _next;
	bool _hold;
	string _axis;
    bool _zoom;

	public Repeater(string axisName, bool isZoom)
	{
		_axis = axisName;
        _zoom = isZoom;
	}

	// Repeater does not inherit MonoBehaviour, Unity will not trigger this
	// must call it manually
	public int Update()
	{
		int retValue = 0;
		int value = Mathf.RoundToInt (Input.GetAxisRaw (_axis));

		if (value != 0)
		{
			if (Time.time > _next)
			{
				retValue = value;
                if(!_zoom)
				    _next = Time.time + (_hold ? rate : threshold);
                else
                    _next = Time.time + (_hold ? zoomRate : threshold);
                _hold = true;
			}
		}

		else 
		{
			_hold = false;
			_next = 0;
		}

		return retValue;
	}
}

public class InputController : MonoBehaviour
{
	// event handlers
	public static event EventHandler<InfoEventArgs<Point>> moveEvent;
	public static event EventHandler<InfoEventArgs<int>> fireEvent;
    public static event EventHandler<InfoEventArgs<string>> rotateEvent;
    public static event EventHandler<InfoEventArgs<int>> zoomEvent;
    public static event EventHandler<InfoEventArgs<string>> escapeEvent;
	
	// tracking if a movement button is being held
	Repeater _hor = new Repeater("Horizontal", false);
	Repeater _ver = new Repeater("Vertical", false);
    Repeater _zoom = new Repeater("Mouse ScrollWheel", true);
	
	// array to hold Fire buttons
	string[] _fireButtons = new string[] {"Fire1", "Fire2", "Fire3"};
	
	// Update is called once per frame
	void Update ()
	{
		int x = _hor.Update ();
		int y = _ver.Update ();
		if (x != 0 || y != 0)
		{
			moveEvent(this, new InfoEventArgs<Point>(new Point(x,y)));
		}

        int z = _zoom.Update();
        if (z != 0)
        {
            if (zoomEvent != null)
                zoomEvent(this, new InfoEventArgs<int>(z));
        }
		
		// after checking for movement, check for Fire
		for (int i = 0; i < 3; i++)
		{
			if (Input.GetButtonUp(_fireButtons[i]))
			{
				if (fireEvent != null)
					fireEvent(this, new InfoEventArgs<int>(i));
			}
		}

        // then, check rotation
        if (Input.GetKeyUp(KeyCode.Q))
        {
            if (rotateEvent != null)
                rotateEvent(this, new InfoEventArgs<string>("right"));
        }

        if (Input.GetKeyUp(KeyCode.E))
        {
            if (rotateEvent != null)
                rotateEvent(this, new InfoEventArgs<string>("left"));
        }

        if(Input.GetKeyUp(KeyCode.Escape))
        {
            if(escapeEvent != null)
            {
                escapeEvent(this, new InfoEventArgs<string>(""));
            }
        }

    }
}