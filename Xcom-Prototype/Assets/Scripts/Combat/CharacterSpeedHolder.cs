using System.Collections.Generic;

public class CharacterSpeedHolder
{
    private Dictionary<IDamageable, float> originalSpeeds = new Dictionary<IDamageable, float>();
    
    public void SetNewSpeed(IDamageable character, float newSpeed)
    {
        originalSpeeds.TryAdd(character, character.CharacterView.characterData.MoveSpeed);
        character.CharacterView.NavMeshAgent.speed = newSpeed;
    }

    public void SetOriginSpeed(IDamageable character)
    {
        if (originalSpeeds.TryGetValue(character, out var originalSpeed))
        {
            character.CharacterView.NavMeshAgent.speed = originalSpeed;
            originalSpeeds.Remove(character);
        }
    }
}