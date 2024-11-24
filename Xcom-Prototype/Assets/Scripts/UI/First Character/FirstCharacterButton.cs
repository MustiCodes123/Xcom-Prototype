using System;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class FirstCharacterButton : MonoBehaviour
{
    public Action<SkinnedMeshRenderer[], BaseCharacterModel> Click;

    private FirstCharacterDataContainer _firstCharacterData;
    private Button _button;

    public BaseCharacterModel CharacterModel => _firstCharacterData.CharacterModel;
    public SkinnedMeshRenderer[] MeshRenderers { get; set; }
    public GameObject CharacterGameObject { get; set; }

    public void Initialize()
    {
        _button = GetComponent<Button>();
        _firstCharacterData = CharacterGameObject.GetComponent<FirstCharacterDataContainer>();

        MeshRenderers = _firstCharacterData.MeshRenderer;

        _button.onClick.AddListener(() => Click?.Invoke(MeshRenderers, _firstCharacterData.CharacterModel));
    }
}
