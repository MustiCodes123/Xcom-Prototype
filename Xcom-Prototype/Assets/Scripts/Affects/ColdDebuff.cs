using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using DG.Tweening;

public class ColdDebuff : BaseBuff, IBuff
{
    public bool inTrigger = false;
    public float slowSpeed = 1f;

    private NavMeshAgent _navMesh;
    private Color blueColor = new Color32(165, 235, 255, 255);
    private Color _originColor;
    private Material[] _characterMatrials;

    private float _originnSpeed;
    private float blueSkinAnimationSpeed = 1;

    public ColdDebuff(BaseParticleView.Factory particleFactory, BaseSkillModel skill)
    {
        _particleFactory = particleFactory;
        _duration = skill.BuffDuration;
        _icon = skill.BuffIcon;
        buffType = BuffsEnum.ColdDebuff;
        _damage = skill.BuffPreiodDamage;
        _skill = skill;
    }

    protected override void OnApply(BaseCharacerView target)
    {
        Owner = target;
        _navMesh = target.NavMeshAgent;
        _originnSpeed = target.characterData.MoveSpeed;
        _navMesh.speed = slowSpeed;

        _characterMatrials = target.Materials;
        _originColor = Color.white;
        ChangeMaterial(blueColor, Owner);

        Owner.HealthBar.SetBuffOnBar(_icon);
    }

    protected override void OnRemove(BaseCharacerView target)
    {
        _navMesh.speed = _originnSpeed;

        ChangeMaterial(_originColor, Owner);

        Owner.HealthBar.RemoveBuufOnBar(_icon);
    }

    public void ChangeMaterial(Color color, BaseCharacerView target)
    {
        foreach (Material mat in _characterMatrials)
        {
            mat.DOColor(color, blueSkinAnimationSpeed);
        }
    }
}