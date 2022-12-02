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

        private bool _cancelDraw = false;
        
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

        public void CancelDraw()
        {
            _cancelDraw = true;
        }

        public void DrawCard()
        {
            if(!AllowDraw || Cards.Count == 0 || CardEnvironment.instance.IsHoldingCard) { return; }

            Card c = Cards.Last();
            _cancelDraw = false;
            CardEnvironment.instance.CreateGhostCard(c);
            StartCoroutine(CardMoveLerp(c, c.CardPosition));
            Debug.Log("Drawing card from deck");
        }

        IEnumerator CardMoveLerp(Card card, Vector2 startPosition)
        {
            _drawTarget = null;
            _guiButton.targetGraphic = null;
            //yield return new WaitForSeconds(0.25f); // wait a bit so target can't be set instantly
            while(_drawTarget == null)
            {
                // await target from card environment
                CardEnvironment.instance.AllowTargetClick();

                if(_cancelDraw)
                {
                    _cancelDraw = false;
                    _guiButton.targetGraphic = card.CardGraphic;
                    Debug.Log("Cancelled draw");
                    RenderCards();
                    yield break;
                }

                yield return new WaitForEndOfFrame();
            }

            Cards.Remove(card);
            card.ResetGraphicsObject();
            card.SetCardObjectParent(CardEnvironment.instance.CardCanvas.transform);
            card.MoveCardObject(transform.localPosition);

            Vector2 targetPosition = _drawTarget.localPosition;
            float delta = 0;
            float distance = Vector2.Distance(startPosition, targetPosition);
            

            while (distance > 0.2f)
            {
                Vector2 movementIncrement = Vector2.Lerp(card.CardPosition, targetPosition, delta * Time.deltaTime);
                card.MoveCardObject(movementIncrement);
                distance = Vector2.Distance(card.CardPosition, targetPosition);
                delta += (Time.deltaTime * 45);
                yield return new WaitForEndOfFrame();
            }

            card.MoveCardObject(targetPosition);
            
            CardEnvironment.instance.AddCardToSystem(card);
            card.UnlockCard();

            Destroy(_drawTarget.gameObject);
            RenderCards();

            yield return null;
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
            ShuffleDeck();
            RenderCards();
        }

        public void ShuffleDeck()
        {
            var rand = new System.Random();
            var shuffledDeck = Cards.OrderBy (x => rand.Next()).ToList();

            Cards = shuffledDeck;

            RenderCards();
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

            _guiButton.targetGraphic = Cards.Last().CardGraphic;
            
        }
    }
}
