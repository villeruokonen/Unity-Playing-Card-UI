using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CardSystem
{
    public class CardEnvironment : MonoBehaviour
    {
        public static CardEnvironment instance;
        public List<Card> Cards { get; private set; }
        
        public Canvas CardCanvas;
        public const int OffScreenPos = -5000;
        public DeckHandler deck;
        public RectTransform CanvasRect { get; private set; }
        public bool IsHoldingCard { get; private set; }

        private bool _canSetTarget;
        private GameObject _ghostCard;
        int _waitFrames = 0;
        const int _maxWaitFrames = 5;

        public void ProcessCardClick(Card card)
        {
            
            if(!card.IsBeingDragged)
            {
                card.RefreshCardObjectParent(CardCanvas.transform);
                card.BeginDrag();
                Debug.Log("Beginning drag");
            }
            else
            {
                card.StopDrag();
                Debug.Log("Stopping drag");
            }
        }

        void OnEnable()
        {
            if(instance != null && instance != this)
            {
                Destroy(this);
            }
            else
            {
                instance = this;
            }
        }

        void Start()
        {
            Initialize();
        }

        void Initialize()
        {
            Cards = new List<Card>();
            CardCanvas = GetComponentInChildren<Canvas>();
            CanvasRect = CardCanvas.GetComponent<RectTransform>();

            for(int i = 0; i < 10; i++)
            {
                CreateCardInSystem(i + 1, "Ville\nRuokonen");
            }

            SendCardsToDeck(deck);
        }

        void CreateCardInSystem(int cardValue, string cardName)
        {
            Card newCard = new Card(cardValue, cardName);
            Cards.Add(newCard);
        }

        public void AddCardToSystem(Card card)
        {
            Cards.Add(card);
        }

        public void DestroyGhostCard()
        {
            Destroy(_ghostCard);
        }

        void SendCardsToDeck(DeckHandler deck)
        {
            deck.SetCards(Cards);

            Cards = new();
        }

        public void AllowTargetClick()
        {
            _canSetTarget = true;
        }

        public void CreateGhostCard(Card source)
        {
            if(_ghostCard != null) { return; }

            _ghostCard = Instantiate(source.CardGraphicObject, CardCanvas.transform);
            Graphic[] graphics = _ghostCard.GetComponentsInChildren<Graphic>();
            for(int i = 0; i < graphics.Length; i++)
            {
                graphics[i].color = new(0.8f, 0.8f, 0.8f, 0.5f);
                graphics[i].raycastTarget = false;
            }
        }

        void AllowCardDrawTargetToClick()
        {
            if(!_canSetTarget) { return; }
            
            Vector2 pos = Input.mousePosition;
            if(_ghostCard != null) { _ghostCard.transform.position = pos; }
            
            if(Input.GetMouseButtonDown(0))
            {
                GameObject target = new ("Click Target");
                target.transform.SetParent(CardCanvas.transform, false);
                target.transform.position = pos;
                deck.SetCardDrawTarget(target.transform);
                _canSetTarget = false;
                return;
            }

            if (Input.GetKeyDown(KeyCode.Escape) || Input.GetMouseButtonDown(1))
            {
                deck.CancelDraw();
                _canSetTarget = false;
            }
        }

        void MoveCardToPosition(Card card, Vector3 position)
        {
            card.MoveCardObject(position);
        }

        void Update()
        {
            UpdateSystem();
        }

        void UpdateSystem()
        {
            foreach(Card card in Cards)
            {
                UpdateCard(card);
            }

            AllowCardDrawTargetToClick();

            if(!_canSetTarget && _ghostCard != null) { DestroyGhostCard(); }
        }

        void UpdateCard(Card card)
        {
            if(card.IsBeingDragged)
            {
                IsHoldingCard = true;
                _waitFrames++;
                card.MoveCardToPointOnScreen(Input.mousePosition);
            }
            else
            {
                IsHoldingCard = false;
            }
        }
    }
}
