using RedBjorn.ProtoTiles.Example;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePlayScript : MonoBehaviour
{

    [SerializeField] private GameObject Player;
    [SerializeField] private GameObject Enemy;
    public ExampleStart estart;
    public static bool playerEnded = false;
    public static bool aiEnded = true;
    

    void Start()
    {
        playerEnded = false;
        aiEnded = true;
    }

    void Update()
    {
        if (playerEnded)
        {
            Enemy.GetComponent<AI>().aiTurn = true;
            Enemy.GetComponent<AI>().turns = 2;
            playerEnded = false;
        }
        if(aiEnded)
        {
            Player.GetComponent<UnitMove>().playerTurn = true;
            Player.GetComponent<UnitMove>().turns = 2;
            aiEnded = false;
        }
    }

    //private void EndTurn()
    //{
    //    currentTurn = (currentTurn + 1) % 2; 
    //    UpdateTurn();
    //}


    //private void UpdateTurn()
    //{
    //    if (currentTurn == 0)
    //    {
    //        Player.GetComponent<UnitMove>().enabled = true;
    //        Enemy.GetComponent<UnitMove>().enabled = false;
    //    }
    //    else
    //    { 
    //        Player.GetComponent<UnitMove>().enabled = false; 
    //        Enemy.GetComponent<UnitMove>().enabled = true; 
    //    }

    //    Debug.Log(currentTurn == 0 ? "Player's Turn" : "Enemy's Turn");
    //}
}
