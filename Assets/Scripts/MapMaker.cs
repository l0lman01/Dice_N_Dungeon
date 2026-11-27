using UnityEngine;
using System.Collections.Generic;

public class MapMaker : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public List<GameObject> mapModules = new();

    private List<List<int>> mapMatrice = new();

    public int spacing = 1;
    public int length = 2;
    public int width = 1;

    public int[] startPos = new int[2];
    public int[] endPos = new int[2];
        
    void Start()
    {
        if (startPos.Length < 2)
        {
            startPos = new int[2] { Random.Range(0, width), 0 };
        }
        if (endPos.Length < 2)
        {
            endPos = new int[2] { Random.Range(0, width), length - 1 };
        }

        //Generate keyrooms
        List<int[]> keyRooms = new();
        print("Nb of tiles: " + width * length);
        for(int i = 0; i < (width * length) / 5; i++)
        {
            int x = Random.Range(0, width);
            int y = Random.Range(0, length);
            int[] coord = { x, y };
            while ((x == startPos[0] && y == startPos[1]) || (x == endPos[0] && y == endPos[1]) || keyRooms.Contains(coord))
            {
                coord[0] = Random.Range(0, width); 
                coord[1] = Random.Range(0, length);
            }
            keyRooms.Add(coord);
            //print("x = " + x + " | y = "+y);
        }

        for (int matriceX = 0; matriceX < length; matriceX++)
        {
            List<int> alist = new();

            for(int matriceY = 0; matriceY < width; matriceY++)
            {
                if(startPos[0] == matriceX && startPos[1] == matriceY)
                {
                    alist.Add(1);
                }
                else if (endPos[0] == matriceX && endPos[1] == matriceY)
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

    void MapCreation()
    {
        int cordX = 0;
        int cordY = 0;
        print(mapMatrice.Count);
        print(mapMatrice[0].Count);
        print("----Collums NB " + mapMatrice.Count);
        foreach (List<int> collum in mapMatrice)
        {
            print("----Row NB "+ collum.Count);
            foreach (int modValue in collum)
            {
                if (modValue != 255)
                {
                    print("coord x:" + cordX + "|y:" + cordY + " - value:" + modValue);
                }

                switch (modValue)
                {
                    //Start
                    case 1:
                        Instantiate(mapModules[0], new Vector3(cordX * spacing, 0, cordY * spacing), transform.rotation);
                        break;
                    //End
                    case 2:
                        Instantiate(mapModules[1], new Vector3(cordX * spacing, 0, cordY * spacing), transform.rotation);
                        break;
                    //Keyroom
                    case 3:
                        Instantiate(mapModules[2], new Vector3(cordX * spacing, 0, cordY * spacing), transform.rotation);
                        break;
                    //Room
                    case 4:
                        Instantiate(mapModules[3], new Vector3(cordX * spacing, 0, cordY * spacing), transform.rotation);
                        break;
                        //Void
                    case 255:
                        Instantiate(mapModules[4], new Vector3(cordX * spacing, 0, cordY * spacing), transform.rotation);
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
