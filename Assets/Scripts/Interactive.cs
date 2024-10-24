using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Interactive : MonoBehaviour
{
    [SerializeField] protected GameObject visual;

    protected bool active = true;

    public virtual void OnProjectileCollision(Projectile projectile)
    {

    }

    public virtual void OnPlayerCollision(PlayerController playerController)
    {

    }

    public GameObject GetVisual() 
    { 
        return visual; 
    }

    protected void StartDestroying()
    {
        active = false;
        if (visual.TryGetComponent<FadeOut>(out FadeOut fadeOut))
        {
            fadeOut.StartTimeFadeOut();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    protected void Show()
    {
        if (visual != null) visual.SetActive(true);
    }

    protected void Hide()
    {
        if (visual != null) visual.SetActive(false);
    }
}
