using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SoundEvent
{
    public string nameSFX = "sfx_event";
    public string audioEvent = "SfxClick";
}
public class SoundEventTrigger : MonoBehaviour, IPointerDownHandler
{
    public string OnClickSFX = "sfx_button_click";
    public static string CLICK_AUDIO_EVENT = "SfxClick";

    void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
    {
        SfxManager.Instance.Play2D(OnClickSFX, false);
    }
}
