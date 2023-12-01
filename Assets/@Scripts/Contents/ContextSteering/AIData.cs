using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIData : MonoBehaviour
{
    public List<InteractionObject> targets = null;
    public Collider2D[] obstacles = null;
    public InteractionObject currentTarget;

    public int GetTargetsCount() => targets == null ? 0 : targets.Count;
}
