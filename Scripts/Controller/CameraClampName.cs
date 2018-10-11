using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraClampName : MonoBehaviour
{
    public Text nameLabel;

    private void Update()
    {
        Vector3 namePos = Camera.main.WorldToScreenPoint(transform.position);
        nameLabel.transform.position = namePos;
    }
}
