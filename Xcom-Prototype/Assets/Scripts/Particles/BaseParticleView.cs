using Signals;
using System;
using System.Collections;
using UnityEngine;
using Zenject;

public class BaseParticleView : MonoBehaviour
{
    public float duration = 100;
    public ParticleType particleType;

    [Inject] private SignalBus _signalBus;

    private bool _isOnPlay = true;
    private IEnumerator _particleDespawnCorutine;
    private ParticleSystem _particleSystem;
    public ParticleSystem ParticleSystem => _particleSystem;
    private void Awake()
    {
        _particleSystem = gameObject.GetComponentInChildren<ParticleSystem>();
    }

    public void OnDespawned()
    {
        _signalBus.TryUnsubscribe<ChangeGameStateSignal>(OnChangeGameState);
        gameObject.transform.SetParent(null);
        gameObject.SetActive(false);
    }

    public void OnSpawned()
    {
        gameObject.transform.localPosition = Vector3.zero;
        gameObject.SetActive(true);

        //*** Make Bug Error***
        //_signalBus.Subscribe<ChangeGameStateSignal>(OnChangeGameState);

        _particleDespawnCorutine = DespawnAfterDelay(duration);

        if (gameObject.activeInHierarchy)
        {
            StartCoroutine(_particleDespawnCorutine);
        }
    }

    public void SetParent(Transform parent)
    {
        transform.SetParent(parent);
        transform.localPosition = Vector3.zero;
    }

    private IEnumerator DespawnAfterDelay(float delay)
    {
        int timeer = 0;
        while (timeer < delay)
        {
            yield return new WaitForSeconds(1f);
            timeer++;
        }
        OnDespawned();
    }

    private void OnChangeGameState()
    {
        if (_isOnPlay)
        {
            _isOnPlay = false;
            StopCoroutine(_particleDespawnCorutine);
            _particleSystem.Pause();
        }
        else
        {
            _isOnPlay = true;
            StartCoroutine(_particleDespawnCorutine);
            _particleSystem.Play();
        }
    }

    private void OnDestroy()
    {
        _signalBus.TryUnsubscribe<ChangeGameStateSignal>(OnChangeGameState);
    }

    public class Factory : PlaceholderFactory<ParticleType, BaseParticleView>
    {
    

    }

}
