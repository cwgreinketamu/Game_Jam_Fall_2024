using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioSource lightningSound;

    public AudioSource lightningHitSound;
    public AudioClip lightningClip;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void playSound(AudioSource sound)
    {
        if(sound == lightningSound || sound == lightningHitSound){
            if(sound == lightningHitSound){
                lightningHitSound.Play();
            }
            Debug.Log("Lightning sound played");
            sound.time = 1.9f;
            sound.PlayOneShot(lightningClip, 0.75f);
            sound.SetScheduledEndTime(AudioSettings.dspTime+(6.0f-1.9f));
        }
        else{
            Debug.Log("Other sound played", sound);
            sound.Play();
        }
    }
}
