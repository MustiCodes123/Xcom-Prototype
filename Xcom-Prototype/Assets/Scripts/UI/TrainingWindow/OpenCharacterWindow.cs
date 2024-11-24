using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class OpenCharacterWindow : MonoBehaviour
{
    [SerializeField] private Button _openCharWindowButton; 
    private UIWindowManager _windowManager; 

    [Inject] private void Construct(UIWindowManager windowManager) => _windowManager = windowManager;

    private void Awake() => _openCharWindowButton.onClick.AddListener(OpenWindow);

    private void OnDestroy() => _openCharWindowButton.onClick.RemoveListener(OpenWindow);

    private void OpenWindow()
{
    if (_windowManager.Windows.ContainsKey(WindowsEnum.TrainingWindow))
    {
        _windowManager.ShowWindow(WindowsEnum.TrainingWindow);
    }
    else
    {
        Debug.LogError("TrainingWindow is not registered in UIWindowManager.");
    }
}
}