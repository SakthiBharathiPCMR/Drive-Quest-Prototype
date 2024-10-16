using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CarMovement : MonoBehaviour
{
    // public Transform endPos;
    public LineRender lineRender;
    private Vector3[] wayPoint;
    private Vector3[] startwayPoint;
    private Vector3 startPos;
    private Vector3 startRot;
    private int counter = 0;
    public bool isMoved;
    public bool isDetect;
    private CarMovement carMovement;
    private Container containerScript;
    private float timeToMove = 0.2f;
    private float resetDistance = 2f;
    private Transform carParent ;

    public Transform container;
    public BoxCollider boxCollider;

    public CarManager carManager;
    public CarColor carColor;
    public AudioSource carAudioSource;

    private void Start()
    {
        wayPoint = lineRender.GetPosArray();
        startwayPoint = wayPoint;
        startPos = transform.position;
        startRot = transform.eulerAngles;
        boxCollider = container.GetComponent<BoxCollider>();
        carAudioSource = GetComponent<AudioSource>();
        carParent = transform.parent;
    }
    private void OnMouseDown()
    {
        if (isMoved || Input.touchCount < 1 || !carManager.isPlayable) return;
        isMoved = true;

        StartCoroutine(MoveThroughWaypoints());

    }


    private IEnumerator MoveThroughWaypoints()
    {
        foreach (Vector3 waypoint in wayPoint)
        {

            yield return StartCoroutine(MovePos(waypoint));
        }

        Debug.Log("Finished");
        container.GetChild(0).gameObject.SetActive(true);
        carManager.CheckAvaiableStorage(this, container);
        gameObject.transform.parent = container;
        gameObject.SetActive(false);
    }

    private IEnumerator MovePos(Vector3 nextPos)
    {
        Vector3 startPos = transform.position;
        float startTime = 0f;
        float endTime = timeToMove;
        while (startTime < endTime)
        {
            if (Vector3.Distance(transform.position, boxCollider.transform.position) < resetDistance + 1)
            {
                boxCollider.enabled = false;

            }
            float t = startTime / endTime;
            transform.position = Vector3.Lerp(startPos, nextPos, t);


            Vector2 direction = new Vector2(nextPos.z - transform.position.z, nextPos.x - transform.position.x);
            float angle = Mathf.Atan2(direction.normalized.y, direction.normalized.x);
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0f, angle * Mathf.Rad2Deg, 0f), t);




            startTime += Time.deltaTime;
            yield return null;

        }
        lineRender.RemoveLastPoint();
        transform.position = nextPos;
        counter++;
    }

    private void OnCollisionEnter(Collision collision)
    {

        if (collision.gameObject.TryGetComponent(out CarMovement carMovement) && isMoved && !isDetect)
        {
            carAudioSource.Play();
            Handheld.Vibrate();


            isDetect = true;
            StartCoroutine(DelayStop());
            // transform.GetComponent<BoxCollider>().enabled = false;
            //carMovement.ResetCar();
            this.carMovement = carMovement;
            PlayHitEffect(carMovement.transform.position);

        }
        else if (collision.gameObject.TryGetComponent<Container>(out Container container))
        {
            carAudioSource.Play();
            Handheld.Vibrate();


            isDetect = true;
            StopAllCoroutines();
            Invoke("ResetCar", 0.5f);
            containerScript = container;
            Vector3 hit = container.transform.position + Vector3.back;
            PlayHitEffect(hit);
        }

    }


    private void PlayHitEffect(Vector3 hitObject)
    {
        Vector3 distance = hitObject;
        distance = new Vector3(distance.x, distance.y + 1, distance.z);
        Transform hitParticle = carManager.GetHitParticleEffect();
        hitParticle.position = distance;
        hitParticle.gameObject.SetActive(true);
        StartCoroutine(DelayTurnOffParticle(hitParticle));
    }

    private IEnumerator DelayTurnOffParticle(Transform hitParticle)
    {
        yield return new WaitForSeconds(1f);
        hitParticle.gameObject.SetActive(false);
    }



    private IEnumerator DelayStop()
    {
        yield return new WaitForSeconds(0.3f);
        StopAllCoroutines();
        Invoke("ResetCar", 0.5f);
    }


    public void ResetCar()
    {
        StartCoroutine(MoveReverseThroughWaypoints());
    }


    public void ResetAfterCollided()
    {
        transform.parent = carParent;
        lineRender.ResetWayPoints(startwayPoint);
        transform.position = startPos;
        transform.eulerAngles = startRot;
        isMoved = false;
        counter = 0;
        isDetect = false;
        gameObject.SetActive(true);
        boxCollider.enabled = true;
        StopAllCoroutines();

        //transform.GetComponent<BoxCollider>().enabled = true;
    }

    private IEnumerator MoveReverseThroughWaypoints()
    {
        bool isReset = true;

        for (int i = counter - 1; i >= 0; i--)
        {
            if (isReset)
            {
                if (carMovement != null)
                {
                    if (Vector3.Distance(transform.position, carMovement.transform.position) > resetDistance + 0.5f)
                    {
                        carMovement.ResetAfterCollided();
                        isReset = false;
                    }
                }
                if (containerScript != null)
                {
                    if (Vector3.Distance(transform.position, containerScript.transform.position) > resetDistance)
                    {
                        containerScript.ResetContainer();
                        isReset = false;
                        containerScript = null;
                    }
                }

            }

            yield return StartCoroutine(MoveReversePos(startwayPoint[i]));

        }
       // Debug.Log("Reversed");
        ResetAfterCollided();
        if(carMovement!=null)
        {
            carMovement.ResetAfterCollided();
            carMovement = null;
        }
        
        // carMovement.ResetAfterCollided();

    }

    private IEnumerator MoveReversePos(Vector3 nextPos)
    {
        Vector3 startPos = transform.position;
        float startTime = 0f;
        float endTime = timeToMove;


        while (startTime < endTime)
        {
            float t = startTime / endTime;
            transform.position = Vector3.Lerp(startPos, nextPos, t);




            Vector2 direction = new Vector2(nextPos.z - transform.position.z, nextPos.x - transform.position.x);
            float angle = Mathf.Atan2(direction.normalized.y, direction.normalized.x);
            angle += Mathf.PI;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0f, angle * Mathf.Rad2Deg, 0f), t);


            startTime += Time.deltaTime;
            yield return null;

        }
        //lineRender.RemoveLastPoint();
        lineRender.AddFirstPoint(nextPos);
        transform.position = nextPos;
       






    }
}