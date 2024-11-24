using UnityEngine;
using UnityEngine.UI;

public class ExecutingAnotherButtonEvent : MonoBehaviour
{
    public void ExecuteAnotherbuttonEvent(Button anotherbutton)
    {
        anotherbutton.onClick.Invoke();
    }
}