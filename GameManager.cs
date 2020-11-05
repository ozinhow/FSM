using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Text tempoTxt;
    private float tempoInicial;

    public float showPosX;
    public float showPosZ;
    public int qtdBicho;



    public GameObject animal;

    private void Start()
    {
        tempoInicial = Time.time;
        InstanciarBicharada();
    }

    void Update()
    {
        Timer();
    }

    private void Timer()
    {
        if (qtdBicho > 0)
        {
            float tempo = Time.time - tempoInicial;

            string minutes = ((int)tempo / 60).ToString();
            string seconds = (tempo % 60).ToString("f2");

            tempoTxt.text = minutes + ":" + seconds;
        }
        else if (qtdBicho <= 0)
        {
            tempoTxt.text = tempoTxt.text;
        }

    }

    private void InstanciarBicharada()
    {
        int qtdAnimais = Random.Range(5, 10);

        qtdBicho = qtdAnimais;

        for (int i = 0; i < qtdAnimais; i++)
        {
            float posX = Random.Range(-10, 10);
            float posZ = Random.Range(-10, 10);

            Instantiate(animal, new Vector3(posX, 0.5f, posZ), Quaternion.identity);
        }

    }
}
