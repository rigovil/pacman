using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public class GhostMove : MonoBehaviour
{
    //string path = @".\Assets\Files\Waypoints.txt";
    //string path2 = @".\Assets\Files\MatrizWaypoints.txt";
    int[] direccionesX = new int[4] { -1, 0, 1, 0 };
    int[] direccionesY = new int[4] { 0, 1, 0, -1 };
    public int[,] matrizWaypoints;
    public int[,] waypointNumbers;
    public Transform[] waypoints;
    public int lastCur;
    public int cur;
    public int waypointX;
    public int waypointY;
    public float speed = 0.2f;

    void Awake()
    {
        matrizWaypoints = new int[29, 26];
        waypointNumbers = new int[29, 26];
        int waypoint = 0;

        for (float y = 30.0f; y >= 2.0f; --y)
        {
            for (float x = 2.0f; x <= 27.0f; ++x)
            {
                int xInt = -1 * ((Convert.ToInt32(y) - 2) - 28);
                int yInt = Convert.ToInt32(x) - 2;
                matrizWaypoints[xInt, yInt] = 0;
                waypointNumbers[xInt, yInt] = -1;

                if (isThereWaypoint(x, y, waypoint))
                {
                    matrizWaypoints[xInt, yInt] = 1;
                    waypointNumbers[xInt, yInt] = waypoint++;
                }
                else if (isTherePacdot(x, y))
                {
                    matrizWaypoints[xInt, yInt] = 2;
                }
            }
        }

        matrizWaypoints[16, 11] = 2;
        matrizWaypoints[16, 12] = 2;
        matrizWaypoints[16, 13] = 2;
    }

    void Start()
    {
        cur = 32;
        lastCur = 0;
        waypointX = 10;
        waypointY = 13;
    }

    void FixedUpdate ()
    {
        // Se sigue moviendo en una direccion hasta encontrar un waypoint
        if (transform.position != waypoints[cur].position)
        {
            Vector2 p = Vector2.MoveTowards(transform.position, waypoints[cur].position, 0.15f);
            GetComponent<Rigidbody2D>().MovePosition(p);
        }
        // Encontro un waypoint, entonces se mueve hacia el siguiente
        else
            cur = nextWaypoint();

        // Animacion
        Vector2 dir = waypoints[cur].position - transform.position;
        GetComponent<Animator>().SetFloat("DirX", dir.x);
        GetComponent<Animator>().SetFloat("DirY", dir.y);
    }

    int nextWaypoint()
    {
        int nextDir = UnityEngine.Random.Range(0, 4);
        while (!valid(nextDir) || isGoingBack(nextDir))
            nextDir = UnityEngine.Random.Range(0, 4);

        return waypointNumbers[waypointX, waypointY];
    }

    bool valid(int nextDir)
    {
        int offsetX = waypointX + direccionesX[nextDir];
        int offsetY = waypointY + direccionesY[nextDir];

        bool outside = (offsetX < 0 || offsetX >= 29) || (offsetY < 0 || offsetY >= 26);
        bool isWall = !outside && (matrizWaypoints[offsetX, offsetY] == 0);

        return (!isWall && !outside);
    }

    bool isGoingBack(int nextDir)
    {
        int nextX = waypointX, nextY = waypointY;
        do
        {
            nextX += direccionesX[nextDir];
            nextY += direccionesY[nextDir];
        } while (matrizWaypoints[nextX, nextY] == 2);

        bool isLastCur = waypointNumbers[nextX, nextY] == lastCur;
        if(!isLastCur)
        {
            lastCur = cur;
            waypointX = nextX;
            waypointY = nextY;
        }

        return isLastCur;
    }

    private bool isTherePacdot(float posX, float posY)
    {
        Vector2 pos = new Vector2(posX, posY);
        RaycastHit2D hit = Physics2D.Raycast(pos, Vector2.up, 0);
        return (hit && hit.collider.name == "pacdot");
    }

    private bool isThereWaypoint(float posX, float posY, int waypoint)
    {
        return (waypoints[waypoint].position.x == posX && waypoints[waypoint].position.y == posY);
    }

    void OnTriggerEnter2D(Collider2D co)
    {
        if(co.name=="pacman" ){
            Destroy(co.gameObject);
            UnityEditor.EditorApplication.isPlaying = false;
            //Application.Quit();

        }
        else if (co.name == "pacmanIA"){
            Destroy(co.gameObject);
            UnityEditor.EditorApplication.isPlaying = false;
            //Application.Quit();
        }
    }
}
