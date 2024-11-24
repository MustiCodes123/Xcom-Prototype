using System;
using System.Collections;
using System.Collections.Generic;
using Data.Resources.AddressableManagement;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class SmalCharacterCard : MonoBehaviour
{
    public bool IsEmpty { get { return baseCharacterInfo == null; } }
    public bool IsLocked { get { return locked.activeSelf; } }

    private Action<BaseCharacterModel> OnClicked;

    [SerializeField] private GameObject locked;
    [SerializeField] private GameObject mainObj;
    [SerializeField] private Image heroImage;
    [SerializeField] private Image rareImage;
    [SerializeField] private Image SelectedImage;
    [SerializeField] private TextMeshProUGUI heroName;
    [SerializeField] private TextMeshProUGUI heroLevel;
    [SerializeField] private Button heroButton;

    [SerializeField] private GameObject[] stars;

    private CharacterHandler _characterSelectHandler;
    private ResourceManager _resourceManager;

    public BaseCharacterModel baseCharacterInfo { get; private set; }

    public static BaseCharacterModel equippedBaseCharacterModel { get; private set; }

    [Inject]
    public void Construct(CharacterHandler characterSelectHandler)
    {
        _characterSelectHandler = characterSelectHandler;
    }

    private void Start()
    {
        heroButton.onClick.AddListener(OnHeroButtonClick);
    }

    public void SubscribeToClick(Action<BaseCharacterModel> action)
    {
        OnClicked = action;
    }

    public void SetLocked(bool locked)
    {
        rareImage.color = ItemsDataInfo.Instance.RareColors[0];
        this.locked.SetActive(locked);
        mainObj.SetActive(!locked);
    }

    public void OnHeroButtonClick()
    {
        if (OnClicked != null)
        {
            if (baseCharacterInfo != null)
            {
                OnClicked.Invoke(baseCharacterInfo);
                equippedBaseCharacterModel = baseCharacterInfo;
            }
        }
        else
        {
            if (baseCharacterInfo != null)
            {
                _characterSelectHandler?.ChangeCharacter(baseCharacterInfo);
                equippedBaseCharacterModel = baseCharacterInfo;
            }
        }
    }

    private void OnDestroy()
    {
        heroButton.onClick.RemoveAllListeners();
    }

    public void ClearSlot()
    {
        baseCharacterInfo = null;
        heroImage.gameObject.SetActive(false);
        SelectedImage.gameObject.SetActive(false);
        heroName.text = "";
        heroLevel.text = "";
        for (int i = 0; i < stars.Length; i++)
        {
            stars[i].SetActive(false);
        }
    }

    public void SetCharacterData(BaseCharacterModel heroData, bool selected = false, ResourceManager resourceManager = null)
    {
        SetLocked(false);
        gameObject.SetActive(true);
        SelectedImage.gameObject.SetActive(selected);

        if (heroData == baseCharacterInfo)
        {
            heroLevel.text = heroData.Level.ToString();
            return;
        }

        baseCharacterInfo = heroData;
        _resourceManager = resourceManager;

        for (int i = 0; i < stars.Length; i++)
        {
            stars[i].SetActive(i < heroData.Stars);
        }

        rareImage.gameObject.SetActive(true);
        rareImage.color = ItemsDataInfo.Instance.RareColors[(int)heroData.Rare];

        heroImage.sprite = _resourceManager.LoadSprite(heroData.AvatarId);
        heroImage.gameObject.SetActive(true);
        heroName.text = heroData.Name;
        heroLevel.text = heroData.Level.ToString();
    }

 
    public void SetCharacterData()
    {
        rareImage.color = ItemsDataInfo.Instance.RareColors[0];

        SetLocked(false);

        baseCharacterInfo = null;

        heroImage.gameObject.SetActive(false);
        SelectedImage.gameObject.SetActive(false);

        heroName.text = "";
        heroLevel.text = "";

        for (int i = 0; i < stars.Length; i++)
        {
            stars[i].SetActive(false);
        }
    }
}
