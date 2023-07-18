using UnityEngine; 
public class FoxVRTime : MonoBehaviour
{
    private TMPro.TextMeshProUGUI text;
    System.DateTime timeNow;
    void Start()
    {
        text = GetComponent<TMPro.TextMeshProUGUI>(); 
        timeNow = System.DateTime.Now;
        InvokeRepeating("UpdateTime", 0, 60);
    }
    void UpdateTime()=> text.text = timeNow.ToString("HH:mm");
    }
