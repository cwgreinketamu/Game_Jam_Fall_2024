using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColumnBehaviour : MonoBehaviour
{
    public Animator anim;
    private float randomNum;
    private bool isOogaBoogaRunning = false;
    void Update()
    {
        if(!isOogaBoogaRunning){
            StartCoroutine(OogaBooga());
        }

    }

    private IEnumerator OogaBooga()
    {
        isOogaBoogaRunning = true;
        randomNum = Random.Range(0, 10);
        Debug.Log(randomNum);
        anim.SetTrigger("Random");
        yield return new WaitForSeconds(randomNum);
        anim.SetTrigger("Random");
        isOogaBoogaRunning = false;

    }
}
