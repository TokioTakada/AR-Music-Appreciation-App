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
    private GameObject offset;
    [SerializeField]
    private Toggle showToggle;
    [SerializeField]
    private Toggle playToggle;

    void Start()
    {
        delayPanel.SetActive(false);
    }
    
    public void ChangeMode()
    {
        if (dropdown.value == 0)
        {
            delayPanel.SetActive(false);
            playbackPanel.SetActive(true);
            if (showToggle.isOn)
            {
                foreach (MeshRenderer child in offset.GetComponentsInChildren<MeshRenderer>())
                {
                    child.enabled = true;
                }
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
        if (dropdown.value == 1)
        {
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
            playbackPanel.SetActive(false);
            delayPanel.SetActive(true);
        }
    }
}
