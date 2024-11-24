using UnityEngine;

public class FightWindowPool
{
    private UIObjectPool<LevelButton> _levelButtonsPool;
    private UIObjectPool<StageSelectionButton> _stageButtonsPool;
    private UIObjectPool<CharacterButton> _characterButtonsPool;
    private UIObjectPool<RewardItemView> _rewardIconsPool;

    public FightWindowPool(
        UIObjectPool<LevelButton> levelButtonsPool,
        UIObjectPool<StageSelectionButton> stageButtonsPool,
        UIObjectPool<CharacterButton> characterButtonsPool,
        UIObjectPool<RewardItemView> rewardIconsPool)
    {
        _levelButtonsPool = levelButtonsPool;
        _stageButtonsPool = stageButtonsPool;
        _characterButtonsPool = characterButtonsPool;
        _rewardIconsPool = rewardIconsPool;
    }

    public LevelButton SpawnLevelButton(Transform parent) => _levelButtonsPool.Spawn(parent);

    public void RemoveLevelButton(LevelButton button) => _levelButtonsPool.Despawn(button);

    public StageSelectionButton SpawnStageButton(Transform parent) => _stageButtonsPool.Spawn(parent);

    public void RemoveStageButton(StageSelectionButton button) => _stageButtonsPool.Despawn(button);

    public CharacterButton SpawnCharacterButton(Transform parent) => _characterButtonsPool.Spawn(parent);

    public void RemoveCharacterButton(CharacterButton button) => _characterButtonsPool.Despawn(button);

    public RewardItemView SpawnRewardIcon(Transform parent) => _rewardIconsPool.Spawn(parent);

    public void RemoveRewardIcon(RewardItemView icon) => _rewardIconsPool.Despawn(icon);
}