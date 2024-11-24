using System.Collections;
using System.Collections.Generic;
using Data.Resources.AddressableManagement;
using UnityEngine;

public class Bandit : MonoBehaviour, IEnemy
{
    public BaseCharacerView View;

    [SerializeField] protected SpawnObjectOnDeath _dropSpawner;

    protected Animator Animator;

    protected string weaponTypeParameterName = "WeaponType";

    public void Setup(BaseCharacterModel model, Vector3 position, bool isLastEnemy, TemploaryInfo temploaryInfo, ResourceManager resourceManager, ItemView.Factory itemFactory)
    {
        if (TryGetComponent<BaseCharacerView>(out var view))
        {
            View = view;
            view.OnSpawned(model, true);
            view.transform.position = position;
            Animator = view.Animator;
            SetupAnimation(Animator);
            SetInventory();

            if (_dropSpawner != null)
            {
                _dropSpawner.Setup(view, temploaryInfo, resourceManager, isLastEnemy, itemFactory);
            }
        }
    }

    public virtual void SetupAnimation(Animator animator)
    {
        if (animator != null)
            animator.SetInteger(weaponTypeParameterName, (int)WeaponTypeEnum.WithOutWeapon);
    }

    public void SetupTeam(Team team)
    {
        View.SetupTeam(team);
        View.InitTapArea();
    }

    protected virtual void SetInventory()
    {

    }
}
