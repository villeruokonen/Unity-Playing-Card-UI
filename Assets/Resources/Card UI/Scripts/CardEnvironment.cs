using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CardSystem
{
    public class CardEnvironment : MonoBehaviour
    {
        public static CardEnvironment instance;
        int _waitFrames = 0;
        const int _maxWaitFrames = 5;
        public List<Card> Cards { get; private set; }
        private Card _curCard;
        public Canvas CardCanvas;
        public const int OffScreenPos = -5000;
        public DeckHandler deck;
        public RectTransform CanvasRect { get; private set; }
        public bool IsHoldingCard { get; private set; }

        private List<Transform> _cardTargets;

        public void ProcessCardClick(Card card)
        {
            //if(card.Locked) { return; }
            //Debug.Log("Card is not locked");

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
            Cards = new List<Card>();
            CardCanvas = GetComponentInChildren<Canvas>();
            CanvasRect = CardCanvas.GetComponent<RectTransform>();
            _cardTargets = new();

            for(int i = 0; i < 10; i++)
            {
                CreateCardInSystem(i, "TestCard");
            }

            _cardTargets.Add(new GameObject("Card Target").GetComponent<Transform>());
            _cardTargets[0].localPosition = new Vector2(-400, 0);
            foreach(Transform t in _cardTargets) { t.SetParent(transform, true); }
            SendCardsToDeck(deck, _cardTargets[0]);
            //deck.ShuffleDeck();
            
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

        void SendCardsToDeck(DeckHandler deck, Transform drawTarget)
        {
            deck.SetCards(Cards);
            deck.SetCardDrawTarget(drawTarget);
            Cards = new();
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
