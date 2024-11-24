using System.Collections;
using System.Collections.Generic;
using Data.Resources.AddressableManagement;
using UnityEngine;

public class Goblin : MonoBehaviour, IEnemy
{
    public BaseCharacerView View;

    [SerializeField] protected SpawnObjectOnDeath _dropSpawner;

    protected Animator Animator;

    public void Setup(BaseCharacterModel model, Vector3 position, bool isLastEnemy, TemploaryInfo temploaryInfo, ResourceManager resourceManager, ItemView.Factory itemFactory)
    {
        if (TryGetComponent<BaseCharacerView>(out var view))
        {
            View = view;
            view.OnSpawned(model, true);
            view.transform.position = position;
            Animator = view.Animator;
            SetupAnimation(Animator);

            if (_dropSpawner != null)
            {
                _dropSpawner.Setup(view, temploaryInfo, resourceManager, isLastEnemy, itemFactory);
            }
        }
    }

    public virtual void SetupAnimation(Animator animator)
    {
        if (animator != null)
            animator.SetInteger("WeaponType", (int)WeaponTypeEnum.WithOutWeapon);
    }

    public void SetupTeam(Team team)
    {
        View.SetupTeam(team);
        View.InitTapArea();
    }
}
