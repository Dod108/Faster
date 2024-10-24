using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using static UnityEngine.Rendering.DebugUI;

public class OptionsUI : MonoBehaviour
{
    [SerializeField] private ScrollbarUI sfxVolumeScrollbar;
    [SerializeField] private ScrollbarUI musicVolumeScrollbar;

    private void Start()
    {
        sfxVolumeScrollbar.SetValue(SoundManager.Instance.GetVolume());
        musicVolumeScrollbar.SetValue(MusicManager.Instance.GetVolume());

        sfxVolumeScrollbar.GetScrollbar().onValueChanged.AddListener(OnSfxVolumeChanged);
        musicVolumeScrollbar.GetScrollbar().onValueChanged.AddListener(OnMusicVolumeChanged);
    }

    private void OnSfxVolumeChanged(float volume)
    {
        SoundManager.Instance.ChangeVolume(volume);
    }

    private void OnMusicVolumeChanged(float volume)
    {
        MusicManager.Instance.ChangeVolume(volume);
    }
}
