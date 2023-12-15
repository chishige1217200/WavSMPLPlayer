using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class PointerCheck : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public UnityEvent EventPointStart; // タップ開始イベント
    public UnityEvent EventPointEnd; // タップ終了イベント

    public void OnPointerDown(PointerEventData eventData)
    {
        EventPointStart.Invoke();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        EventPointEnd.Invoke();
    }
}
