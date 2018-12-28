using UnityEngine;
using System.Collections.Generic;
using UnityEditor;

[CreateAssetMenu(fileName = "Convo", menuName = "Convo", order = 1)]
public class ConvoData : ScriptableObject
{
    public List<SpeakerData> list;
}
