using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonAudio : MonoBehaviour
{
    [SerializeField] AudioData TargetAudio;

    void Start()
    {
        GetComponent<Button>().onClick.AddListener(PlayAudio);
    }

    // Update is called once per frame
    void PlayAudio()
    {
        AudioManager.Instance.PlayAudioStatic(TargetAudio);
    }
}
