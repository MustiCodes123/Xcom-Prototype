using RedBjorn.ProtoTiles.Example;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePlayScript : MonoBehaviour
{

    [SerializeField] private GameObject Player;
    [SerializeField] private GameObject Enemy;
    public ExampleStart estart;

    public int currentTurn;

    void Start()
    {
        currentTurn = 0;
        UpdateTurn();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            EndTurn();
        }
    }

    private void EndTurn()
    {
        currentTurn = (currentTurn + 1) % 2; 
        UpdateTurn();
    }


    private void UpdateTurn()
    {
        if (currentTurn == 0)
        {
            Player.GetComponent<UnitMove>().enabled = true;
            Enemy.GetComponent<UnitMove>().enabled = false;
        }
        else
        { 
            Player.GetComponent<UnitMove>().enabled = false; 
            Enemy.GetComponent<UnitMove>().enabled = true; 
        }

        Debug.Log(currentTurn == 0 ? "Player's Turn" : "Enemy's Turn");
    }
}
