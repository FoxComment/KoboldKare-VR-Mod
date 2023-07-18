using UnityEngine;
public class FoxVRSplash : MonoBehaviour
{
    [SerializeField] string[] splashes;
    TMPro.TextMeshProUGUI splTT;
    private void Start()
    {
        splTT = GetComponent<TMPro.TextMeshProUGUI>();  
        InvokeRepeating("UpdSplsh",6,6);
    }
    void UpdSplsh() => splTT.text = splashes[Random.Range(0, splashes.Length - 1)];
}
