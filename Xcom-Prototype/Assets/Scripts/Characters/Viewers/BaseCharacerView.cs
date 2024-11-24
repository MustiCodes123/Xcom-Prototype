using System;
using System.Collections;
using DG.Tweening;
using Signals;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using Zenject;

[RequireComponent(typeof(AgentLinkMover))]
public class BaseCharacerView : MonoBehaviour, IPoolable<BaseCharacterModel, bool>, IDamageable
{
    public float OriginalNavMeshSpeed => _originalNavMeshSpeed;
    public float OriginalAnimatorSpeed => _originalAnimatorSpeed;
    
    public BaseCharacerView CharacterView => this;
    public CombatServiceProvider CombatServiceProvider { get; private set; }
    public SkillServiceProvider SkillServiceProvider { get; private set; }
    public CharacterSlotsHolder SlotsHolder => slotsHolder;
    public NavMeshAgent NavMeshAgent => navMeshAgent;
    public CharacterController CharacterController => characterController;
    public UISlicedBar HealthBar => healthBar;
    public Material[] Materials => materials;
    public Animator Animator => animator;
    public Vector3 Position => transform.position;
    public bool IsDead => characterData.Health <= 0;
    public Team Team => team;
    public IBehaviourState CurrentState => currentState;
    public UiEnemyIndicator EnemyIndicator => _enemyIndicator;

    [SerializeField] protected Animator animator;
    [SerializeField] protected NavMeshAgent navMeshAgent;
    [SerializeField] protected UISlicedBar healthBar;
    [SerializeField] protected Transform combatTextParent;
    [SerializeField] protected NavmeshPathDraw navmeshPathDraw;
    [SerializeField] protected CharacterSlotsHolder slotsHolder;
    [SerializeField] protected UiEnemyIndicator _enemyIndicator;

    public float _disableDelay = 7f;

    protected CharacterController characterController;

    public bool IsImmortal { get; set; }
    public bool IsBot;
    public bool IsBoss;
    public bool IsSelected;
    public bool AutoBattle;

    public BaseCharacerView Target { get; set; }
    public IBehaviourState OriginState { get; set; }
    public IMemoryPool Pool { get; set; }
    public BaseCharacterModel characterData { get; protected set; }
    public Action OnDie { get; set; }
    public Action OnDestroyAction { get; set; }
    public Action<int> OnTakeDamage { get; set; }

    public WeaponItem Weapon { get; private set; }
    public WeaponItem OffHandWeapon { get; private set; }
    public ItemView RangeWeaponView { get; private set; }

    [Inject] protected SignalBus signalBus;
    [Inject] protected CharactersRegistry charactersRegistry;
    [Inject] protected BaseParticleView.Factory particleFactory;
    [Inject] protected BaseShootProjectile.Factory weaponProjectileFactory;
    [Inject] protected BaseProjectile.Factory projectileFactory;
    [Inject] protected BaseDecale.Factory DecaleFactory;
    [Inject] protected CombatText.Factory combatTextFactory;
    [Inject] protected TalentFactory talentFactory;
    [Inject] protected TemploaryInfo temploaryInfo;
    [Inject] protected UniRxDisposable _uniRxDisposable;
    [Inject] protected IShapeMath ShapeMath;
    [Inject] protected CustomGameTimeScaleHandler CustomGameTimeScaleHandler;
    [Inject] private IShapeCollection _shapeCollection;

    protected Team team;

    protected IBehaviourState currentState;

    protected CharacterCardView myBattleCard;

    protected Material[] materials;

    private float _originalNavMeshSpeed;
    private float _originalAnimatorSpeed;
    private BoxShape _boxShape;
    private ObjectDisabler _objectDisabler;

    private void Awake()
    {
        if(_enemyIndicator == null && IsBot)
            Debug.LogError("Enemy doesnt have UiEnemyIndicator gameobject");
    }

    public void InitTapArea()
    {
        if (team == Team.Enemies)
        {
            var boxShape = GetComponent<BoxShape>();
            if (boxShape != null)
            {
                boxShape.Initialize(_shapeCollection);
            }
            else
            {
                Debug.LogWarning("BoxShape component not found on enemy: " + gameObject.name);
            }
        }
    }

