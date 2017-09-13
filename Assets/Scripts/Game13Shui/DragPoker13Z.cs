using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using System;

public class DragPoker13Z : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IBeginDragHandler,IDragHandler
{

    private UnityEvent _onHandDown = new UnityEvent();
    private UnityEvent _onHandEnter = new UnityEvent();

    /// <summary>当开始滑动
    /// </summary>
    public UnityEvent onHandDown
    {
        get
        {   
            return _onHandDown;
        }

        private set
        {
            _onHandDown = value;
        }
    }

    /// <summary>当进入
    /// </summary>
    public UnityEvent onHandEnter
    {
        get
        {
            return _onHandEnter;
        }

        private set
        {
            _onHandEnter = value;
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        //throw new NotImplementedException();
        Debug.Log("OnBeginDrag");
        if (onHandDown != null)
        {
            onHandDown.Invoke();
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        //Debug.Log("OnDrag");
        //throw new NotImplementedException();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        //Debug.Log("OnPointerDown");
        //if (onHandDown!=null)
        //{
        //    onHandDown.Invoke();
        //}
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        //Debug.Log("OnPointerEnter");
        if (onHandEnter != null)
        {
            onHandEnter.Invoke();
        }
    }

}
