using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseCharacterController 
{
    public BaseCharacerView characterView;
    public BaseCharacterModel characterModel;

    public BaseCharacterController(BaseCharacerView characterView, BaseCharacterModel characterModel)
    {
        this.characterView = characterView;
        this.characterModel = characterModel;
    }





}
