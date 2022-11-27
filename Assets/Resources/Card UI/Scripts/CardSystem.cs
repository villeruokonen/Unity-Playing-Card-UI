using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CardSystem
{
    public class CardSystem : MonoBehaviour
    {
        public static CardSystem instance;
        int _waitFrames = 0;
        const int _maxWaitFrames = 5;
        public List<Card> Cards { get; private set; }
        private Card _curCard;

        public Canvas CardCanvas;

        public void ProcessCardClick(Card card)
        {
            if(!card.IsBeingDragged)
            {
                card.BeginDrag();
            }
            else
            {
                card.StopDrag();
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
            AddCardToSystem(0, "TestCard");
        }

        void AddCardToSystem(int cardValue, string cardName)
        {
            Card newCard = new Card(cardValue, cardName);
            Cards.Add(newCard);
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
                _waitFrames++;
                card.MoveCardObject(Input.mousePosition);
            }
        }
    }
}
