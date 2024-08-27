using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaitAudio : MonoBehaviour
{
    [SerializeField] private bool audioPlayed;
    [SerializeField] private AudioClip[] baitSplashAudio;

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ocean") && !audioPlayed)
        {
            GetComponent<AudioSource>().PlayOneShot(baitSplashAudio[Random.Range(0, baitSplashAudio.Length)]);
            audioPlayed = true;
        }
    }
}
