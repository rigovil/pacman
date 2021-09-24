using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using UnityEngine;

public class PacmanMove : MonoBehaviour
{
    int mapa = 1;           // 1 para primer mapa, 2 para cualquier otro
    int filas = 29;         // 29 para mapas grandes -- 18 para mapa pequeño
    int columnas = 26;      // 26 para mapas grandes -- 12 para mapa pequeño
    public Matrix matriz;
    public float speed = 0.3f;
    Vector2 dest = Vector2.zero;
    Vector2 posClyde;
    Vector2 posBlinky;
    Vector2 posInky;
    Vector2 posPinky;
    public List<float[]> pacdotsList;
    public int pacdotsInGame;
    public int shortestDistance;
    public float[] closestPacdot;
    public int temp;
    public float closestX;
    public float closestY;
    private string path;
    string info = "";
    string new_info = "";
    string last_new_info = "";
    public string info_temp;
    public int pasos = 0;
    public Dictionary<string, int> caminos;

    // Awake se llama cuando se instancia un objeto 
    private void Awake()
    {
        path = @".\Assets\Files\TrainingSet.txt";
        pacdotsInGame = 0;
        shortestDistance = 0;
        temp = 0;
        pacdotsList = new List<float[]>();
        matriz = gameObject.AddComponent<Matrix>() as Matrix;
        matriz.Constructor(filas, columnas,mapa);

        for (float y = 2.0f; y < 31.0f; ++y)
        {
            for (float x = 2.0f; x < 28.0f; ++x)
            {
                if (isTherePacdot(x, y))
                {
                    pacdotsList.Add(new float[] { x, y });
                    matriz.pacdots[Convert.ToInt32(y) - 2, columnas + 1 - Convert.ToInt32(x)] = 1;
                    matriz.visitados[Convert.ToInt32(y) - 2, columnas + 1 - Convert.ToInt32(x)] = false;
                }
            }
        }

        matriz.minSteps();
        caminos = matriz.steps;
        //closestPacdot = new float[] { 12, 14 };
    }
    // Start se llama al inicio del primer frame
    void Start()
    {
        dest = transform.localPosition;
    }

