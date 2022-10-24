using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantiateBlooper : MonoBehaviour
{
    public GameObject blooper;
    public GameObject goomba;
    public int width = 10;
    public int height = 4;

    void Start()
    {
        for (int y=0; y<height; ++y)
        {
            for (int x=0; x<width; ++x)
            {
                Instantiate(blooper, new Vector3(x,y,0), Quaternion.identity);
            }
        }       
        Invoke("AddGoomba", 6);
    }

    public void AddGoomba(Vector3 position)
    {
        // Instantiate(goomba, position, Quaternion.identity);
        Instantiate(goomba, new Vector3(0,0,0), Quaternion.identity);
    }

    public void AddGoomba()
    {
        Instantiate(goomba, new Vector3(0,0,0), Quaternion.identity);
    }
}
