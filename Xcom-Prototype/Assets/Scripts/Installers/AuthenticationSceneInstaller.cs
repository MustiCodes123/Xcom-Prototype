using System.Collections;
using UnityEngine;
using Zenject;

public class AuthenticationSceneInstaller : MonoInstaller
{
    [SerializeField] private AuthenticationManager _authenticationManager;

    public override void InstallBindings()
    {
        Container.Bind<AuthenticationManager>().FromInstance(_authenticationManager).AsSingle();
    }
}