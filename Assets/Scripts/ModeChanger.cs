using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ModeChanger : MonoBehaviour
{
    [SerializeField]
    private TMP_Dropdown dropdown;
    [SerializeField]
    private GameObject playbackPanel;
    [SerializeField]
    private GameObject delayPanel;
    [SerializeField]
    private GameObject feedbackPanel;
    [SerializeField]
    private GameObject offset;
    [SerializeField]
    private Toggle showToggle;
    [SerializeField]
    private Toggle playToggle;

    void Start()
    {
        delayPanel.SetActive(false);
        feedbackPanel.SetActive(false);
    }
    
    public void ChangeMode()
    {
        // すべてのパネルを非表示にする
        playbackPanel.SetActive(false);
        delayPanel.SetActive(false);
        feedbackPanel.SetActive(false);

        if (dropdown.value == 0 || dropdown.value == 2)
        {
            // playback モードまたは feedback モードのとき
            if (dropdown.value == 0)
            {
                playbackPanel.SetActive(true);
                if (showToggle.isOn)
                {
                    foreach (MeshRenderer child in offset.GetComponentsInChildren<MeshRenderer>())
                    {
                        child.enabled = true;
                    }
                }
            }
            else
            {
                feedbackPanel.SetActive(true);
            }

            if (playToggle.isOn)
            {
                AudioSource[] audioSources = offset.GetComponentsInChildren<AudioSource>();
                foreach (AudioSource audioSource in audioSources)
                {
                    audioSource.Play();
                }
            }
        }
        else if (dropdown.value == 1)
        {
            // delay モードの処理
            if (showToggle.isOn)
            {
                foreach (MeshRenderer child in offset.GetComponentsInChildren<MeshRenderer>())
                {
                    child.enabled = false;
                }
            }
            if (playToggle.isOn)
            {
                AudioSource[] audioSources = offset.GetComponentsInChildren<AudioSource>();
                foreach (AudioSource audioSource in audioSources)
                {
                    audioSource.Stop();
                }
            }
            delayPanel.SetActive(true);
        }
    }
}
