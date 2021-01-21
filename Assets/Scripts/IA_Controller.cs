using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IA_Controller : MonoBehaviour
{
    public bool blueHappy;
    public Animator animatorBlue;

    public bool redHappy;
    public Animator animatorRed;

    void Update()
    {
        if (SceneManager.GetActiveScene().buildIndex == 6)
        {            
            if (animatorBlue.GetCurrentAnimatorStateInfo(0).IsName("BlueCrying"))
            {
                if (Level_Controller.instance.cancelAnim.GetCurrentAnimatorStateInfo(0).IsName("CancelaOpen"))
                {
                    blueHappy = true;
                    animatorBlue.SetBool("Happy", blueHappy);
                }
            }
        }
        if (SceneManager.GetActiveScene().buildIndex == 11)
        {
            if (animatorRed.GetCurrentAnimatorStateInfo(0).IsName("RedCrying"))
            {
                if (Level_Controller.instance.cancelAnim.GetCurrentAnimatorStateInfo(0).IsName("CancelaOpen"))
                {
                    redHappy = true;
                    animatorRed.SetBool("Happy", redHappy);
                }
            }
        }        
    }
    private void PlaySFX(string path)
    {
        FMODUnity.RuntimeManager.PlayOneShot(path);
    }
}
