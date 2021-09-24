using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public class PacmanIA : MonoBehaviour
{
    int mapa = 1;           // 1 para primer mapa, 2 para cualquier otro
    int filas = 29;         // 29 para mapas grandes -- 18 para mapa pequeño
    int columnas = 26;      // 26 para mapas grandes -- 12 para mapa pequeño
    public Matrix matriz;
    public float speed = 0.3f;
    Vector2 dest = Vector2.zero;
    public List<float[]> pacdotsList=null;
    public int pacdotsInGame;
    Vector2 posClyde;
    Vector2 posBlinky;
    Vector2 posInky;
    Vector2 posPinky;
    public int shortestDistance;
    public float[] closestPacdot;
    public int temp;
    public float closestX;
    public float closestY;
    private string path;
    NeuralNetwork network;
    public float[] input;
    public float[] output;
    public int mayor;
    public float mayorVal;
    Vector2 lastPos = Vector2.zero;
    public int pacdot_restantes;
    public Dictionary<string, int> caminos;

    // Awake se llama cuando se instancia un objeto 
    private void Awake()
    {
        path = @".\Assets\Files\Weights.txt";
        pacdotsInGame = 0;
        shortestDistance = 0;
        temp = 0;
        pacdotsList = new List<float[]>();
        network = gameObject.AddComponent<NeuralNetwork>() as NeuralNetwork;
        network.Constructor(new int[] { 19, 10, 4 });
        network.Load(path);
        matriz = gameObject.AddComponent<Matrix>() as Matrix;
        matriz.Constructor(filas, columnas, mapa);

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
    // Para f�sicas siempre es mejor FixedUpdate para que sea m�s fluido
    void FixedUpdate()
    {
        mayorVal = -100.0f;
        temp = 0;
        shortestDistance = 0;
        closestX = 0.0f;
        closestY = 0.0f;
        mayor = 0;
        float[] validsTemp = new float[4];

        pacdot_restantes =pacdotsList.Count;
        if(pacdot_restantes==0){   
            UnityEditor.EditorApplication.isPlaying = false;
            //Application.Quit();
        }
        // Se mueve hacia el destino
        Vector2 p = Vector2.MoveTowards(transform.localPosition, dest, 0.3f);
        GetComponent<Rigidbody2D>().MovePosition(p);

        // revisa el input si no est� en movimiento
        if ((Vector2)transform.localPosition == dest)
        {
            //Redondea los valores en (x,y)
            transform.localPosition = new Vector2((float)Math.Round(transform.localPosition.x), (float)Math.Round(transform.localPosition.y));

            //Cantidad de pasos al pacdot mas cercano
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

            validsTemp[0] = Convert.ToSingle(valid(Vector2.up));
            validsTemp[1] = Convert.ToSingle(valid(Vector2.right));
            validsTemp[2] = Convert.ToSingle(valid(-Vector2.up));
            validsTemp[3] = Convert.ToSingle(valid(-Vector2.right));

            if (!(validsTemp[0] == 0 && validsTemp[1] == 0 && validsTemp[2] == 0 && validsTemp[3] == 0))
            {
                posInky = GameObject.Find("inky").transform.position;
                posClyde = GameObject.Find("clyde").transform.position;
                posPinky = GameObject.Find("pinky").transform.position;
                posBlinky = GameObject.Find("blinky").transform.position;

                input = new float[] { (closestPacdot[0] - transform.localPosition.x), (closestPacdot[1] - transform.localPosition.y), shortestDistance,
                        (float)Math.Round(posPinky[0] - transform.localPosition.x), (float)Math.Round(posPinky[1] - transform.localPosition.y),
                        (float)stepsToPacdot(new int[] { Convert.ToInt32(transform.localPosition.y) - 2, columnas + 1 - Convert.ToInt32(transform.localPosition.x) },
                                      new int[] { Convert.ToInt32(Math.Round(posPinky[1])) - 2, columnas + 1 - Convert.ToInt32(Math.Round(posPinky[0])) }),
                        (float)Math.Round(posBlinky[0] - transform.localPosition.x), (float)Math.Round(posBlinky[1] - transform.localPosition.y),
                        (float)stepsToPacdot(new int[] { Convert.ToInt32(transform.localPosition.y) - 2, columnas + 1 - Convert.ToInt32(transform.localPosition.x) },
                                      new int[] { Convert.ToInt32(Math.Round(posBlinky[1])) - 2, columnas + 1 - Convert.ToInt32(Math.Round(posBlinky[0])) }),
                        (float)Math.Round(posInky[0] - transform.localPosition.x), (float)Math.Round(posInky[1] - transform.localPosition.y),
                        (float)stepsToPacdot(new int[] { Convert.ToInt32(transform.localPosition.y) - 2, columnas + 1 - Convert.ToInt32(transform.localPosition.x) },
                                      new int[] { Convert.ToInt32(Math.Round(posInky[1])) - 2, columnas + 1 - Convert.ToInt32(Math.Round(posInky[0])) }),
                        (float)Math.Round(posClyde[0] - transform.localPosition.x), (float)Math.Round(posClyde[1] - transform.localPosition.y),
                        (float)stepsToPacdot(new int[] { Convert.ToInt32(transform.localPosition.y) - 2, columnas + 1 - Convert.ToInt32(transform.localPosition.x) },
                                      new int[] { Convert.ToInt32(Math.Round(posClyde[1])) - 2, columnas + 1 - Convert.ToInt32(Math.Round(posClyde[0])) }),
                        Convert.ToSingle(valid(Vector2.up)), Convert.ToSingle(valid(Vector2.right)),
                        Convert.ToSingle(valid(-Vector2.up)), Convert.ToSingle(valid(-Vector2.right)) };

                output = network.FeedForward(input);

                //Si un movimiento no es valido, cambiar el output a 0 para esa direccion de movimiento
                ///*
                for (int i = 0; i < validsTemp.Length; ++i)
                {
                    if (validsTemp[i] == 0)
                        output[i] = -99.0f;
                }

                for (int i = 0; i < 4; i++)
                {
                    //Toma en cuenta un movimiento si no es para ir en la direccion en la que venia
                    //if (mayorVal < output[i] && nextPos(i) != lastPos)
                    if (mayorVal < output[i])
                    {
                        mayorVal = output[i];
                        mayor = i;
                    }
                }

                switch (mayor)
                {
                    case 0:
                        dest = (Vector2)transform.localPosition + Vector2.up;
                        break;
                    case 1:
                        dest = (Vector2)transform.localPosition + Vector2.right;
                        break;
                    case 2:
                        dest = (Vector2)transform.localPosition - Vector2.up;
                        break;
                    case 3:
                        dest = (Vector2)transform.localPosition - Vector2.right;
                        break;
                    //case 4:
                    //    dest = (Vector2)transform.localPosition;
                    //    break;
                    default:
                        break;

                }
                //Guarda la posicion actual como ultima posicion, antes de moverse     
                lastPos = (Vector2)transform.position;
            }
        }

        //Actualizar los parametros de las animaciones 
        Vector2 dir = dest - (Vector2)transform.localPosition;
        GetComponent<Animator>().SetFloat("DirX", dir.x);
        GetComponent<Animator>().SetFloat("DirY", dir.y);
    }

    private Vector2 nextPos(int dir)
    {
        switch (dir)
        {
            case 0:
                return (Vector2)transform.localPosition + Vector2.up;
            case 1:
                return (Vector2)transform.localPosition + Vector2.right;
            case 2:
                return (Vector2)transform.localPosition - Vector2.up;
            case 3:
                return (Vector2)transform.localPosition - Vector2.right;
            default:
                return Vector2.zero;
        }
    }

    private bool valid(Vector2 dir)
    {
        Vector2 pos = (Vector2)transform.localPosition;
        
        RaycastHit2D hit = Physics2D.Linecast(pos + dir, pos);
        return hit.collider == GetComponent<Collider2D>();
    }

    public int stepsToPacdot(int[] posInicio, int[] posFinal)
    {
        int pasos = -1;
        string key = posInicio[0] + "," + posInicio[1] + "," + posFinal[0] + "," + posFinal[1];
        string key2 = posFinal[0] + "," + posFinal[1] + "," + posInicio[0] + "," + posInicio[1];

        if (caminos.ContainsKey(key))
            pasos = caminos[key];
        else if (caminos.ContainsKey(key2))
            pasos = caminos[key2];

        return pasos;
    }

    private bool isTherePacdot(float posX, float posY)
    {
        Vector2 pos = new Vector2(posX, posY);
        RaycastHit2D hit = Physics2D.Raycast(pos, Vector2.up, 0);
        return (hit && hit.collider.name == "pacdot");
    }

    public void RemovePacdot(int index)
    {
        if (index != -1)
            pacdotsList.RemoveAt(index);
    }
}
