using System.Collections.Generic;
using UnityEngine;

public class ChestMultipleRewardCardsView : MonoBehaviour
{
    [SerializeField] private Transform _container;
    [SerializeField] private List<ChestCardPrefab> _cards;
    [SerializeField] private ChestCardPrefab _cardPrefab;

    public void Show(List<ItemTemplate> items)
    {
        for (int i = 0; i < items.Count; i++)
        {
            if (_cards[i] != null)
            {
                _cards[i].Show(items[i]);
            }

            else
            {
                ChestCardPrefab card = Instantiate(_cardPrefab, _container);
                _cards.Add(card);

                card.Show(items[i]);
            }
        }
    }
}