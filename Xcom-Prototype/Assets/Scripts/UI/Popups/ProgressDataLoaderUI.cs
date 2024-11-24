using TMPro;
using UnityEngine;
using Zenject;

namespace UI.Popups
{
    public class ProgressDataLoaderUI : MonoBehaviour
    {
        public static ProgressDataLoaderUI Instance;

        [SerializeField] private TextMeshProUGUI _nameOfLoading;
        [SerializeField] private TextMeshProUGUI _sizeOfLoad;
        

        [Inject]
        private void Init()
        {
            Instance = this;
        }

       

        public void UpdateLoadingStatus(string status)
        {
            _nameOfLoading.text = status;
            _sizeOfLoad.text = "";
        }

        public void StartLoading(string fileName) => _nameOfLoading.text = $"Loading: {fileName}";

        public void UpdateProgress(float downloadedBytes, float totalBytes) => _sizeOfLoad.text = $"Size: {FormatBytes(downloadedBytes)} / {FormatBytes(totalBytes)}";
        
        private string FormatBytes(float bytes)
        {
            if (bytes < 1024) return $"{bytes:F0} B";
            if (bytes < 1024 * 1024) return $"{bytes / 1024:F1} KB";
            if (bytes < 1024 * 1024 * 1024) return $"{bytes / (1024 * 1024):F1} MB";
            return $"{bytes / (1024 * 1024 * 1024):F1} GB";
        }
    }
}