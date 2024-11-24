using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class DropItemView : ItemView
{
    [SerializeField] protected RewardDropText _rewardTextBox;
    [SerializeField] protected GameObject _itemViewBox;

    protected int _gold;
    protected int _exp;
    protected DropItemViewEnum _dropItemType;
    private bool _isCollected = false;
    private bool _isAutoCollectStarted = false;

    public virtual void Setup(int count, DropItemViewEnum itemType, Color color)
    {
        _dropItemType = itemType;

        _rewardTextBox.SetTetxAndColor(this, ("+ " + count.ToString()), color);

    }

    public virtual void Setup(BaseItem item, DropItemViewEnum itemType)
    {
        _rewardTextBox.SetTetxAndColor(this, item.itemName, ItemsDataInfo.Instance.RareColors[(int)item.Rare]);
        _dropItemType = itemType;
    }
    private void OnMouseDown()
    {
        Collect();
    }

    private void Update()
    {
        if (!_isCollected && !_isAutoCollectStarted && UIGameWinWIndow.IsVictoryWindowShown)
        {
            _isAutoCollectStarted = true;
            StartCoroutine(AutoCollectAfterDelay(1f));
        }
    }

    private IEnumerator AutoCollectAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Collect();
    }

    public virtual void Collect()
    {
        if (_isCollected) return;
        _isCollected = true;
        _rewardTextBox.gameObject.SetActive(true);
        _itemViewBox.SetActive(false);

        StartCoroutine(DestroyAfterDelay(3f));
    }

    private IEnumerator DestroyAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(this.gameObject);
    }
}
