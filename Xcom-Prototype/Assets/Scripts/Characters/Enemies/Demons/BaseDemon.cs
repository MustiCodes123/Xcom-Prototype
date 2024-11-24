using Data.Resources.AddressableManagement;
using UnityEngine;

public class BaseDemon : MonoBehaviour, IEnemy
{
    public BaseCharacerView View;

    [SerializeField] protected SpawnObjectOnDeath _dropSpawner;

    protected Animator Animator;
    protected const string WeaponType = "WeaponType";

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

        SetInventory();
    }

    public virtual void SetupAnimation(Animator animator)
    {
        if (animator != null)
            animator.SetInteger(WeaponType, (int)WeaponTypeEnum.Sword);
    }

    public virtual void SetInventory()
    {

    }

    public void SetupTeam(Team team)
    {
        View.SetupTeam(team);
        View.InitTapArea();
    }
}
