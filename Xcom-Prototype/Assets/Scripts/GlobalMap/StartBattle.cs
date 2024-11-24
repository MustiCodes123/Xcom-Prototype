using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartBattle : MonoBehaviour
{
    private void OnMouseDown()
    {
        Debug.Log("Start Battle");

        SceneManager.LoadScene(1);
    }
    
}
