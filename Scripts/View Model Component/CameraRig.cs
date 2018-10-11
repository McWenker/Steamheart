using UnityEngine;
using System.Collections;
using System;

public class CameraRig : MonoBehaviour
{
	public float speed = 3f;
	public Transform follow;
    public Transform heading;
    public Transform pitch;

    public Vector3[] zoomPoints;
    public bool _isZooming = false;
    public bool _isRotating = false;

    //Directions facing = Directions.North;
    int facingIndex = 0; // 0 is north, 1 is east, 2 is south, 3 is west
    int cameraIndex = 2;
    
    public int CameraIndex
    {
        get
        {
            return cameraIndex;
        }
        set
        {
            cameraIndex = Mathf.Clamp(value, 0, zoomPoints.Length-1);
            if(!_isZooming)
                StartCoroutine(ZoomCamera(cameraIndex));
        }
    }

    public int FacingIndex
    {
        get
        {
            return facingIndex;
        }
        set
        {
            if (!_isRotating)
            {
                facingIndex = value;
                StartCoroutine(RotateCamera(heading.rotation, GetHead()));
            }
        }
    }

	Transform _transform;

    void Awake()
	{
		_transform = transform;
    }

	void Update()
	{
		if (follow)
			_transform.position = Vector3.Lerp(_transform.position, follow.position, speed * Time.deltaTime);
	}

    Quaternion GetHead()
    {
        Debug.Log("GetHead().facingIndex: " + facingIndex);
        Vector3 camRot = heading.rotation.eulerAngles;
        if (facingIndex == 1) // camera is facing east
        {
            //facing = Directions.East;
            Debug.Log("camera is facing east");
            return Quaternion.Euler(new Vector3(camRot.x, 135f, camRot.z));
        }
        if (facingIndex == 2) // camera is facing south
        {
            //facing = Directions.South;
            Debug.Log("camera is facing south");
            return Quaternion.Euler(new Vector3(camRot.x, 225f, camRot.z));
        }
        if (facingIndex < 0 || facingIndex == 3) // camera was facing north, rotating west; or camera is facing west
        {
            facingIndex = 3;
            Debug.Log("camera facing north, rotating west; or camera is facing west");
            //facing = Directions.West;
            return Quaternion.Euler(new Vector3(camRot.x, 315f, camRot.z));
        }
        if (facingIndex > 3 || facingIndex == 0) // camera was facing west, rotating back to north; or camera is facing north
        {
            facingIndex = 0;
            Debug.Log("camera was facing west, rotating back to north; or camera is facing north");
            //facing = Directions.North;
            return Quaternion.Euler(new Vector3(camRot.x, 45f, camRot.z));
        }
        else return heading.rotation;
    }

    public IEnumerator ZoomCamera(int index)
    {
        _isZooming = true;
        float elapsedTime = 0f;
        Vector3 pitchRot = pitch.localRotation.eulerAngles;
        Vector3 newLift = new Vector3(0, zoomPoints[index].y, 0);
        Vector3 newPitch = new Vector3(zoomPoints[index].x, pitchRot.y, 0);
        float startSize = GetComponentInChildren<Camera>().orthographicSize;
        float newSize = zoomPoints[index].z;

        Tweener liftTweener = heading.MoveToLocal(newLift, 0.3f, EasingEquations.EaseInQuad);
        Tweener pitchTweener = pitch.RotateToLocal(newPitch, 0.3f, EasingEquations.EaseInQuad);

        while ((pitchTweener != null || liftTweener != null) && elapsedTime < 0.3f)
        {
            GetComponentInChildren<Camera>().orthographicSize = EasingEquations.EaseInQuad(startSize, newSize, elapsedTime / 0.3f);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        _isZooming = false;
    }

    public IEnumerator RotateCamera(Quaternion camRot, Quaternion newRot)
    {
        if (_isRotating)
            yield break;
        _isRotating = true;
        //Tweener tweener = heading.RotateToLocal(newRot.eulerAngles, 0.3f, EasingEquations.EaseInQuad);
        float counter = 0;
        while(counter < 0.3f)
        {
            counter += Time.deltaTime;
            heading.rotation = Quaternion.Lerp(camRot, newRot, counter / 0.3f);
            yield return null;
        }
        _isRotating = false;
        Debug.Log("Ending Index: " + facingIndex);
    }
}
