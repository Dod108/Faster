using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractiveCollectedUI : MonoBehaviour
{
    [SerializeField] private ScreenFlashUI flash;

    private void Start()
    {
        PlayerController.Instance.ProjectileProgressChanged += PlayerController_ProjectileProgressChanged;
    }

    private void PlayerController_ProjectileProgressChanged(object sender, PlayerController.ProjectileProgressChangedEventArgs e)
    {
        flash.Flash();
    }
}
