using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private float chunkSizeX = 14f;
    [SerializeField] private float chunkSizeY = 10f;
    [SerializeField] private float chunkSizeZ = 70f;
    [SerializeField] private int chunkNumberForward = 10;
    [SerializeField] private int chunkNumberBackward = 1;
    [SerializeField] private int chunkNumberDelete = 3;
    [SerializeField] private List<LevelSO> levels;

    public static GameManager Instance { get; private set; }
    public event EventHandler GameOver;
    public event EventHandler GamePaused;
    public event EventHandler GameUnpaused;
    public event EventHandler GameStarted;

    private const string SAVE_FILE_NAME = "save.txt";
    private static readonly string SAVE_FILE_PATH = Application.dataPath + "/" + SAVE_FILE_NAME;
    private List<int> chunks = new List<int>();
    private Dictionary<int, List<GameObject>> chunksObjects = new Dictionary<int, List<GameObject>>();
    private Dictionary<int, LevelSO> chunksLevels = new Dictionary<int, LevelSO>();
    private int currentGeneratedChunk = 0;
    private LevelSO generatedLevel;
    private int generatedLevelNumber = 0;
    private int levelChunksLeft;
    private bool isGamePaused = false;

    private enum State
    {
        GameInitialized,
        GamePlaying,
        GameOver,
    }

    private State state;

    private void Awake()
    {
        Instance = this;

        state = State.GameInitialized;
        NextGeneratedLevel();
    }

    private void Start()
    {
        GameInput.Instance.Pause += GameInput_Pause;

        Load();
    }

    private void Update()
    {
        if (IsGameInitialized())
        {
            GenerateLevel(Vector3.zero);
        }
    }

    private void GameInput_Pause(object sender, EventArgs e)
    {
        TogglePause();
    }

    public bool IsGameInitialized()
    { 
        return state == State.GameInitialized; 
    }

    public bool IsGamePlaying()
    { 
        return state == State.GamePlaying;
    }

    public bool IsGameOver()
    {
        return state == State.GameOver;
    }

    public void SetGameOver()
    {
        state = State.GameOver;
        Cursor.visible = true;
        Save();
        GameOver?.Invoke(this, EventArgs.Empty);
    }

    public void StartGamePlaying()
    {
        Cursor.visible = false;
        state = State.GamePlaying;
        GameStarted?.Invoke(this, EventArgs.Empty);
    }

    public void RestartGame()
    {
        Loader.Load(Loader.Scene.GameScene);
    }

    public void GenerateLevel(Vector3 playerPosition)
    {
        // Determine current chunk.
        int newCurrentChunk = GetCurrentChunk(playerPosition);

        // If chunk changed, generate level.
        if (newCurrentChunk != currentGeneratedChunk || IsGameInitialized())
        {
            currentGeneratedChunk = newCurrentChunk;

            // Check which chunks are generated and generate if necessary.
            for (int i = currentGeneratedChunk - chunkNumberBackward; i <= currentGeneratedChunk + chunkNumberForward; i++)
            {
                if (!chunks.Contains(i))
                {
                    // Create game object list.
                    List<GameObject> thisChunkObjects = new List<GameObject>();

                    // Generate walls.
                    float wallLength = generatedLevel.wallPrefab.GetComponent<Wall>().GetLength();
                    int wallsNumber = Mathf.FloorToInt(chunkSizeZ / wallLength);
                    for (int j = 0; j < wallsNumber; j++)
                    {
                        GameObject wall = Instantiate(generatedLevel.wallPrefab, new Vector3(0f, 0f, i * chunkSizeZ + j * wallLength), Quaternion.identity);
                        thisChunkObjects.Add(wall);
                    }

                    // Generate stuff if the chunk is after the 0 chunk.
                    if (i > 0)
                    {
                        // Generate obstacles.
                        if (generatedLevel.obstaclePrefabs.Count > 0)
                        {
                            int obstaclesNumber = UnityEngine.Random.Range(generatedLevel.obstaclesInChunkMin, generatedLevel.obstaclesInChunkMax + 1);
                            for (int j = 0; j < obstaclesNumber; j++)
                            {
                                int randomIndex = UnityEngine.Random.Range(0, generatedLevel.obstaclePrefabs.Count);
                                Obstacle obstaclePrefab = generatedLevel.obstaclePrefabs[randomIndex];
                                GameObject obstaclePrefabVisual = obstaclePrefab.GetVisual();
                                Bounds obstacleBounds = obstaclePrefabVisual.GetComponent<MeshFilter>().sharedMesh.bounds;
                                float offsetX = obstacleBounds.extents.x * obstaclePrefabVisual.transform.lossyScale.x;
                                float offsetY = obstacleBounds.extents.y * obstaclePrefabVisual.transform.lossyScale.y;

                                float randomX = UnityEngine.Random.Range(-chunkSizeX / 2 + offsetX, chunkSizeX / 2 - offsetX);
                                float randomY = UnityEngine.Random.Range(-chunkSizeY / 2 + offsetY, chunkSizeY / 2 - offsetY);
                                float randomZ = UnityEngine.Random.Range(((float) i + (float) j / (float) obstaclesNumber) * chunkSizeZ, ((float) i + ((float) j + 1) / (float) obstaclesNumber) * chunkSizeZ);

                                GameObject obstacle = Instantiate(obstaclePrefab.gameObject, new Vector3(randomX, randomY, randomZ), Quaternion.identity);
                                thisChunkObjects.Add(obstacle);
                            }
                        }

                        // Generate timer adders.
                        int timerAddersNumber = UnityEngine.Random.Range(generatedLevel.timerAddersInChunkMin, generatedLevel.timerAddersInChunkMax + 1);
                        for (int j = 0; j < timerAddersNumber; j++)
                        {
                            GameObject timerAdderPrefabVisual = generatedLevel.timerAdderPrefab.GetVisual();
                            Bounds timerAdderBounds = timerAdderPrefabVisual.GetComponent<MeshFilter>().sharedMesh.bounds;
                            float offsetX = timerAdderBounds.extents.x * timerAdderPrefabVisual.transform.lossyScale.x;
                            float offsetY = timerAdderBounds.extents.y * timerAdderPrefabVisual.transform.lossyScale.y;

                            float randomX = UnityEngine.Random.Range(-chunkSizeX / 2 + offsetX, chunkSizeX / 2 - offsetX);
                            float randomY = UnityEngine.Random.Range(-chunkSizeY / 2 + offsetY, chunkSizeY / 2 - offsetY);
                            float randomZ = UnityEngine.Random.Range(((float)i + (float)j / (float)timerAddersNumber) * chunkSizeZ, ((float)i + ((float)j + 1) / (float)timerAddersNumber) * chunkSizeZ);
                            GameObject timerAdder = Instantiate(generatedLevel.timerAdderPrefab.gameObject, new Vector3(randomX, randomY, randomZ), Quaternion.identity);
                            thisChunkObjects.Add(timerAdder);
                        }

                        // Generate other interactive objects.
                        if (generatedLevel.interactivePrefabs.Count > 0)
                        {
                            int interactiveNumber = UnityEngine.Random.Range(generatedLevel.interactiveInChunkMin, generatedLevel.interactiveInChunkMax + 1);
                            for (int j = 0; j < interactiveNumber; j++)
                            {
                                int randomIndex = UnityEngine.Random.Range(0, generatedLevel.interactivePrefabs.Count);
                                Interactive interactivePrefab = generatedLevel.interactivePrefabs[randomIndex];
                                GameObject interactivePrefabVisual = interactivePrefab.GetVisual();
                                Bounds interactiveBounds = interactivePrefabVisual.GetComponent<MeshFilter>().sharedMesh.bounds;
                                float offsetX = interactiveBounds.extents.x * interactivePrefabVisual.transform.lossyScale.x;
                                float offsetY = interactiveBounds.extents.y * interactivePrefabVisual.transform.lossyScale.y;

                                float randomX = UnityEngine.Random.Range(-chunkSizeX / 2 + offsetX, chunkSizeX / 2 - offsetX);
                                float randomY = UnityEngine.Random.Range(-chunkSizeY / 2 + offsetY, chunkSizeY / 2 - offsetY);
                                float randomZ = UnityEngine.Random.Range(i * chunkSizeZ, (i + 1) * chunkSizeZ);

                                GameObject interactive = Instantiate(interactivePrefab.gameObject, new Vector3(randomX, randomY, randomZ), Quaternion.identity);
                                thisChunkObjects.Add(interactive);
                            }
                        }
                    }

                    // Add chunk to the list.
                    chunks.Add(i);
                    chunksObjects.Add(i, thisChunkObjects);
                    chunksLevels.Add(i, generatedLevel);

                    // Decrease level chunk number and change level if necessary.
                    if (i >= currentGeneratedChunk) levelChunksLeft--;
                    if (levelChunksLeft <= 0)
                    {
                        NextGeneratedLevel();
                    }
                }
            }

            // Delete old chunks.
            if (chunks.Contains(currentGeneratedChunk - chunkNumberDelete))
            {
                // Destroy game objects.
                for (int i = 0; i < chunksObjects[currentGeneratedChunk - chunkNumberDelete].Count; i++)
                {
                    Destroy(chunksObjects[currentGeneratedChunk - chunkNumberDelete][i]);
                }

                // Remove chunk from the list.
                chunks.Remove(currentGeneratedChunk - chunkNumberDelete);
                chunksObjects.Remove(currentGeneratedChunk - chunkNumberDelete);
                chunksLevels.Remove(currentGeneratedChunk - chunkNumberDelete);
            }

            // Start game if it is not playing yet.
            if (IsGameInitialized())
            {
                StartGamePlaying();
            }
        }
    }

    public LevelSO GetCurrentLevel(Vector3 playerPosition)
    {
        return chunksLevels[GetCurrentChunk(playerPosition)];
    }

    public int GetCurrentChunk(Vector3 playerPosition)
    {
        return Mathf.FloorToInt(playerPosition.z / chunkSizeZ);
    }

    private void NextGeneratedLevel()
    {
        generatedLevel = levels[generatedLevelNumber];
        levelChunksLeft = generatedLevel.chunksNumber;

        generatedLevelNumber++;
        if (generatedLevelNumber >= levels.Count)
        {
            generatedLevelNumber = generatedLevel.ifLastJumpTo;
        }
    }

    public void TogglePause()
    {
        if (IsGamePlaying())
        {
            isGamePaused = !isGamePaused;

            if (isGamePaused)
            {
                Save();
                Time.timeScale = 0f;
                GamePaused?.Invoke(this, EventArgs.Empty);
            }
            else
            {
                Time.timeScale = 1f;
                GameUnpaused?.Invoke(this, EventArgs.Empty);
            }
        }   
    }

    public Vector3 GetChunkSize()
    {
        return new Vector3(chunkSizeX, chunkSizeY, chunkSizeZ);
    }

    private void Save()
    {
        // Prepare variables.
        float highScore = PlayerController.Instance.GetHighScore();

        // Create save object.
        SaveObject saveObject = new SaveObject { 
            highScore = highScore,
        };

        // Convert to json and save to file.
        string json = JsonUtility.ToJson(saveObject);
        File.WriteAllText(SAVE_FILE_PATH, json);
    }

    private void Load()
    {
        if (File.Exists(SAVE_FILE_PATH))
        {
            // Load string and convert from json to save object.
            string saveString = File.ReadAllText(SAVE_FILE_PATH);
            SaveObject saveObject = JsonUtility.FromJson<SaveObject>(saveString);

            // Load data.
            PlayerController.Instance.SetHighScore(saveObject.highScore);
        }
    }
}
