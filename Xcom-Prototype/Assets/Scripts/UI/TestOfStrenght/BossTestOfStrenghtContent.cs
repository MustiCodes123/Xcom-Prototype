using UnityEngine;

public class BossTestOfStrenghtContent : BossContent
{
    [SerializeField] private BossSkillSlot _skillSlotPrefab;
    [SerializeField] private Transform _skillSlotContainer;

    public override void Initialize(BossData boss, BossWindowView windowView)
    {
        base.Initialize(boss, windowView);
        for (int i = 0; i < boss.SkillsIconPaths.Length; i++)
        {
            var sprite = _resourceManager.LoadSprite(boss.SkillsIconPaths[i]);
            var slot = Instantiate(_skillSlotPrefab, _skillSlotContainer);
            slot.Init(sprite);
        }
    }
}
