using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SliderAmountDisplay : MonoBehaviour
{
    public CanvasGroup group;
    private WaitForEndOfFrame wait = new WaitForEndOfFrame();
    public TextMeshProUGUI targetText;
    private IEnumerator FadeOut() {
        float start = Time.realtimeSinceStartup;
        // Wait for 2 seconds in real time.
        while (Time.realtimeSinceStartup < start + 2f) {
            yield return null;
        }
        while (group.alpha != 0) {
            group.alpha = Mathf.MoveTowards(group.alpha, 0f, Time.unscaledDeltaTime);
            yield return wait;
        }
    }
    private void Start()
    {
        UpdateText(GetComponentInParent<Slider>().value);
        group.alpha = 1f;
    }
    public void UpdateText(float single) {
        if (single.ToString(CultureInfo.CurrentCulture).Length > 3) {
            targetText.text = single.ToString("0.0");
        } else {
            targetText.text = single.ToString(CultureInfo.CurrentCulture);
        }
        if (!isActiveAndEnabled) {
            return;
        }
        StopAllCoroutines();
       // StartCoroutine(FadeOut());
    }
}
