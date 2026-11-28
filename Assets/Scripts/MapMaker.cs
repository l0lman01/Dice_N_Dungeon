using UnityEngine;
using System.Collections.Generic;

public class MapMaker : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public GameObject mapStart;
    public int startPosX = -1;
    public int startPosY = -1;
    public GameObject mapEnd;
    public int endPosX = -1;
    public int endPosY = -1;

    public List<GameObject> mapModules = new();
    public List<GameObject> mapBackgroundModules = new();

    private List<List<int>> mapMatrice = new();

    public int spacing = 1;
    public int length = 2;
    public int width = 1;
        
    void Start()
    {
        InsertCheck();
        //Generate keyrooms
        List<int[]> keyRooms = new();
        for(int i = 0; i < (width * length) / 5; i++)
        {
            int x = Random.Range(0, width);
            int y = Random.Range(0, length);
            int[] coord = { x, y };
            while ((x == startPosX && y == startPosY) || (x == endPosX && y == endPosY) || keyRooms.Contains(coord))
            {
                coord[0] = Random.Range(0, width - 1); 
                coord[1] = Random.Range(0, length - 1);
            }
            keyRooms.Add(coord);
            //print("x = " + x + " | y = "+y);
        }

        for (int matriceX = 0; matriceX < length; matriceX++)
        {
            List<int> alist = new();

            for(int matriceY = 0; matriceY < width; matriceY++)
            {
                print(""+matriceX+" - "+startPosX+ "|" + matriceY + " - " + startPosY);
                if (startPosX == matriceX && startPosY == matriceY)
                {
                    alist.Add(1);
                    print("start was been added");
                }
                else if (endPosX == matriceX && endPosY == matriceY)
                {
                    alist.Add(2);
                }
                else
                {
                    //TO BE OPTIMISED
                    bool foundKeyRoom = false;
                    foreach (int[] pos in keyRooms)
                    {
                        if (pos[0] == matriceX && pos[1] == matriceY)
                        {
                            alist.Add(3);
                            foundKeyRoom = true;
                        }
                    }
                    if (foundKeyRoom == false)
                    {
                        alist.Add(255);
                    }
                }
            }
            mapMatrice.Add(alist);
        }

        MapCreation();
    }

    void InsertCheck()
    {
        if (startPosX < 0)
        {
            startPosX = Random.Range(0, width - 1);
        }
        if (startPosY < 0)
        {
            startPosY = 0;
        }
        if (endPosX < 0)
        {
            endPosX = Random.Range(0, width - 1);
        }
        if (endPosY < 0)
        {
            endPosY = length - 1;
        }
    }
    void MapCreation()
    {
        int cordX = 0;
        int cordY = 0;
        //print(mapMatrice.Count);
        //print(mapMatrice[0].Count);
        //print("----Collums NB " + mapMatrice.Count);
        foreach (List<int> collum in mapMatrice)
        {
            //print("----Row NB "+ collum.Count);
            foreach (int modValue in collum)
            {
                if (modValue != 255)
                {
                    //print("coord x:" + cordX + "|y:" + cordY + " - value:" + modValue);
                }

                switch (modValue)
                {
                    //Start
                    case 1:
                        Instantiate(mapStart, new Vector3(cordX * spacing, 0, cordY * spacing), transform.rotation);
                        print("Start Added");
                        break;
                    //End
                    case 2:
                        Instantiate(mapEnd, new Vector3(cordX * spacing, 0, cordY * spacing), transform.rotation);
                        break;
                    //Keyroom
                    case 3:
                        Instantiate(mapModules[Random.Range(0, mapModules.Count)], new Vector3(cordX * spacing, 0, cordY * spacing), transform.rotation);
                        break;
                    //Room
                    case 4:
                        Instantiate(mapModules[Random.Range(0, mapModules.Count)], new Vector3(cordX * spacing, 0, cordY * spacing), transform.rotation);
                        break;
                    //Void
                    case 255:
                        if (mapBackgroundModules.Count > 0)
                        {
                            Instantiate(mapBackgroundModules[Random.Range(0, mapBackgroundModules.Count)], new Vector3(cordX * spacing, 0, cordY * spacing), transform.rotation);
                        }
                        else
                        {
                            Instantiate(mapModules[4], new Vector3(cordX * spacing, 0, cordY * spacing), transform.rotation);
                        }
                        break;
                    default:
                        break;
                }
                ++cordY;
            }
            ++cordX;
            cordY = 0;
        }
    }
}
