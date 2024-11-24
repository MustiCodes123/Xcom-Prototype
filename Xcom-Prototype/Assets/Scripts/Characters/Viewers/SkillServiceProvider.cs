using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class SkillServiceProvider
{
    public bool IsAvalable;
    public List<BaseSkillBehaviour> Talents { get; set; } = new List<BaseSkillBehaviour>();
    public List<BaseSkillBehaviour> SkillsInUse { get; set; } = new List<BaseSkillBehaviour>();

    private BaseCharacerView _baseCharacterView { get; set; }
    private BaseCharacterModel _characterData { get; set; }
    private TalentFactory _talentFactory { get; set; }
    private List<IBuff> _buffs { get; set; } = new List<IBuff>();

    private float _skillAnimationDelay = 0.9f;
    private int _nextUpdate = 1;
    private bool _isOnPause;

    private BaseSkillBehaviour _currentSkill;
    private DynamicJoystick _currentJoystick;
    private UniRxDisposable _uniRxDisposable;
    private IEnumerator _skillDelayCorutine;

    private const float _autoBattleSkillDelay = 2f;

    public SkillServiceProvider(BaseCharacerView characerView, BaseCharacterModel model, TalentFactory talentFactory, UniRxDisposable unirxDisposable)
    {
        _baseCharacterView = characerView;
        _characterData = model;
        _talentFactory = talentFactory;
        _uniRxDisposable = unirxDisposable;
        IsAvalable = true;
        AddTalents();
        AddSkillsInUse();
    }

    public void Update()
    {
        if (!_isOnPause)
        {
            if (SkillsInUse != null)
            {
                for (int i = 0; i < SkillsInUse.Count; i++)
                {
                    SkillsInUse[i].CustomUpdate();
                }
            }

            if (Time.time >= _nextUpdate)
            {
                _nextUpdate = Mathf.FloorToInt(Time.time) + 1;
                InvokeSecondTick();
            }
        }
    }

    public void RemoveAllBuffs()
    {
        foreach (var buff in _buffs)
        {
            buff.Remove(_baseCharacterView);
        }

        _buffs.Clear();
    }
        
    public void RemoveBuff(IBuff buff)
    {
        buff.Remove(_baseCharacterView);
        _buffs.Remove(buff);
    }

    public void AddBuff(IBuff buff)
    {
        if (_baseCharacterView.IsDead)
        {
            return;
        }
        buff.Apply(_baseCharacterView);
        _buffs.Add(buff);
    }

    public void SpendMana(int value)
    {
        float finalManaCost = value * (1 - _characterData.GetManaCostReduction());

        _characterData.Mana -= Mathf.RoundToInt(finalManaCost);

        _baseCharacterView.HealthBar.SetManaValue((float)_characterData.Mana / _characterData.GetMaxMana());
    }

    public void TryUseRandomTalent()
    {
        int randomSkillIndex = UnityEngine.Random.Range(0, SkillsInUse.Count);
        var randomSkill = SkillsInUse[randomSkillIndex];

        if (IsCanUseSkill(randomSkill))
        {
            AutoBattleSkillUse(randomSkill);
        }    
    }

    public void AutoBattleSkillUse(BaseSkillBehaviour talent)
    {
        DisableAllSkills();

        _currentSkill = talent;
        _skillDelayCorutine = SkillCorutine();
        SetAnimDelay(talent.AnimationDelay);
        
        var castState = new CastState(_baseCharacterView);
        _baseCharacterView.SetState(castState);
        _baseCharacterView.SetAnimation(AnimStates.TalentID, (int)talent.Skill.Id);
        _baseCharacterView.SetAnimation(AnimStates.Cast);

        _baseCharacterView.StartCoroutine(_skillDelayCorutine);

        if (_isOnPause)
        {
            _baseCharacterView.StopCoroutine(_skillDelayCorutine);
        }

        Observable.Timer(TimeSpan.FromSeconds(_autoBattleSkillDelay)).Subscribe(_ =>
        {
            EnableAllSkills();
        }).AddTo(_uniRxDisposable.SkillDelayTimerDisposable);
    }

    public void UseSkillWithDelay(BaseSkillBehaviour talent)
    {
        _currentSkill = talent;
        _skillDelayCorutine = SkillCorutine();

        SetAnimDelay(talent.AnimationDelay);
        
        if (IsCanUseSkill(talent))
        {
            DisableAllSkills();

            _baseCharacterView.SetAnimation(AnimStates.TalentID, (int)talent.Skill.Id);
            _baseCharacterView.SetAnimation(AnimStates.Cast);
            _baseCharacterView.StartCoroutine(_skillDelayCorutine);
            
            if (_isOnPause)
            {
                _baseCharacterView.StopCoroutine(_skillDelayCorutine);
            }

            Observable.Timer(TimeSpan.FromSeconds(_skillAnimationDelay)).Subscribe(_ =>
            {
                EnableAllSkills();
            }).AddTo(_uniRxDisposable.SkillDelayTimerDisposable);
        }
    }

    public BaseSkillModel GetSkillByID(TalentsEnum talentType)
    {
        foreach(var skill in SkillsInUse)
        {
            if(skill.Skill.Id == talentType)
            {
                return skill.Skill;
            }
        }
        return null;
    }

    public bool IsBuffOnMe(BuffsEnum buffType)
    {
        foreach (var item in _buffs)
        {
            if (item.buffType == buffType)
            {
                return true;
            }
        }
        return false;
    }

    public BaseDecale UseDecale(BaseSkillBehaviour talent, DynamicJoystick joystick)
    {
        _currentJoystick = joystick;
        return talent.CreateDecale(_baseCharacterView.transform);
    }

    public bool IsCanUseSkill(BaseSkillBehaviour talent)
    {
        if (talent.IsReady() && talent.IsUsable() && _characterData.Mana >= talent.ManaCost())
            return true;
        else
            return false;
    }

    public void SetOnPause()
    {
        if (_skillDelayCorutine != null)
        {
            _isOnPause = true;
            _baseCharacterView.StopCoroutine(_skillDelayCorutine);
        }
    }

    public void SetOnPlay()
    {
        if (_skillDelayCorutine != null)
        {
            _isOnPause = false;
            _baseCharacterView.StartCoroutine(_skillDelayCorutine);
        }
    }

    public void SetAnimDelay(float value)
    {
        _skillAnimationDelay = value;
    }

    public bool IsHaveAvalableSkill()
    {
        foreach (var skill in SkillsInUse)
        {
            if(IsCanUseSkill(skill))
            {
                return true;
            }
        }
        return false;
    }

    private void InvokeSecondTick()
    {
        for (int i = 0; i < _buffs.Count; i++)
        {
            _buffs[i].Tick();
        }
    }

    private void AddTalents()
    {
        if (_characterData.CharacterTalents.Talents != null)
        {
            for (int i = 0; i < _characterData.CharacterTalents.Talents.Count; i++)
            {
                var skill = _characterData.CharacterTalents.Talents[i];
                Talents.Add(_talentFactory.GetTalent(skill));
            }
        }
    }

    private void AddSkillsInUse()
    {
        if (_characterData.SkillsInUse.Count <= 0) return;

        for (int i = 0; i < _characterData.SkillsInUse.Count; i++)
        {
            if (_characterData.SkillsInUse[i] != null)
            {
                var skill = _characterData.SkillsInUse[i];
                SkillsInUse.Add(_talentFactory.GetTalent(skill));
            }
        }
    }

    private void UseSkill(BaseSkillBehaviour talent)
    {
        SpendMana(talent.ManaCost());
        talent.Use(_baseCharacterView.Target, _baseCharacterView);
    }

    private void DisableAllSkills()
    {
        IsAvalable = false;

        foreach(var skill in _characterData.SkillsInUse)
        {
            skill.IsUssable = false;
        }
    }

    private void EnableAllSkills()
    {
        IsAvalable = true;

        foreach (var skill in _characterData.SkillsInUse)
        {
            skill.IsUssable = true;
        }
    }
    private IEnumerator SkillCorutine()
    {
        int timer = 0;
        while (timer < _skillAnimationDelay)
        {
            yield return new WaitForSeconds(1f);
            timer++;
        }

        _baseCharacterView.SetState(_baseCharacterView.OriginState);
        UseSkill(_currentSkill);
    }
}
