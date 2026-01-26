using UnityEngine;
using System.Collections;

public class SongPlayer : MonoBehaviour
{
    private AudioSource audioSource;

    [Header("Plays once on game start")]
    public AudioClip FirstSong;
    [Header("Plays on loop after FirstSong")]
    public AudioClip SecondSong;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        audioSource.loop = false;
    }

    void Start()
    {
        StartCoroutine(PlayMusicSequence());
    }

    IEnumerator PlayMusicSequence()
    {
        audioSource.clip = FirstSong;
        audioSource.loop = false;
        audioSource.Play();

        yield return new WaitForSeconds(0.5f/*FirstSong.length*/);

        audioSource.clip = SecondSong;
        audioSource.loop = true;
        audioSource.Play();
    }
}
