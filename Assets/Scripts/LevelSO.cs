using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class LevelSO : ScriptableObject
{
    public int chunksNumber;
    public float forwardSpeedIncrease = 10;
    public GameObject wallPrefab;
    public List<Obstacle> obstaclePrefabs;
    public TimerAdder timerAdderPrefab;
    public List<Interactive> interactivePrefabs;
    public int obstaclesInChunkMin = 1;
    public int obstaclesInChunkMax = 3;
    public int timerAddersInChunkMin = 1;
    public int timerAddersInChunkMax = 1;
    public int interactiveInChunkMin = 1;
    public int interactiveInChunkMax = 1;
    public int ifLastJumpTo = 0;
}
