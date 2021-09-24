using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Posicion
{
    public int fila;
    public int columna;
    public int distancia;

    public Posicion(int x, int y, int z)
    {
        this.fila = x;
        this.columna = y;
        this.distancia = z;
    }
};

public class Matrix : MonoBehaviour
{
    int mapa;
    int filas;
    int columnas;
    string path = @".\Assets\Files\Matriz.txt";
    string path2 = @".\Assets\Files\Visitados.txt";
    string path3 = @".\Assets\Files\Caminos.txt";
    public bool[,] visitados;
    public int[,] pacdots;
    public Dictionary<string, int> steps;

    public void Constructor(int x, int y, int z)
    {
        mapa = z;
        filas = x;
        columnas = y;
        pacdots = new int[filas, columnas];
        visitados = new bool[filas, columnas];
        steps = new Dictionary<string, int>();

        for (int i = 0; i < filas; i++)
        {
            for (int j = 0; j < columnas; j++)
            {
                visitados[i, j] = true;
            }
        }

        // Para usar con el primer mapa
        if(mapa == 1)
        {
            pacdots[12, 12] = 1;
            pacdots[12, 13] = 1;
            pacdots[12, 14] = 1;
            visitados[12, 12] = false;
            visitados[12, 13] = false;
            visitados[12, 14] = false;
        }
    }

    public void minSteps()
    {
        for(int i = 0; i < filas; ++i)
        {
            for(int j = 0; j < columnas; ++j)
            {
                for(int k = 0; k < filas; ++k)
                {
                    for (int z = 0; z < columnas; ++z)
                    {
                        string key = i + "," + j + "," + k + "," + z;
                        string secondKey = k + "," + z + "," + i + "," + j;
                        if (pacdots[i, j] == 1 && pacdots[k, z] == 1 && (i == k && j == z) == false && steps.ContainsKey(secondKey) == false)
                            steps[key] = minDistance(new int[] { i, j }, new int[] { k, z });
                    }
                }
            }
        }
    }

    public int minDistance(int[] posInicio, int[] posFinal)
    {
        Posicion inicio = new Posicion(posInicio[0], posInicio[1], 0);
        inicio.fila = posInicio[0];
        inicio.columna = posInicio[1];
 
        List<Posicion> pila = new List<Posicion>();
        pila.Add(inicio);
        visitados[inicio.fila, inicio.columna] = true;

        while (pila.Count != 0) {
            Posicion pos = pila[0];
            pila.RemoveAt(0);

            // posFinal
            if (pos.fila == posFinal[0] && pos.columna == posFinal[1])
            {
                limpiarVisitados();
                return pos.distancia;
            }

            // arriba
            if (pos.fila - 1 >= 0 && visitados[pos.fila - 1, pos.columna] == false)
            {
                pila.Add(new Posicion(pos.fila - 1, pos.columna, pos.distancia + 1));
                visitados[pos.fila - 1, pos.columna] = true;
            }
 
            // abajo
            if (pos.fila + 1 < filas && visitados[pos.fila + 1, pos.columna] == false)
            {
                pila.Add(new Posicion(pos.fila + 1, pos.columna, pos.distancia + 1));
                visitados[pos.fila + 1, pos.columna] = true;
            }
 
            // izq
            if (pos.columna - 1 >= 0 && visitados[pos.fila, pos.columna - 1] == false)
            {
                pila.Add(new Posicion(pos.fila, pos.columna - 1, pos.distancia + 1));
                visitados[pos.fila, pos.columna - 1] = true;
            }
 
             // der
            if (pos.columna + 1 < columnas && visitados[pos.fila, pos.columna + 1] == false)
            {
                pila.Add(new Posicion(pos.fila, pos.columna + 1, pos.distancia + 1));
                visitados[pos.fila, pos.columna + 1] = true;
            }
        }

        limpiarVisitados();
        return -1;
    }

    public void limpiarVisitados()
    {
        for (int i = 0; i < filas; i++)
        {
            for (int j = 0; j < columnas; j++)
            {
                if (pacdots[i, j] == 0)
                    visitados[i, j] = true;
                else
                    visitados[i, j] = false;
            }
        }

        if(mapa == 1)
        {
            visitados[12, 12] = false;
            visitados[12, 13] = false;
            visitados[12, 14] = false;
        }       
    }

    public void imprimir()
    {
        string matriz = "";
        for(int x = 0; x < filas; ++x)
        {
            string fila = "";
            for(int y = 0; y < columnas; ++y)
            {
                fila += pacdots[x,y] + "\t";
            }
            matriz += fila + "\n";
        }

        using (StreamWriter sw = File.AppendText(path))
        {
            sw.WriteLine(matriz);
        }
    }

    public void imprimirVisitados()
    {
        string matriz = "";
        for (int x = 0; x < filas; ++x)
        {
            string fila = "";
            for (int y = 0; y < columnas; ++y)
            {
                if (visitados[x, y] == true)
                    fila += "T\t";
                else
                    fila += "F\t";
            }
            matriz += fila + "\n";
        }

        using (StreamWriter sw = File.AppendText(path2))
        {
            sw.WriteLine(matriz);
        }
    }
}