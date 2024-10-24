using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileAdder : Interactive
{
    private void Start()
    {
        PlayerController.Instance.ProjectileProgressChanged += PlayerController_ProjectileProgressChanged;

        if (PlayerController.Instance.GetProjectileNumber() >= PlayerController.Instance.GetMaxProjectileNumber())
        {
            Hide();
        }
    }

    private void PlayerController_ProjectileProgressChanged(object sender, PlayerController.ProjectileProgressChangedEventArgs e)
    {
        if (e.projectileNumber == PlayerController.Instance.GetMaxProjectileNumber())
        {
            Hide();
        }
        else
        {
            Show();
        }
    }

    public override void OnPlayerCollision(PlayerController playerController)
    {
        if (active)
        {
            playerController.AddProjectileNumberProgress();
            StartDestroying();
        }
    }
}
