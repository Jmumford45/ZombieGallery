using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetBehaviour : MonoBehaviour
{
    private bool beenHit = false;
    private Animator animator;
    private GameObject parent;
    private bool activated;
    private Vector3 originalPos;

    //how fast to move in x axis
    public float moveSpeed = 1f;
    //speed of Sine movement
    public float frequency = 5f;
    //size of Sine movement
    public float magnitude = 0.1f;

    // Start is called before the first frame update
    void Start()
    {
        parent = transform.parent.gameObject;
        originalPos = parent.transform.position;
        animator = parent.GetComponent<Animator>();
        ShowTarget();
    }

    //called whenever the player clicks on the object only works if you have a collider on the object 
    private void OnMouseDown()
    {
       //is it valid to hit it
       if(!beenHit && activated)
        {
            beenHit = true;
            animator.Play("flip");

            StopAllCoroutines();

            StartCoroutine(HideTarget());
        }
    }
   
    public void ShowTarget()
    {
        if(!activated)
        {
            activated = true;
            beenHit = false;
            animator.Play("isMoving");

            iTween.MoveBy(parent, iTween.Hash("y", 1.3, "easeType", "easeInOutExpo", "time", 0.5, 
                "oncomplete", "OnShown", "oncompletetarget", gameObject));
        }
    }

    public IEnumerator HideTarget()
    {
        yield return new WaitForSeconds(0.5f);

        //Move down to original spot
        iTween.MoveBy(parent.gameObject, iTween.Hash("y", (originalPos.y - parent.transform.position.y), 
            "easeType", "easeOutQuad", "loopType", "none", "time", 0.5, "oncomplete", "OnHidden", "oncompletetarget", gameObject));
    }

    //After the tween finishes we now make sure we can be shown again
    void OnHidden()
    {
        //Just to make sure the object's position resets 
        parent.transform.position = originalPos;
        activated = false;
    }

    void OnShown()
    {
        StartCoroutine("MoveTarget");
    }    

    IEnumerator MoveTarget()
    {
        var relativeEndPos = parent.transform.position;

        //are we facing right or left
        if(transform.eulerAngles == Vector3.zero)
        {
            //if we are going right positive
            relativeEndPos.x = 6;
        }
        else
        {
            //otherwise negative
            relativeEndPos.x = -6;
        }
        var movementTime = Vector3.Distance(parent.transform.position, relativeEndPos) * moveSpeed;
        var pos = parent.transform.position;
        var time = 0f;

        while(time < movementTime)
        {
            time += Time.deltaTime;
            pos += parent.transform.right * Time.deltaTime * moveSpeed;
            parent.transform.position = pos + (parent.transform.up * Mathf.Sin(Time.time * frequency) * magnitude);

            yield return new WaitForSeconds(0);
        }
        StartCoroutine(HideTarget());
    }
}