    private void Start()
    {
        CombatServiceProvider = new CombatServiceProvider(this, weaponProjectileFactory, _uniRxDisposable, particleFactory, charactersRegistry);
        var animationService = Animator.AddComponent<AnimationServiceProvider>();
        animationService.CharacterView = this;
        
        signalBus.Subscribe<ChangeGameStateSignal>(OnChangeGameState);
        signalBus.Subscribe<ChangeGameSpeedSignal>(OnChangeGameSpeed);

        _objectDisabler = new ObjectDisabler();
    }

    private void OnChangeGameState(ChangeGameStateSignal signal)
    {
        if (signal.NewState == GameState.Pause)
        {
            StopPlaying();
        }
        else
        {
            ResumePlaying();
        }
    }

    protected void Update()
    {
        if (currentState != null)
        {
            currentState.Update();
        }

        if (SkillServiceProvider != null)
        {
            SkillServiceProvider.Update();
        }
    }

    protected void FixedUpdate()
    {
        if (currentState != null)
        {
            currentState.FixedUpdate();
        }
    }

    public void Select()
    {
        if (myBattleCard.IsActive)
        {
            IsSelected = true;
            navmeshPathDraw.gameObject.SetActive(true);
            myBattleCard.SelectCard();
        }
    }

    public void Unselect()
    {
        IsSelected = false;
        navmeshPathDraw.gameObject.SetActive(false);
        myBattleCard.UnSelectCard();
    }

    public void SetAnimation(AnimStates anim, int id = 0)
    {
        switch (anim)
        {
            case AnimStates.Idle:
                animator.SetFloat(AnimStates.Walk.ToString(), 0);
                break;
            case AnimStates.Walk:
                animator.SetFloat(anim.ToString(), navMeshAgent.velocity.magnitude);
                break;
            case AnimStates.Run:
                break;
            case AnimStates.Atack:
                animator.SetTrigger(anim.ToString());
                break;
            case AnimStates.CriticalAttack:
                break;
            case AnimStates.Combo:
                break;
            case AnimStates.WeaponType:
                animator.SetInteger(anim.ToString(), id);
                break;
            case AnimStates.TalentID:
                animator.SetInteger(anim.ToString(), id);
                break;
            case AnimStates.Resurect:
                animator.SetTrigger(anim.ToString());
                break;
            default:
                animator.SetTrigger(anim.ToString());
                break;
        }
    }

    public void SetState(IBehaviourState state)
    {
        if (currentState != null)
        {
            OriginState = currentState;

            if (!currentState.Exit())
            {
                return;
            }
        }

        currentState = state;
        currentState.Enter();
    }

    public void Heal(int param)
    { 
        characterData.Health += param;
        if(characterData.Health > characterData.GetMaxHP())
        {
            characterData.Health = characterData.GetMaxHP();
        }

        healthBar.SetHPValue((float)characterData.Health / characterData.GetMaxHP());

        var combatText = combatTextFactory.Create(param.ToString(), combatTextParent);
        combatText.SetColor(Color.green);
    }

    public void RegenerateMana()
    {
        float manaRegenerationAmount = characterData.GetMaxMana() * characterData.GetManaRegenerationSpeed() * Time.deltaTime;

        characterData.Mana += Mathf.RoundToInt(manaRegenerationAmount);

        if (characterData.Mana > characterData.GetMaxMana())
        {
            characterData.Mana = characterData.GetMaxMana();
        }

        healthBar.SetManaValue((float)characterData.Mana / characterData.GetMaxMana());
    }

    public void SetDestination(Vector3 destination)
    {
        navmeshPathDraw.destinationPos = destination;
        navMeshAgent.SetDestination(destination);
    }

    public void SetTarget(IDamageable target)
    {
        if (target == null || target.CharacterView == null)
        {
            currentState.SetNewTarget(null);
            return;
        }

        if (target.CharacterView == this)
        {
            return;
        }

        navmeshPathDraw.destinationPos = target.Position;
        currentState.SetNewTarget(target);
    }

