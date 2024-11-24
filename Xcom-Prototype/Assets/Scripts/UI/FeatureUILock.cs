using System.Linq;
using TMPro;
using UnityEngine;
using Zenject;

public class FeatureUILock : MonoBehaviour
{
    [Inject] private FeatureLockConfig _config;
    [Inject] private PlayerData _playerData;

    [SerializeField] private LockableGameFeature _feature;
    [SerializeField] private Collider _collider;
    [SerializeField] private TMP_Text _message;

    private Camera _mainCamera;

    private void Awake()
    {
        _mainCamera = Camera.main;
        FeatureLockConfig.FeatureLock lockSettings = _config.FeatureLocks.FirstOrDefault(cfg => cfg.Feature == _feature);

        _collider.enabled = !lockSettings.IsLocked(_playerData.GetCompanyProgres().Keys.Count);
        _message.text = lockSettings.LockMessage;
        gameObject.SetActive(lockSettings.IsLocked(_playerData.GetCompanyProgres().Keys.Count));

    }

    private void LateUpdate()
    {
        transform.LookAt(_mainCamera.transform);
    }
}
