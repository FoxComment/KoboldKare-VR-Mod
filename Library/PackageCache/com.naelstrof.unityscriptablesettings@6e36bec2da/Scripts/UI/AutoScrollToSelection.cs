using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UnityScriptableSettings {
public class AutoScrollToSelection : MonoBehaviour {
    public float pixelBuffer = 10f;
    public ScrollRect rect;
    private GameObject lastSelectedObj = null;
    void Update() {
        EventSystem sys = EventSystem.current;
        
        if (sys == null) {
            return;
        }
        GameObject obj = sys.currentSelectedGameObject;
        if (obj == null || obj == lastSelectedObj) {
            return;
        }
        bool IsChild = false;
        for(Transform t = obj.transform;t!=null;t=t.parent) {
            if (t==rect.content.transform) {
                IsChild = true;
                break;
            }
        }
        if (!IsChild) {
            return;
        }
        lastSelectedObj = obj;
        RectTransform viewRect = rect.content.parent.GetComponent<RectTransform>();

        RectTransform selRect = obj.GetComponent<RectTransform>();
        Vector3 min = selRect.TransformPoint(selRect.rect.min);
        Vector3 max = selRect.TransformPoint(selRect.rect.max);
        Vector3 viewMin = viewRect.TransformPoint(viewRect.rect.min);
        Vector3 viewMax = viewRect.TransformPoint(viewRect.rect.max);
        float pixelBuff = Mathf.Min(viewRect.rect.height/2f, pixelBuffer);
        Vector3 downDistance = rect.content.InverseTransformVector(min - (viewMin+Vector3.one*pixelBuff));
        Vector3 upDistance = rect.content.InverseTransformVector(max - (viewMax-Vector3.one*pixelBuff));

        if (upDistance.y > 0f) {
            rect.content.anchoredPosition -= new Vector2(0f, upDistance.y);
        }
        if (downDistance.y < 0f) {
            rect.content.anchoredPosition -= new Vector2(0f, downDistance.y);
        }
        if (upDistance.y > 0f || downDistance.y < 0f){
            rect.content.anchoredPosition = new Vector2(rect.content.anchoredPosition.x, Mathf.Clamp(rect.content.anchoredPosition.y, 0f, -viewRect.rect.height+rect.content.rect.height));
        }
    }
}

}