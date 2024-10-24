using UnityEngine;

public class ResetStaticDataManager : MonoBehaviour
{
    private void Awake()
    {
        Obstacle.ResetStaticData();
    }
}
