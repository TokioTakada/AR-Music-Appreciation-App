using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayAudioSources : MonoBehaviour
{
    [SerializeField]
    private GameObject offset;
    [SerializeField]
    private Toggle playToggle;

    public void OnClickToggle()
    {
        AudioSource[] audioSources = offset.GetComponentsInChildren<AudioSource>();

        if (playToggle.isOn)
        {
            // 取得したAudioSourceを再生
            foreach (AudioSource audioSource in audioSources)
            {
                audioSource.Play();
            }
        }
        else
        {
            foreach (AudioSource audioSource in audioSources)
            {
                audioSource.Stop();
            }
        }
    }
}
