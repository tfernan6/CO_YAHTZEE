using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/* Testing changes to Git */
public class GameController : MonoBehaviour
{

    public Sprite[] diceImages = new Sprite[6];
    public GameObject[] diceObjects = new GameObject[5];
    

    //initialize dice
    private int[] dieValues = { 1, 2, 3, 4, 5};

    // Start is called before the first frame update
    void Start()
    {

        rollDice();
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i<diceObjects.Length; i++){
             diceObjects[i].GetComponent<Image>().sprite = diceImages[dieValues[i]-1];
        }
    }

    public void rollDice(){
        for (int i = 0; i<dieValues.Length; i++){
            dieValues[i] = Random.Range(1,7);
        }
    }

}
