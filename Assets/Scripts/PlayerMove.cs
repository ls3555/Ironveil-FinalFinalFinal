using UnityEngine;
using System.Collections;

public abstract class PlayerMove : MonoBehaviour
{
    public float cooldown;
    public float manaCost;

    public abstract IEnumerator Execute();
}
