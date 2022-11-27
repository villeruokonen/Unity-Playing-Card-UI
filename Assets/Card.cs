using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace CardSystem
{
    public class Card : MonoBehaviour, ISelectHandler
    {
        Selectable _select;
        bool _dragging;
        int waitFrames = 0;
        BaseEventData curData;

        // Start is called before the first frame update
        void Start()
        {
            _select = GetComponent<Selectable>();
        }

        // Update is called once per frame
        void Update()
        {
            if(_dragging)
            {
                waitFrames++;
                transform.position = Input.mousePosition;
            }

            if(Input.GetMouseButtonDown(0) && waitFrames >= 5) { ClickEvent(); }
            
        }

        public void AllowCardSelection(bool value)
        {
            _select.enabled = value;
        }

        public void OnSelect(BaseEventData data)
        {
            _dragging = true;
            curData = data;
            AllowCardSelection(false);
        }

        public void ClickEvent()
        {
            if(_dragging)
            {
                _dragging = false;
                _select.OnDeselect(curData);
                AllowCardSelection(true);
                waitFrames = 0;
            }
        }
    }
}
