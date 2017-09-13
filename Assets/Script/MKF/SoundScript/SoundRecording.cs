using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class SoundRecording : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{

    public void OnPointerDown(PointerEventData eventData)
    {
        Event.Inst().StartMic();
    }
    bool isPlaySE;


    public void OnPointerUp(PointerEventData eventData)
    {
        MicphoneTest.IsCancelMic = false;
        Event.Inst().StartMic();
    }




}
