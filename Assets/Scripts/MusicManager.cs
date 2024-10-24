using UnityEngine;

public class MusicManager : MonoBehaviour
{
    [SerializeField] private float defaultMusicVolume = 1.0f;

    public static MusicManager Instance { get; private set; }

    private const string PLAYER_PREFS_MUSIC_VOLUME = "MusicVolume";
    private float musicVolume;
    private AudioSource audioSource;

    private void Awake()
    {
        Instance = this;

        audioSource = GetComponent<AudioSource>();

        ChangeVolume(PlayerPrefs.GetFloat(PLAYER_PREFS_MUSIC_VOLUME, defaultMusicVolume));
    }

    public void ChangeVolume(float volume)
    {
        musicVolume = volume;
        audioSource.volume = volume;

        PlayerPrefs.SetFloat(PLAYER_PREFS_MUSIC_VOLUME, musicVolume);
        PlayerPrefs.Save();
    }

    public float GetVolume()
    { 
        return musicVolume; 
    }
}
