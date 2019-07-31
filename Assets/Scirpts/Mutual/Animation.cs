using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Animation : MonoBehaviour
{
    public Animator TakeOffAnimator;
    private float timer;
    private bool isMoving;
    // Start is called before the first frame update
    void Start()
    {
        timer = 1.0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.H) && !isMoving)
        {
            isMoving = true;
            TakeOffAnimator.ResetTrigger("EndFlight");
            TakeOffAnimator.SetTrigger("StartFlight");
            //TakeOffAnimator.ResetTrigger("StartFlight");
        }

        if (isMoving)
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                TakeOffAnimator.ResetTrigger("StartFlight");
                TakeOffAnimator.SetTrigger("EndFlight");
                timer = 1.0f;
                isMoving = false;
            }
        }
    }
}
