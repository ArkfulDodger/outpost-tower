using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Music : MonoBehaviour
{
    public AudioSource music1;
    public AudioSource music2;

    Dictionary<AudioSource, float> audioDefaults = new Dictionary<AudioSource, float>();


    private void Start()
    {
        audioDefaults.Add(music1, music1.volume);
        audioDefaults.Add(music2, music2.volume);
    }


    void PlayAtDefault(AudioSource audio)
    {
        audio.volume = audioDefaults[audio];
        audio.Play();
    }

    void FadeOutAll()
    {
        StopAllCoroutines();

        foreach (KeyValuePair<AudioSource, float> pair in audioDefaults)
        {
            if (pair.Key.isPlaying)
            {
                StartCoroutine(FadeOut(pair.Key, 1, true));
            }
        }
    }

    void FadeOutAllButClip(AudioSource audio, float fadeTime, bool pauseOnly, bool fadeInNewClip)
    {
        StopAllCoroutines();

        foreach (KeyValuePair<AudioSource, float> pair in audioDefaults)
        {
            if (pair.Key != audio && pair.Key.isPlaying)
            {
                StartCoroutine(FadeOut(pair.Key, fadeTime, pauseOnly));
            }
        }

        if (fadeInNewClip)
            StartCoroutine(FadeIn(audio, fadeTime));
    }

    IEnumerator FadeOut(AudioSource audio, float fadeTime, bool pauseOnly)
    {
        float startingVolume = audio.volume;
        float time = 0;
        while (time < fadeTime)
        {
            audio.volume = Mathf.Lerp(startingVolume, 0, time/fadeTime);
            time += Time.deltaTime;
            yield return null;
        }
        audio.volume = 0;

        if (pauseOnly)
            audio.Pause();
        else
            audio.Stop();
    }

    IEnumerator FadeIn(AudioSource audio, float fadeTime)
    {
        float startingVolume = audio.isPlaying ? audio.volume : 0;
        float time = 0;
        audio.Play();
        while (time < fadeTime)
        {
            audio.volume = Mathf.Lerp(startingVolume, audioDefaults[audio], time/fadeTime);
            time += Time.deltaTime;
            yield return null;
        }
        audio.volume = audioDefaults[audio];
    }
}