    public void Die()
    {
        OnDie?.Invoke();
        navMeshAgent.isStopped = true;
        SetState(new DeathState(this));
        characterController.enabled = false;
        healthBar.gameObject.SetActive(false);
        charactersRegistry.CheckForDeadCharacters();
        RestoreAllComponents();
        var box = GetComponent<BoxShape>();
        _shapeCollection.Remove(box);
        charactersRegistry.RemoveCharacter(this);

        if (myBattleCard != null)
        {
            myBattleCard.SetDefeated();
        }

        _objectDisabler.DisableAfterDelay(this, _disableDelay);
    }

    public void ResurectMe()
    {
        gameObject.SetActive(true);
        SetAttackState();
        SetAnimation(AnimStates.Resurect);
        characterData.CalculateSpeed();
        _originalNavMeshSpeed = NavMeshAgent.speed;
        _originalAnimatorSpeed = Animator.speed;

        AddArmor(characterData.MaxArmor);
        characterData.Health = characterData.GetMaxHP();
        characterData.Mana = characterData.GetMaxMana();
        healthBar.SetShieldIcon();
        UpdateHPBars();


        WeaponTypeEnum WeaponType = characterData.GetCurrentWeaponType();
        int AnimationID;
        if (WeaponType == WeaponTypeEnum.Hummer)
        {
            AnimationID = (int)WeaponTypeEnum.Sword;
        }
        else if (WeaponType == WeaponTypeEnum.TwoHandedHummer)
        {
            AnimationID = (int)WeaponTypeEnum.TwoHandedSword;
        }
        else
        {
            AnimationID = (int)WeaponType;
        }
        SetAnimation(AnimStates.WeaponType, AnimationID);

        navMeshAgent.isStopped = false;
        characterController.enabled = true;
        healthBar.gameObject.SetActive(true);
        charactersRegistry.AddCharacter(this);

        RestoreAllComponents();

        if (myBattleCard != null)
        {
            myBattleCard.SetAlive();
        }
    }

    public virtual void OnDespawned()
    {
        Pool = null;
    }

    public virtual void OnSpawned(BaseCharacterModel baseCharacter, bool isBot)
    {
        characterData = baseCharacter;
        SetupMaterials();

        AddWeapons();

        ChangeCharacterName();
        IsBot = isBot;

        SetAttackState();

        charactersRegistry.AddCharacter(this);

        characterData.CalculateSpeed();
        NavMeshAgent.speed = characterData.MoveSpeed;
        characterData.Score.ResetScore();
        _originalNavMeshSpeed = NavMeshAgent.speed;
        _originalAnimatorSpeed = Animator.speed;
        
        AddArmor(characterData.MaxArmor);
        healthBar.SetShieldIcon();
        UpdateHPBars();

        WeaponTypeEnum WeaponType = baseCharacter.GetCurrentWeaponType();
        int AnimationID;
        if (WeaponType == WeaponTypeEnum.Hummer)
        {
            AnimationID = (int)WeaponTypeEnum.Sword;
        }
        else if (WeaponType == WeaponTypeEnum.TwoHandedHummer)
        {
            AnimationID = (int)WeaponTypeEnum.TwoHandedSword;
        }
        else
        {
            AnimationID = (int)WeaponType;
        }
        SetAnimation(AnimStates.WeaponType, AnimationID);
        characterController = GetComponent<CharacterController>();

        SkillServiceProvider = new SkillServiceProvider(this, characterData, talentFactory, _uniRxDisposable);

        StartCoroutine(Wait());
    }

    public void SetAttackState()
    {
        if (IsBot)
        {
            SetState(new AutoAttackState(this, charactersRegistry, temploaryInfo));
        }
        else
        {
            SetState(new PlayerInputState(this, charactersRegistry, temploaryInfo, DecaleFactory, ShapeMath));                
        }
    }

    protected IEnumerator Wait()
    {
        navMeshAgent.enabled = false;
        yield return new WaitForEndOfFrame();
        navMeshAgent.enabled = true;
    }

    protected void SetupMaterials()
    {
        var renderers = GetComponentsInChildren<Renderer>();
        materials = new Material[renderers.Length];
        for (int i = 0; i < renderers.Length; i++)
        {
            materials[i] = renderers[i].material;
        }
    }

