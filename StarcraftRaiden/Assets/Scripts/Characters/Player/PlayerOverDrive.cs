using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerOverDrive : MonoBehaviour
{
    public static UnityAction on = delegate { };

    public static UnityAction off = delegate { };
    [Header("�S��")]
    [SerializeField] GameObject triggerVFX;
    [SerializeField] GameObject engineVFXNormal;
    [SerializeField] GameObject engineVFXOverdrive;
    [Header("����")]
    [SerializeField] AudioData onSFX;
    [SerializeField] AudioData offSFX;

    private void Awake()
    {
        on += On;
        off += Off;
    }

    private void OnDestroy()
    {
        on -= On;
        off -= Off;
    }

    private void On()
    {
        triggerVFX.SetActive(true);
        engineVFXNormal.SetActive(true);
        engineVFXOverdrive.SetActive(true);
        AudioManager.Instance.PlayRandomSFX(onSFX);
    }

    private void Off()
    {
        engineVFXNormal.SetActive(true);
        engineVFXOverdrive.SetActive(false);
        AudioManager.Instance.PlayRandomSFX(offSFX);
    }
}
