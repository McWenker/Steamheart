using UnityEngine;
using System.Collections;

public abstract class StatusEffect : MonoBehaviour
{
    public virtual Sprite Icon { get; set; }
    public virtual int Rank { get; set; }
}
