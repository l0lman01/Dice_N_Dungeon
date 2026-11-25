using UnityEngine;
using System.Collections.Generic;

public class MapMaker : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public List<GameObject> mapModules;
    private List<List<int>> mapMatrice;
    public int length = 2;
    public int width = 1;
    public int startPosX = 0;
    public int startPosY = 0;

    void Start()
    {
        //mapMatrice
        for(int i = 0; i < this.width * this.length / 5; i++)
        {

        }
        for(int matLength = 0; matLength < this.length; matLength++)
        {

            for(int matWidth = 0; matWidth < this.width; matWidth++)
            {

            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
