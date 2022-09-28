using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TransitionButton : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] GameObject from;
    [SerializeField] GameObject to;

    void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
    {
        from.SetActive(false);
        to.SetActive(true);
    }
}