    public void AddWeapons()
    {
        Weapon = characterData.GetWeapon(false);
        OffHandWeapon = characterData.GetWeapon(true);
        if (slotsHolder != null && slotsHolder.ItemSlots != null)
        {
            foreach (var slot in slotsHolder.ItemSlots)
            {
                if (slot.ItemView == null) continue;
                if (slot.ItemView is WeaponView weaponView)
                {
                    RangeWeaponView = weaponView;
                }
            }
        }
        else
        {
            Debug.LogError("SlotsHolder is null");
        }
    }

    public void UpdateHPBars()
    {
        healthBar.SetLevel(characterData.Level);
        healthBar.SetMaxHP(characterData.GetMaxHP());
        healthBar.SetMaxMana(characterData.GetMaxMana());

        healthBar.SetHPValue((float)(characterData.Health / characterData.GetMaxHP()));
    }

    public void SetupTeam(Team team)
    {
        this.team = team;
        healthBar.SetTeam(this.team, this);
    }

    public void SetBattleCard(CharacterCardView card) => myBattleCard = card;

    public CharacterCardView BattleCharacterCard() => myBattleCard;

    protected virtual void ChangeCharacterName() => gameObject.name = characterData.Name;

    public void RestoreCharacter()
    {
        characterData.Armor = characterData.MaxArmor;
        characterData.Health = characterData.GetMaxHP();
        characterData.Mana = characterData.GetMaxMana();
    }

    private void CalculateDamage(ref int damage, AttackType attackType)
    {
        float resistance = 0;

        if (attackType == AttackType.Magical)
        {
            resistance = (float)damage / 100 * characterData.GetMagicalResistance();
        }
        else
        {
            resistance = (float)damage / 100 * characterData.PhysicalResistance;
        }

        damage -= (int)resistance;

        if (characterData.Armor > 0)
        {
            characterData.DamageArmor(damage);
            if (characterData.Armor < 0)
                damage = Mathf.Abs(characterData.Armor);
            else
                damage = 0;
        }

        OnTakeDamage?.Invoke(damage);
        characterData.Health -= damage;
        if (characterData.Health <= 0)
        {
            if (IsImmortal == false)
                Die();
            else
            {
                characterData.Health = characterData.GetMaxHP();
                OnDie?.Invoke();
            }

        }
        else if (currentState.State == State.Attack)
        {
            SetAnimation(AnimStates.Hit);
        }
    }

    private bool CalculateDodge(float accuracy)
    {
        float random = UnityEngine.Random.Range(0, 1f);

        float totalDodge = random;

        float dodgeIncreaseFactor = 1.0f;

        foreach (var item in characterData.EquipedItems)
        {
            if (item is AmuletItem amulet)
            {
                dodgeIncreaseFactor += amulet.DodgeChance;
            }
        }

        totalDodge *= dodgeIncreaseFactor;
        totalDodge += characterData.DodgeAdditional;

        return totalDodge > accuracy;
    }

    public bool TakeDamage(int damage, AttackType attackType, float accuracy = 100, Color color = new Color())
    {
        if (IsDead) return false;
        var isBlockedAttack = IsEnoughtChanceToBlock();
        var isAttackDodged = CalculateDodge(accuracy);
        CombatText text = null;

        if (isAttackDodged)
        {
            text = combatTextFactory.Create("miss", combatTextParent);
        }
        else if (isBlockedAttack)
        {
            text = combatTextFactory.Create("blocked", combatTextParent);
        } 
        else if (!isBlockedAttack || !isAttackDodged)
        {
            CalculateDamage(ref damage, attackType);
            text = combatTextFactory.Create(damage.ToString(), combatTextParent);

            if (IsBot || IsBoss)
                text.SetColor(color);
            else
                text.SetColor(Color.red);
        }

        if (!IsBot && !IsBoss)
        {
            text.ChangeRandomPosition();
            text.DoTransformScale(1.25f, 0.625f, UnityEngine.Random.Range(0.8f, 1f));
        }

        if (!IsBoss)
        {
            healthBar.SetHPValue((float)characterData.Health / characterData.GetMaxHP());
            healthBar.SetArmor((float)characterData.Armor / characterData.MaxArmor);
        }
        
        if (myBattleCard != null)
        {
            myBattleCard.UpdateCardHPBar(characterData.Health);
        }

        return !isBlockedAttack || !isAttackDodged;
    }

