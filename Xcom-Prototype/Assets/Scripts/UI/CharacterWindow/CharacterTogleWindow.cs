using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class CharacterTogleWindow : MonoBehaviour
{
    [SerializeField] private Button inventoryButton;
    [SerializeField] private Button skillsButton;
    [SerializeField] private Button statsButton;

    [SerializeField] private GameObject inventoryWindow;
    [SerializeField] private GameObject skillsWindow;
    [SerializeField] private GameObject statsWindow;

    [SerializeField] private GameObject inventoryWindowSelectable;
    [SerializeField] private GameObject skillsWindowSelectable;
    [SerializeField] private GameObject statsWindowSelectable;

    private void Start()
    {
     //   inventoryWindowSelectable.SetActive(true);
      //  skillsWindowSelectable.SetActive(false);
      //  statsWindowSelectable.SetActive(false);

        inventoryWindow.SetActive(true);
        skillsWindow.SetActive(false);
      //  statsWindow.SetActive(false);

        inventoryButton.onClick.AddListener(OnInventoryButtonClick);
        skillsButton.onClick.AddListener(OnSkillsButtonClick);
        statsButton.onClick.AddListener(OnStatsButtonClick);

        OnInventoryButtonClick();
    }

    private void OnInventoryButtonClick()
    {
        inventoryWindow.SetActive(true);
        skillsWindow.SetActive(false);
       // statsWindow.SetActive(false);


        inventoryButton.transform.DOScale(transform.localScale * 1.2f, 0.2f).SetEase(Ease.InOutBack);
        skillsButton.transform.localScale =Vector3.one;
        statsButton.transform.localScale = Vector3.one;


       // inventoryWindowSelectable.SetActive(true);
       //   skillsWindowSelectable.SetActive(false);
       //  statsWindowSelectable.SetActive(false);
    }

    private void OnSkillsButtonClick()
    {
        inventoryWindow.SetActive(false);
        skillsWindow.SetActive(true);
      //  statsWindow.SetActive(false);

        skillsButton.transform.DOScale(transform.localScale * 1.2f, 0.2f).SetEase(Ease.InOutBack);
        inventoryButton.transform.localScale = Vector3.one;
        statsButton.transform.localScale = Vector3.one;

        //    inventoryWindowSelectable.SetActive(false);
        //    skillsWindowSelectable.SetActive(true);
        //    statsWindowSelectable.SetActive(false);
    }

    private void OnStatsButtonClick()
    {
        inventoryWindow.SetActive(false);
        skillsWindow.SetActive(false);
     //   statsWindow.SetActive(true);

        statsButton.transform.DOScale(transform.localScale * 1.2f, 0.2f).SetEase(Ease.InOutBack);
        inventoryButton.transform.localScale = Vector3.one;
        skillsButton.transform.localScale = Vector3.one;

        //   inventoryWindowSelectable.SetActive(false);
        //   skillsWindowSelectable.SetActive(false);
        //   statsWindowSelectable.SetActive(true);
    }

    private void OnDestroy()
    {
        inventoryButton.onClick.RemoveListener(OnInventoryButtonClick);
        skillsButton.onClick.RemoveListener(OnSkillsButtonClick);
        statsButton.onClick.RemoveListener(OnStatsButtonClick);
    }



}
