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

    IEnumerator BlackoutActivate()
    { 
        while (true)
        { 
            loadingObject.SetActive(true);
            yield return new WaitUntil(() => FoxVRLoader.endedBlackout);
              
            loadingObject.SetActive(false);
            yield return new WaitUntil(() => !FoxVRLoader.endedBlackout);
             
        } 
    }
}
