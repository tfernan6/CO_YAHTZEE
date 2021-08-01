using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using edu.jhu.co;


    public class ScoreboardController : MonoBehaviour
    {

    // For testing purposes, will grab this from GameManager
    public Scorecard[] scorecards;
    public GameManager gameManager;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.Find("GameRoomObject").GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void populateScoreboard()
    {
        
    }
    }


