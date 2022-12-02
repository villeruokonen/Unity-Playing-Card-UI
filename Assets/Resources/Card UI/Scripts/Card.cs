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
        public Graphic CardGraphic { get; private set; }
        public bool Locked { get; private set; } = false;
        public Vector2 CardPosition { get { return _cardObjectContainer.transform.localPosition; } private set {} }
        public GameObject CardObject { get { return _cardObjectContainer; } private set {} }
        public GameObject CardGraphicObject { get { return _cardGraphicsContainer; } private set {} }

        private Button _cardButton;

        public Card(int cardValue, string cardName)
        {
            CardValue = cardValue;
            CardName = cardName;
            GameObject cardPrefab = Resources.Load("Card UI/Prefabs/Card") as GameObject;
            _cardObjectContainer = GameObject.Instantiate(cardPrefab, CardEnvironment.instance.CardCanvas.transform);
            _cardGraphicsContainer = _cardObjectContainer.transform.Find("Card Graphics").gameObject;
            CardGraphic =  _cardGraphicsContainer.transform.Find("Card Sprite").GetComponent<Graphic>();
            _cardNameText =  _cardGraphicsContainer.transform.Find("Card Name Text").GetComponent<TMPro.TMP_Text>();
            _cardValueText =  _cardGraphicsContainer.transform.Find("Card Value Text").GetComponent<TMPro.TMP_Text>();
            _cardButton = _cardObjectContainer.GetComponent<Button>();
            _cardButton.onClick.AddListener(delegate { GetClickEvent(); });
            _cardNameText.text = CardName;
            _cardValueText.text = CardValue.ToString();
        }

        GameObject _cardObjectContainer;
        GameObject _cardGraphicsContainer;
        TMPro.TMP_Text _cardNameText;
        TMPro.TMP_Text _cardValueText;
        
        public bool IsBeingDragged {get; private set;}

        public void BeginDrag()
        {
            if(Locked) { return; }
            IsBeingDragged = true;
        }

        public void LockCard()
        {
            Locked = true;
            _cardButton.interactable = false;
        }

        public Button ReturnButton()
        {
            return _cardButton;
        }

        public void GetClickEvent()
        {
            Debug.Log("Card clicked!");
            CardEnvironment.instance.ProcessCardClick(this);
        }

        public void UnlockCard()
        {
            Locked = false;
            _cardButton.interactable = true;
        }

        public void MakeGhost()
        {
            CardGraphic.color = new(0.5f, 0.5f, 0.5f, 0.5f);
        }

        public void MoveCardObject(Vector2 newPosition)
        {
            _cardObjectContainer.transform.localPosition = newPosition;
        }
        public void MoveCardToPointOnScreen(Vector2 mousePosition)
        {
            RectTransform canvasRect = CardEnvironment.instance.CanvasRect;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, mousePosition, null, out Vector2 result);
            //result += canvasRect.sizeDelta / 2;
            _cardObjectContainer.transform.localPosition = result;

        }

        public void MoveCardGraphicsObject(Vector2 newPosition)
        {
            _cardGraphicsContainer.transform.position = newPosition;
        }

        public void SetCardGraphicsObjectParent(Transform parent)
        {
            _cardGraphicsContainer.transform.SetParent(parent, true);
        }

        public void ResetGraphicsObject()
        {
            _cardGraphicsContainer.transform.SetParent(_cardObjectContainer.transform, true);
            _cardGraphicsContainer.transform.localPosition = Vector3.zero;
        }

        public void SetCardObjectParent(Transform parent)
        {
            _cardObjectContainer.transform.SetParent(parent, true);
        }

        public void RefreshCardObjectParent(Transform targetParent)
        {
            _cardObjectContainer.transform.SetParent(null, true);
            _cardObjectContainer.transform.SetParent(targetParent, true);
        }

        public void StopDrag()
        {
            IsBeingDragged = false;
        }
    }
}
