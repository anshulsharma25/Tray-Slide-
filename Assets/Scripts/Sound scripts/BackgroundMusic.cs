using System.Collections;
using UnityEngine;

public class BackgroundMusic : MonoBehaviour
{
    [SerializeField]
    private AudioClip music;

    private SoundMaster MSound => SoundMaster.Instance;

    private IEnumerator Start()
    {
        // Wait until SoundMaster is initialized
        yield return new WaitUntil(() => MSound != null);

        Debug.Log("Play Music");
       // MSound.SetMusic(true);
        MSound.SetMusicAndPlay(music);
    }
}