    public bool TakeCriticalDamage(int damage, AttackType attackType, float accuracy = 100, Color color = new Color())
    {
        if (IsDead) return false;
        var isBlockedAttack = IsEnoughtChanceToBlock();
        var isAttackDodged = CalculateDodge(accuracy);

        if (color == new Color())
        {
            UnityEngine.ColorUtility.TryParseHtmlString("#ECE672", out Color lightYellow);
            color = lightYellow;
        }

        if (isAttackDodged)
        {
            combatTextFactory.Create("miss", combatTextParent);
        }
        else if (isBlockedAttack)
        {
            combatTextFactory.Create("blocked", combatTextParent);
        } 
        else if (!isBlockedAttack || !isAttackDodged)
        {
            int criticalDamage = (int)(damage * Weapon.CriticalMultiplayer);
            CalculateDamage(ref criticalDamage, attackType);
            var text = combatTextFactory.Create(criticalDamage.ToString(), combatTextParent);
            if (IsBot || IsBoss)
                text.SetColor(color);
            else
                text.SetColor(Color.red);
        }

        if (!IsBoss)
        {
            healthBar.SetHPValue((float)characterData.Health / characterData.GetMaxHP());
            healthBar.SetArmor((float)characterData.Armor / characterData.MaxArmor);
        }
    
        if (myBattleCard != null)
        {
            myBattleCard.UpdateCardHPBar(characterData.Health);
        }

        return !isBlockedAttack || !isAttackDodged;
    }
    
    public void AddArmor(int value)
    {
        characterData.Armor += value;
        characterData.Armor = Math.Clamp(characterData.Armor, 0, characterData.MaxArmor);
        healthBar.SetArmor((float)characterData.Armor / characterData.MaxArmor);
    }

    public void SetRangeWeapon(RangeWeaponView weaponView)
    {
        RangeWeaponView = weaponView;
    }

    public void LookAtTarget(Transform target)
    {
        transform.LookAt(target);
    }

    public void ChangeAutoAtack(bool autobattle)
    {
        AutoBattle = autobattle;
    }

    public bool IsMyTeammate(BaseCharacerView target)
    {
        if(this.team == target.team)
            return true;
        else
            return false;
    }

    public virtual bool IsEnoughtChanceToBlock()
    {
        float random = UnityEngine.Random.Range(0, 100);
        return random < characterData.BlockChance;
    }

    private void StopPlaying()
    {
        animator.speed = 0f;
        navMeshAgent.isStopped = true;
        navMeshAgent.speed = 0f;
        navMeshAgent.velocity = Vector3.zero;
        navMeshAgent.updatePosition = false;
        SkillServiceProvider.SetOnPause();
        if(myBattleCard != null)
            myBattleCard.IsPaused = true;
    }

    private void ResumePlaying()
    {
        animator.speed = 1f;
        navMeshAgent.isStopped = false;
        navMeshAgent.updatePosition = true;
        navMeshAgent.speed = characterData.MoveSpeed;
        SkillServiceProvider.SetOnPlay();
        if (myBattleCard != null)
            myBattleCard.IsPaused = false;
    }

    public void SpeedUpAllComponents()
    {
        CustomGameTimeScaleHandler.SpeedUpAllComponents(this);
    }

    public void RestoreAllComponents()
    {
        CustomGameTimeScaleHandler.RestoreAllComponents(this);
    }

    private void OnChangeGameSpeed(ChangeGameSpeedSignal signal)
    {
        if (signal.NewSpeed == 2)
        {
            SpeedUpAllComponents();
            Debug.Log("x2");
        }
        else
        {
            RestoreAllComponents();
            Debug.Log("normal");
        }
    }


    private void OnDestroy()
    {
        OnDie = null;
        OnTakeDamage = null;
        signalBus.Unsubscribe<ChangeGameStateSignal>(OnChangeGameState);
        signalBus.Unsubscribe<ChangeGameSpeedSignal>(OnChangeGameSpeed);
        OnChangeGameSpeed(new ChangeGameSpeedSignal(1));

        if (SkillServiceProvider != null)
        {
            SkillServiceProvider.RemoveAllBuffs();
        }

        _objectDisabler.CancelPendingDisable();

        OnDestroyAction = null;
    }
    
    public class Factory : PlaceholderFactory<BaseCharacterModel, BaseCharacerView>
    {

    }
}