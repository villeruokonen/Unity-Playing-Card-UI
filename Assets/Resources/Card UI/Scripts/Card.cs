using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace CardSystem
{
    public class Card
    {
        public int CardValue { get; private set; } = 0;
        public string CardName { get; private set; } = "NewCard";
        public bool Locked { get; private set; } = false;
        public Card(int cardValue, string cardName)
        {
            CardValue = cardValue;
            CardName = cardName;
            GameObject cardPrefab = Resources.Load("Card UI/Prefabs/Card") as GameObject;
            _cardObject = GameObject.Instantiate(cardPrefab, CardSystem.instance.CardCanvas.transform);
            _cardNameText = _cardObject.transform.Find("Card Name Text").GetComponent<TMPro.TMP_Text>();
            _cardObject.GetComponent<Button>().onClick.AddListener(delegate { GetClickEvent(); });
            _cardNameText.text = CardName;
            _cardValueText = _cardObject.transform.Find("Card Value Text").GetComponent<TMPro.TMP_Text>();
            _cardValueText.text = CardValue.ToString();
        }

        GameObject _cardObject;
        TMPro.TMP_Text _cardNameText;
        TMPro.TMP_Text _cardValueText;
        
        public bool IsBeingDragged {get; private set;}

        public void BeginDrag()
        {
            if(Locked) { Debug.Log("Tried to begin drag on locked card"); return; }
            IsBeingDragged = true;
        }

        public void LockCard()
        {
            Locked = true;
        }

        public void GetClickEvent()
        {
            Debug.Log("Card clicked!");
            CardSystem.instance.ProcessCardClick(this);
        }

        public void UnlockCard()
        {
            Locked = false;
        }

        public void MoveCardObject(Vector3 newPosition)
        {
            if(Locked) { Debug.Log("Tried to move locked card"); return; }
            _cardObject.transform.position = newPosition;
        }

        public void StopDrag()
        {
            IsBeingDragged = false;
        }
    }
}
