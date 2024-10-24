using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class ProjectileNumberUI : MonoBehaviour
{
    [SerializeField] private ProjectileNumberVisualUI progressVisualTemplate;

    private Image lastImage;
    private List<ProjectileNumberVisualUI> progressVisuals;

    private void Start()
    {
        lastImage = progressVisualTemplate.GetBar();
        lastImage.fillAmount = 1f;

        progressVisuals = new List<ProjectileNumberVisualUI>();
        progressVisuals.Add(progressVisualTemplate);

        PlayerController.Instance.ProjectileProgressChanged += PlayerController_ProjectileProgressChanged;
    }

    private void PlayerController_ProjectileProgressChanged(object sender, PlayerController.ProjectileProgressChangedEventArgs e)
    {
        if (progressVisuals.Count < e.projectileNumber + Mathf.CeilToInt(e.progressNormalized))
        {
            for (int i = 0; i < e.projectileNumber + Mathf.CeilToInt(e.progressNormalized) - progressVisuals.Count; i++)
            {
                lastImage.fillAmount = 1f;
                GameObject newVisual = Instantiate(progressVisualTemplate.gameObject, transform);
                progressVisuals.Add(newVisual.GetComponent<ProjectileNumberVisualUI>());
                lastImage = newVisual.GetComponent<ProjectileNumberVisualUI>().GetBar();
            }
        }
        else if (progressVisuals.Count > e.projectileNumber + Mathf.CeilToInt(e.progressNormalized))
        {
            for (int i = progressVisuals.Count - 1; i >= e.projectileNumber + Mathf.CeilToInt(e.progressNormalized); i--)
            {
                Destroy(progressVisuals[i].gameObject);
                progressVisuals.RemoveAt(i);
            }

            lastImage = progressVisuals[progressVisuals.Count - 1].GetBar();
        }

        if (e.progressNormalized > 0f) lastImage.fillAmount = e.progressNormalized;
        else lastImage.fillAmount = 1f;
    }
}

