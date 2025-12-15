using System.Threading;
using UnityEngine;

public class AudioToggleBinder : MonoBehaviour
{
    public void Start()
    {
        AudioManager.Instance.SetMute(false);
    }

    public void OnToggleMute(bool mute)
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.SetMute(mute);
    }
}