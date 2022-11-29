using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace CardSystem
{
    public class DeckHandler : MonoBehaviour
    {
        public bool AllowDraw { get; private set; } = true;
        public List<Card> Cards;
        private const float _deckCardYDelta = 8;
        private Button _guiButton;

        private UnityAction _onDraw;
        
        private Transform _drawTarget;
        
        void Start()
        {
            Cards = new();
            _guiButton = GetComponent<Button>();
            _onDraw += DrawCard;
            _guiButton.onClick.AddListener(_onDraw);
        }

        public void LockDeck()
        {
            AllowDraw = false;
        }
        public void UnlockDeck()
        {
            AllowDraw = true;
        }
        public void AddCardToDeckTop(Card card)
        {
            Cards.Add(card);
            RenderCards();
        }
        public void AddCardToDeckBottom(Card card)
        {
            Card temp = null;
            Cards.Add(temp);

            for(int i = 1; i < Cards.Count; i++)
            {
                Cards[i] = Cards[i - 1];
            }

            Cards[0] = card;

            RenderCards();
        }

        public void AddCards(List<Card> cards)
        {
            Cards.AddRange(cards);
            RenderCards();
        }

        public void DrawCard()
        {
            if(!AllowDraw || Cards.Count == 0 || CardEnvironment.instance.IsHoldingCard) { return; }

            Card c = Cards.Last();
            Cards.Remove(c);
            c.SetCardObjectParent(CardEnvironment.instance.CardCanvas.transform);
            c.ResetGraphicsObject();
            c.MoveCardObject(_drawTarget.position);
            _guiButton.targetGraphic = null;
            CardEnvironment.instance.AddCardToSystem(c);
            c.UnlockCard();

            Debug.Log("Drawing card from deck");
            //CardEnvironment.instance.ProcessCardClick(c);

            RenderCards();
        }

        public void SetCardDrawTarget(Transform target)
        {
            _drawTarget = target;
        }

        public void DrawCardToTarget(Transform target)
        {
            SetCardDrawTarget(target);
            DrawCard();
        }

        public void SetCards(List<Card> cards)
        {
            Cards = new();
            Cards = cards;
            RenderCards();
        }

        public void ShuffleDeck()
        {
            var rand = new System.Random();
            var shuffledDeck = Cards.OrderBy (x => rand.Next()).ToList();

            Cards = shuffledDeck;
        }

        public void RenderCards()
        {
            if(Cards.Count == 0) { return; }
            //ShuffleDeck();
            float yAdjust = (Cards.Count * _deckCardYDelta) / 2; // center cards on deck
            Vector3 deckPosition = transform.position;
            deckPosition.y -= yAdjust;

            Vector3 offscreen = new Vector3(CardEnvironment.OffScreenPos, CardEnvironment.OffScreenPos, 0);

            for(int i = 0; i < Cards.Count; i++)
            {
                Cards[i].SetCardObjectParent(transform);
                Cards[i].MoveCardObject(offscreen);
                Cards[i].SetCardGraphicsObjectParent(transform);
                Cards[i].MoveCardGraphicsObject(deckPosition);
                deckPosition.y += _deckCardYDelta;
                Cards[i].LockCard();
            }

            //Cards.Last().UnlockCard();
            
            _guiButton.targetGraphic = Cards.Last().CardGraphic;
            
        }
    }
}
