using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Tiles : MonoBehaviour, IPointerClickHandler {

    public void OnPointerClick(PointerEventData eventData)
    {
        TimerScript.mouseClick++;

        if (TimerScript.mouseClick%2==0)
        {
            TimerScript.firstClicked.transform.position = gameObject.transform.position;
            TimerScript.firstClicked.transform.SetSiblingIndex(gameObject.transform.GetSiblingIndex());
            TimerScript.firstClicked.transform.localScale = new Vector3(1f, 1f, gameObject.transform.localScale.z);
            gameObject.transform.SetSiblingIndex(TimerScript.index);
            gameObject.transform.position = TimerScript.startingPos;
        }
        else
        {
            TimerScript.startingPos = gameObject.transform.position;
            TimerScript.firstClicked = gameObject;
            TimerScript.index = gameObject.transform.GetSiblingIndex();
            gameObject.transform.localScale = new Vector3(1.5f, 1.5f, gameObject.transform.localScale.z);

        }
    }
}