    // Update se llama cada frames, FixedUpdate se llama por tiempo en lugar de frames
    // Para físicas siempre es mejor FixedUpdate para que sea más fluido
    void FixedUpdate()
    {
        if (pacdotsList.Count == 0)
        {
            UnityEditor.EditorApplication.isPlaying = false;
            writeTrainingSet(info);
        }

        posInky = GameObject.Find("inky").transform.position;
        posClyde = GameObject.Find("clyde").transform.position;
        posPinky = GameObject.Find("pinky").transform.position;
        posBlinky = GameObject.Find("blinky").transform.position;

        temp = 0;
        shortestDistance = 0;
        closestX = 0.0f;
        closestY = 0.0f;

        // Se mueve hacia el destino
        Vector2 p = Vector2.MoveTowards(transform.localPosition, dest, 0.3f);
        GetComponent<Rigidbody2D>().MovePosition(p);

        // revisa el input si no está en movimiento
        if ((Vector2)transform.localPosition == dest)
        {
            foreach (var item in pacdotsList)
            {
                temp = stepsToPacdot(new int[] { Convert.ToInt32(transform.localPosition.y) - 2, columnas + 1 - Convert.ToInt32(transform.localPosition.x) },      // POS PACMAN
                                     new int[] { Convert.ToInt32(item[1]) - 2, columnas + 1 - Convert.ToInt32(item[0]) });                                         // POS PACDOT
                if (shortestDistance == 0 || shortestDistance > temp)
                {
                    shortestDistance = temp;
                    closestX = item[0];
                    closestY = item[1];
                    closestPacdot = item;
                }
            }

            //bool noMove = false;
            info_temp = (closestPacdot[0] - transform.localPosition.x) + " " + (closestPacdot[1] - transform.localPosition.y) + " " + shortestDistance + " " +
                        Math.Round(posPinky[0] - transform.localPosition.x) + " " + Math.Round(posPinky[1] - transform.localPosition.y) + " " +
                        stepsToPacdot(new int[] { Convert.ToInt32(transform.localPosition.y) - 2, columnas + 1 - Convert.ToInt32(transform.localPosition.x) },
                                      new int[] { Convert.ToInt32(Math.Round(posPinky[1])) - 2, columnas + 1 - Convert.ToInt32(Math.Round(posPinky[0])) }) + " " +
                        Math.Round(posBlinky[0] - transform.localPosition.x) + " " + Math.Round(posBlinky[1] - transform.localPosition.y) + " " +
                        stepsToPacdot(new int[] { Convert.ToInt32(transform.localPosition.y) - 2, columnas + 1 - Convert.ToInt32(transform.localPosition.x) },
                                      new int[] { Convert.ToInt32(Math.Round(posBlinky[1])) - 2, columnas + 1 - Convert.ToInt32(Math.Round(posBlinky[0])) }) + " " +
                        Math.Round(posInky[0] - transform.localPosition.x) + " " + Math.Round(posInky[1] - transform.localPosition.y) + " " +
                        stepsToPacdot(new int[] { Convert.ToInt32(transform.localPosition.y) - 2, columnas + 1 - Convert.ToInt32(transform.localPosition.x) },
                                      new int[] { Convert.ToInt32(Math.Round(posInky[1])) - 2, columnas + 1 - Convert.ToInt32(Math.Round(posInky[0])) }) + " " +
                        Math.Round(posClyde[0] - transform.localPosition.x) + " " + Math.Round(posClyde[1] - transform.localPosition.y) + " " +
                        stepsToPacdot(new int[] { Convert.ToInt32(transform.localPosition.y) - 2, columnas + 1 - Convert.ToInt32(transform.localPosition.x) },
                                      new int[] { Convert.ToInt32(Math.Round(posClyde[1])) - 2, columnas + 1 - Convert.ToInt32(Math.Round(posClyde[0])) }) + " " +
                        Convert.ToSingle(valid(Vector2.up)) + " " + Convert.ToSingle(valid(Vector2.right)) + " " +
                        Convert.ToSingle(valid(-Vector2.up)) + " " + Convert.ToSingle(valid(-Vector2.right));

            if (Input.GetKey(KeyCode.UpArrow) && valid(Vector2.up))
            {
                dest = (Vector2)transform.localPosition + Vector2.up;
                if(pasos != -1)
                    info += info_temp + " 1 0 0 0\n";
            }
            else if (Input.GetKey(KeyCode.RightArrow) && valid(Vector2.right))
            {
                dest = (Vector2)transform.localPosition + Vector2.right;
                if (pasos != -1)
                    info += info_temp + " 0 1 0 0\n";
            }
            else if (Input.GetKey(KeyCode.DownArrow) && valid(-Vector2.up))
            {
                dest = (Vector2)transform.localPosition - Vector2.up;
                if (pasos != -1)
                    info += info_temp + " 0 0 1 0\n";
            }
            else if (Input.GetKey(KeyCode.LeftArrow) && valid(-Vector2.right))
            {
                dest = (Vector2)transform.localPosition - Vector2.right;
                if (pasos != -1)
                    info += info_temp + " 0 0 0 1\n";
            }
            //else
            //{
            //    dest = (Vector2)transform.localPosition;
            //    new_info = info_temp + " 0 0 0 0 1\n";
            //    noMove = true;
            //}

            //if(noMove)
            //{
            //    if(new_info != last_new_info)
            //    {
            //        info += new_info;
            //        last_new_info = new_info;
            //    }
            //}
            //else
            //{
            //    info += new_info;
            //    last_new_info = new_info;
            //}
        }

        //Actualizar los parametros de las animaciones 
        Vector2 dir = dest - (Vector2)transform.localPosition;
        GetComponent<Animator>().SetFloat("DirX", dir.x);
        GetComponent<Animator>().SetFloat("DirY", dir.y);
    }

    private bool valid(Vector2 dir)
    {
        Vector2 pos = transform.localPosition;
        RaycastHit2D hit = Physics2D.Linecast(pos + dir, pos);
        return (hit.collider == GetComponent<Collider2D>());
    }

    private bool isTherePacdot(float posX, float posY) {
        Vector2 pos = new Vector2(posX, posY);
        RaycastHit2D hit = Physics2D.Raycast(pos, Vector2.up, 0);
        return (hit && hit.collider.name == "pacdot");
    }

    public int stepsToPacdot(int[] posInicio, int[] posFinal)
    {
        string key = posInicio[0] + "," + posInicio[1] + "," + posFinal[0] + "," + posFinal[1];
        string key2 = posFinal[0] + "," + posFinal[1] + "," + posInicio[0] + "," + posInicio[1];

        if (caminos.ContainsKey(key))
            pasos = caminos[key];
        else if (caminos.ContainsKey(key2))
            pasos = caminos[key2];
        else
            pasos = -1;

        return pasos;
    }

    public void RemovePacdot(int index)
    {
        if (index != -1)
            pacdotsList.RemoveAt(index);
    }

    private void writeTrainingSet(string info) {
        if (!File.Exists(path))
        {
            using (StreamWriter sw = File.CreateText(path))
            {
                sw.Write(info);
            }
        }

        using (StreamWriter sw = File.AppendText(path))
        {
            sw.Write(info);
        }
    }
}