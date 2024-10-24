using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProjectileNumberVisualUI : MonoBehaviour
{
    [SerializeField] private Image bar;

    public Image GetBar() 
    {
        return bar;
    }
}
