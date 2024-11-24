using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class VisualStateConfig
{
    [field: SerializeField] public Sprite BackgroundSprite { get; private set; }
    [field: SerializeField] public Sprite SlotSprite { get; private set; }
    [field: SerializeField] public Sprite StartButtonSprite { get; private set; }
    [field: SerializeField] public bool IsLocked { get; private set; }
}

public class LevelButtonView : MonoBehaviour
{
    public VisualStateConfig SelectedConfig;
    public VisualStateConfig UnselectedConfig;
    public VisualStateConfig LockedConfig;

    [SerializeField] private StartBattleButton _startButton;
    [SerializeField] private GameObject _locked;
    [SerializeField] private Image _background;
    [SerializeField] private TMP_Text _levelNameTMP;
    [SerializeField] private TMP_Text _bestTimeTMP;

    [SerializeField] private Image[] _slots;
    [SerializeField] private Image[] _enemyIcons;
    [SerializeField] private GameObject[] _stars = new GameObject[3];

    private ILevelButtonViewState _currentState;
    string levelName;
    public void Initialize(CampLevel levelData)
    {
        _levelNameTMP.text = levelData.name;
        if(levelData.stage)
        levelName = levelData.stage.Name + levelData.Name.ToString();

        showRecord();
    }
    public void showRecord()
    {
        if (PlayerPrefs.HasKey(levelName))
        {
            _bestTimeTMP.text = FormatTime(PlayerPrefs.GetFloat(levelName));
        }
        else
        {
            _bestTimeTMP.text = "";
        }
    }
    private string FormatTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60);
        int seconds = Mathf.FloorToInt(time % 60);
        return string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    public void ChangeView(VisualStateConfig config)
    {
        _background.sprite = config.BackgroundSprite;

        foreach (Image slot in _slots)
            slot.sprite = config.SlotSprite;

        _locked.SetActive(config.IsLocked);

        _startButton.Background.sprite = config.StartButtonSprite;
        _startButton.gameObject.SetActive(!config.IsLocked);
    }

    public void DisplayStars(int starsCount)
    {
        for (int i = 0; i < starsCount; i++)
        {
            _stars[i].SetActive(true);
        }
    }

    public void DisplayBestTime(string timeText)
    {
        if (int.TryParse(timeText, out int timeInSeconds))
        {
            int minutes = timeInSeconds / 60;
            int seconds = timeInSeconds % 60;

            string formattedTime = string.Format("{0:D2}:{1:D2}", minutes, seconds);
            _bestTimeTMP.text = formattedTime;
        }
        else
        {
            
            _bestTimeTMP.text = "00:00";
        }
    }

    public void DisplayEnemyIcons(Sprite[] icons)
    {
        for (int i = 0; i < _enemyIcons.Length && i < icons.Length; i++)
        {
            _enemyIcons[i].sprite = icons[i];
            _enemyIcons[i].gameObject.SetActive(true);
        }

        for (int i = icons.Length; i < _enemyIcons.Length; i++)
        {
            _slots[i].gameObject.SetActive(false);
        }
    }

    public void SetState(ILevelButtonViewState newState)
    {
        _currentState = newState;
        _currentState.ApplyState(this);
    }
}