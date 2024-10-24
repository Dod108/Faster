using System.Globalization;
using TMPro;
using UnityEngine;

public class ObstacleDurability : MonoBehaviour
{
    [SerializeField] private Obstacle obstacle;
    [SerializeField] private TextMeshProUGUI durabilityText;

    private void Update()
    {
        float durability = obstacle.GetDurability();
        if (durability > 0 && obstacle.IsHighlighted)
        {
            if (!durabilityText.gameObject.activeInHierarchy)
            {
                durabilityText.gameObject.SetActive(true);
            }

            durabilityText.text = durability.ToString("0", CultureInfo.InvariantCulture);
        }
        else if (durabilityText.gameObject.activeInHierarchy)
        {
            durabilityText.gameObject.SetActive(false);
        }
    }
}
