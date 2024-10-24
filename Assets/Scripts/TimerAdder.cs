using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimerAdder : Interactive
{
    [SerializeField] private float time = 5f;

    public override void OnPlayerCollision(PlayerController playerController)
    {
        if (active)
        {
            playerController.AddTime(time);
            StartDestroying();
        }
    }
}
