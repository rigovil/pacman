using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Pacdot : MonoBehaviour
{
    //Detecta si algo "choca" con el collider del objeto
    //aquí se debe aumentar el contador de puntaje o restar cuantos pellets hay en juego
    void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.name == "pacman")
        {
            PacmanMove pacman = GameObject.Find("pacman").GetComponent<PacmanMove>();
            float[] dot = new float[] { transform.localPosition.x, transform.localPosition.y };
            int index = pacman.pacdotsList.FindIndex(x => x.SequenceEqual(dot));
            pacman.RemovePacdot(index);
            Destroy(gameObject);
        }
        else if (collision.name == "pacmanIA")
        {
            PacmanIA pacman = GameObject.Find("pacmanIA").GetComponent<PacmanIA>();
            float[] dot = new float[] { transform.localPosition.x, transform.localPosition.y };
            int index = pacman.pacdotsList.FindIndex(x => x.SequenceEqual(dot));
            pacman.RemovePacdot(index);
            Destroy(gameObject);
        }
               
        
    }
}
