using System.Collections;
using UnityEngine;

public class BlackoutScreen : MonoBehaviour
{
    [SerializeField] private CanvasGroup vrCanvas;
    [SerializeField] private GameObject loadingObject;
     

    private void Awake()
    {
        vrCanvas = GetComponent<CanvasGroup>();
        StartCoroutine(BlackoutActivate());
    }

    /*
    public void GetActiveCamera(bool forced)
    {

        vrCanvas.GetComponent<Canvas>().planeDistance = 1.5f;

        if (forced)
        {            
            vrCanvas.GetComponent<Canvas>().worldCamera = GameObject.Find("OrbitCamera").GetComponent<Camera>();    //Make it nicer later
            return;
        }

        if(FoxVRLoader.GetUICamera() != null) vrCanvas.GetComponent<Canvas>().worldCamera = FoxVRLoader.GetUICamera();
        else vrCanvas.GetComponent<Canvas>().worldCamera = FoxVRLoader.GetActiveCameraTransform().GetComponent<Camera>();

    }*/


    IEnumerator BlackoutActivate()
    {
      //float duration = .5f;
      //float startTime = Time.time;
        while (true)
        {
            //vrCanvas.alpha = 1;
            loadingObject.SetActive(true);
            yield return new WaitUntil(() => FoxVRLoader.endedBlackout);
             
            //startTime = Time.time;
          /*  while (Time.time < startTime + duration)
            {
                float t = (Time.time - startTime) / duration;
                vrCanvas.alpha = 1f - t;
                yield return null;
            }
            vrCanvas.alpha = 0;
          */
            loadingObject.SetActive(false);
            yield return new WaitUntil(() => !FoxVRLoader.endedBlackout);

            //GetActiveCamera(false);
        } 
    }
}
