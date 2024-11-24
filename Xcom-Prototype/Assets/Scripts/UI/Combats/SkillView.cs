using Signals;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zenject;

public class SkillView : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public static Action<bool> DecaleDragStateChanged;

    public Image icon;
    public TextMeshProUGUI skillName;
    public Button button;
    public BaseSkillBehaviour talent;
    public BaseCharacerView character;
    public BaseDecale Decale;
    public DynamicJoystick Joystick;

    [SerializeField] private Image fillImage;

    private bool _isUseInProgress;
    
    [Inject]
    private SignalBus _signalBus;

    private void Awake()
    {
        button = GetComponent<Button>();
        Joystick = GetComponent<DynamicJoystick>();
        Joystick.enabled = true;
        _signalBus.Subscribe<ChangeGameStateSignal>(OnChangeGameState);
    }

    private void Update()
    {
        if(talent != null && !talent.Skill.IsUssable)
        {
            fillImage.fillAmount = 1;
        }
        else
        {
            fillImage.fillAmount = Interpolate(0, talent.Skill.Cooldown, 0f, 1f, talent.cooldown);
        }
    }

    public void OnPointerDown(PointerEventData data)
    {
        if (character.SkillServiceProvider.IsCanUseSkill(talent) && character.CurrentState is not CastState)
        {
            var castState = new CastState(character);
            character.SetState(castState);
            _isUseInProgress = true;

            if (talent.GetDecale() != null)
            {
                AactivateJoystic();
                Joystick.OnPointerDown(data);
                
                var decale = character.SkillServiceProvider.UseDecale(talent, Joystick);
                decale.Setup(Joystick, character);
                Decale = decale;

                DecaleDragStateChanged?.Invoke(true);
            }
        }
    }

    public void OnPointerUp(PointerEventData data)
    {
        if(_isUseInProgress)
        {
            if (Decale != null)
            {
                Decale.OnDispawned();
                DeactivateJoystic();

                DecaleDragStateChanged?.Invoke(false);
            }

            UseSkillOnClick(character, talent);
            _isUseInProgress = false;
        }
    }

    private void UseSkillOnClick(BaseCharacerView character, BaseSkillBehaviour skill)
    {
        character.SkillServiceProvider.UseSkillWithDelay(skill);
    }

    public void DeactivateJoystic()
    {
        Joystick.GetBackGround().gameObject.SetActive(false);
        Joystick.enabled = false;
    }

    public void AactivateJoystic()
    {
        Joystick.enabled = true;
        Joystick.transform.position = gameObject.transform.position;
    }

    private float Interpolate(float Xmin, float Xmax, float Ymin, float Ymax, float Xvalue)
    {
        return Ymin + (Xvalue - Xmin) / (Xmax - Xmin) * (Ymax - Ymin);
    }
    
    private void OnChangeGameState(ChangeGameStateSignal signal)
    {
        if (signal.NewState == GameState.Gameplay)
        {
            button.interactable = true;
           
        }
        else
        {
            button.interactable = false;
          
        }
    }
    
    private void OnDestroy()
    {
        _signalBus?.Unsubscribe<ChangeGameStateSignal>(OnChangeGameState);
    }
}
