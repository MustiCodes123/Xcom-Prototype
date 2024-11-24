using System.Collections;
using System.Collections.Generic;
using Data.Resources.AddressableManagement;
using UnityEngine;

public interface IEnemy
{
    public void SetupTeam(Team team);
    public void Setup(BaseCharacterModel model, Vector3 position, bool isLastEnemy, TemploaryInfo temploaryInfo, ResourceManager resourceManager, ItemView.Factory itemFactory);
}

