using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SameTable : MonoBehaviour
{
    public List<GameObject> gameObjects;
    private int gameObjectCounter;
    // Start is called before the first frame update
    void Start()
    {
        gameObjectCounter = 0;
        for(int i=1; i<gameObjects.Count; i++)
        {
            gameObjects[i].SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void nextGame()
    {
        for (int i = 0; i < gameObjects.Count; i++)
        {
            gameObjects[i].SetActive(false);
        }
        gameObjectCounter++;
        if(gameObjectCounter < gameObjects.Count)
        {
            gameObjects[gameObjectCounter].SetActive(true);
        }
    }
}
