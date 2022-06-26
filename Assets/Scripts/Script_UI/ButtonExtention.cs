using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class ButtonExtention : Button
{
    public UnityEvent onLongPress = new UnityEvent();
    public float longPressIntervalSecond = 1.25f;

    private float pressingSecond = 0.0f;
    private bool isEnabledLongPress = true;
    private bool isPressing = false;

    private void Update()
    {
        if(isPressing && isEnabledLongPress)
        {
            pressingSecond += Time.deltaTime;
            if(pressingSecond >= longPressIntervalSecond)
            {
                onLongPress.Invoke();
                isEnabledLongPress = false;
            }
        }
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);
        isPressing = true;
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        base.OnPointerUp(eventData);
        pressingSecond = 0.0f;
        isEnabledLongPress = true;
        isPressing = false;
    }
}
