using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioSource lightningSound;
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
        if(sound == lightningSound){
            Debug.Log("Lightning sound played");
            sound.time = 1.9f;
            sound.Play();
            sound.SetScheduledEndTime(AudioSettings.dspTime+(6.0f-1.9f));
        }
        else{
            sound.Play();
        }
    }
}
