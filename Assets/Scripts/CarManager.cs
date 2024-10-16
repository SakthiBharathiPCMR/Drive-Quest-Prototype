using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum CarColor
{
    blue,
    green
}
public class CarManager : MonoBehaviour
{

    public Transform[] storagelocation;
    public Transform[] shipTransforms;
    public Transform[] greenshiplocation;
    public Transform[] blueshiplocation;
    public Transform[] hitparticleFx;
    public Transform[] smokeparticleFx;
    public AudioSource manager2audioSource;
    public Transform winTransform;

    public bool isPlayable = true;

    public bool isGreen = true;
    private int shipMoved;


    public Container[] containersArray;
    public ShipScript[] cargoArray;
    public CarMovement[] carMovementArray;
    public LineRender[] lineRenderArray;





    public void ResetGame()
    {
        foreach (Container item in containersArray)
        {
            item.ResetContainer();
        }
        foreach (ShipScript item in cargoArray)
        {
            item.ResetCargo();
        }
        foreach (CarMovement item in carMovementArray)
        {
            item.ResetAfterCollided();
        }
        foreach (LineRender item in lineRenderArray)
        {
            item.UpdateLine();
        }
        winTransform.gameObject.SetActive(false);
        isPlayable = true;
        isGreen = true;
        shipMoved = 0;
    }




    public Transform GetHitParticleEffect()
    {
        foreach (Transform item in hitparticleFx)
        {
            if(!item.gameObject.activeInHierarchy)
            {
                return item;
            }
        }
        return null;
    }
    public Transform GetSmokeParticleEffect()
    {
        foreach (Transform item in smokeparticleFx)
        {
            if (!item.gameObject.activeInHierarchy)
            {
                return item;
            }
        }
        return null;
    }








    public void CheckAvaiableStorage(CarMovement carMovement, Transform container)
    {
        if (isGreen && carMovement.carColor == CarColor.green)
        {
            bool isAdded = true;

            foreach (Transform ship in greenshiplocation)
            {
                if (ship.childCount == 0&& isAdded)
                {
                    StartCoroutine(MoveContainer(container, ship));
                    isAdded = false;

                }
            }
            shipMoved++;
            if (shipMoved >= 3)
            {
                StartCoroutine(ShipMover());

            }
        }
        else if (!isGreen && carMovement.carColor == CarColor.blue)
        {
            bool isAdded = true;

            foreach (Transform ship in blueshiplocation)
            {
                if (ship.childCount == 0&&isAdded)
                {
                    StartCoroutine(MoveContainer(container, ship));
                    isAdded = false;
                }
            }
            shipMoved++;

            if (shipMoved >= 6)
            {
               // Debug.Log("Won");
                //winTransform.gameObject.SetActive(true);
                StartCoroutine(WinTranformText());


                return;
            }
        }
        else
        {
            bool isAdded = true;

            foreach (Transform ship in storagelocation)
            {

                if (ship.childCount == 0&& isAdded)
                {
                    StartCoroutine(MoveContainer(container, ship));
                    isAdded = false;

                }
            }
        }
    }

    private IEnumerator ShipMover()
    {
        isPlayable = false;
        yield return new WaitForSeconds(1f);

        for (int i = 0; i < 2; i++)
        {
            StartCoroutine(MoveShip(shipTransforms[i]));
        }
        yield return new WaitForSeconds(3f);
        isGreen = false;
        ArrangeRemaingCars();
        yield return new WaitForSeconds(1f);
        isPlayable = true;
    }



    private IEnumerator MoveContainer(Transform containerTransform, Transform EndTransform)
    {
        Vector3 startPos = containerTransform.position;
        float startTime = 0f;
        float endTime = 0.2f;
        containerTransform.eulerAngles = new Vector3(0f, -90f, 0f);
        while (startTime < endTime)
        {
            float t = startTime / endTime;
            containerTransform.position = Vector3.Lerp(startPos, EndTransform.position, t);

            startTime += Time.deltaTime;
            yield return null;

        }
        containerTransform.position = EndTransform.position;
        containerTransform.parent = EndTransform;


        Transform particle = GetSmokeParticleEffect();

        particle.position = EndTransform.position+(Vector3.back*1.5f)-(Vector3.up*0.5f);
        particle.gameObject.SetActive(true);
        StartCoroutine(DelayTurnOffParticle(particle));
        manager2audioSource.Play();
        Handheld.Vibrate();


    }
    private IEnumerator DelayTurnOffParticle(Transform hitParticle)
    {
        yield return new WaitForSeconds(1f);
        hitParticle.gameObject.SetActive(false);
    }
    private IEnumerator MoveShip(Transform containerTransform)
    {
        Vector3 startPos = containerTransform.position;
        Vector3 endPos = startPos + Vector3.right * 15f;
        float startTime = 0f;
        float endTime = 3f;
        while (startTime < endTime)
        {
            float t = startTime / endTime;
            containerTransform.position = Vector3.Lerp(startPos, endPos, t);

            startTime += Time.deltaTime;
            yield return null;

        }
        containerTransform.position = endPos;

    }


    private void ArrangeRemaingCars()
    {
        int index = 0;
        foreach (Transform child in storagelocation)
        {
            if (child.childCount!=0)
            {
                StartCoroutine(MoveContainer(child.GetChild(0).transform, blueshiplocation[index]));
                index++;
                shipMoved++;
                if (shipMoved >= 6)
                {
                    //   Debug.Log("Won");
                    //winTransform.gameObject.SetActive(true);
                    StartCoroutine(WinTranformText());
                    return;
                }
            }
        }
    }

    private IEnumerator WinTranformText()
    {
        yield return new WaitForSeconds(1f);
        winTransform.gameObject.SetActive(true);
        isPlayable = false;
    }


}
