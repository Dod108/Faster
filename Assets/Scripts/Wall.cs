using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour
{
    [SerializeField] private float length = 10f;

    public float GetLength()
    {
        return length;
    }
}
