using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "FeatureLockConfig", menuName = "Data/Config/FeatureLockConfig")]
public class FeatureLockConfig : ScriptableObject
{
    [System.Serializable]
    public class FeatureLock
    {
        [field: SerializeField] public LockableGameFeature Feature { get; private set; }
        [field: SerializeField] public string LockMessage { get; private set; }
        [SerializeField] private int RequiredStage;

        public bool IsLocked(int currentStage) => RequiredStage > currentStage;
    }

    [field: SerializeField] public FeatureLock[] FeatureLocks { get; private set; }

}
