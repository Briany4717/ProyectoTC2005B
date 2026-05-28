using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

[System.Serializable]
public class Pregunta
{
    public string pregunta;
    public string[] opciones;
    public int respuestaCorrecta;
    public string explicacion;
}

public class SistemaPreguntas : MonoBehaviour
{
    [Header("UI Generador")]
    public GameObject panelGenerador;
    public TMP_Text textoPregunta;
    public Button[] botonesOpciones;
    public TMP_Text[] textosOpciones;
    public Button btnSiguiente;
    public Button btnCerrarGenerador;

    [Header("Sprites botones")]
    public Sprite spriteBotonNormal;
    public Sprite spriteBotonCorrecto;
    public Sprite spriteBotonIncorrecto;

    [Header("Preguntas")]
    public List<Pregunta> preguntas;

    private int indicePreguntaActual = 0;
    private List<int> indicesBarajados = new List<int>();
    private bool respondido = false;

    public static SistemaPreguntas instancia;

    void Awake() => instancia = this;

    void Start()
    {
        panelGenerador.SetActive(false);
        btnSiguiente.onClick.AddListener(SiguientePregunta);
        btnCerrarGenerador.onClick.AddListener(CerrarGenerador);

        for (int i = 0; i < botonesOpciones.Length; i++)
        {
            int index = i;
            botonesOpciones[i].onClick.AddListener(() => Responder(index));
        }

        BarajarPreguntas();
    }

    void BarajarPreguntas()
    {
        indicesBarajados.Clear();
        for (int i = 0; i < preguntas.Count; i++)
            indicesBarajados.Add(i);

        for (int i = indicesBarajados.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            int temp = indicesBarajados[i];
            indicesBarajados[i] = indicesBarajados[j];
            indicesBarajados[j] = temp;
        }

        indicePreguntaActual = 0;
    }

    public void AbrirGenerador()
    {
        panelGenerador.SetActive(true);
        Time.timeScale = 0f;
        MostrarPregunta();
    }

    public void CerrarGenerador()
    {
        panelGenerador.SetActive(false);
        SistemaCamaras.instancia.AbrirMenuPC();
    }

    void MostrarPregunta()
    {
        if (indicePreguntaActual >= indicesBarajados.Count)
            BarajarPreguntas();

        respondido = false;
        btnSiguiente.gameObject.SetActive(false);

        Pregunta p = preguntas[indicesBarajados[indicePreguntaActual]];
        textoPregunta.text = p.pregunta;

        for (int i = 0; i < botonesOpciones.Length; i++)
        {
            if (i < p.opciones.Length)
                textosOpciones[i].text = p.opciones[i];

            botonesOpciones[i].interactable = true;
            botonesOpciones[i].GetComponent<Image>().sprite = spriteBotonNormal;
        }
    }

    void Responder(int opcionElegida)
    {
        if (respondido) return;
        respondido = true;

        Pregunta p = preguntas[indicesBarajados[indicePreguntaActual]];

        foreach (var btn in botonesOpciones)
            btn.interactable = false;

        if (opcionElegida == p.respuestaCorrecta)
        {
            MusicController.instancia?.PlayRight();
            textoPregunta.text = $"<color=#1B8B00>¡Correcto!</color>\n{p.explicacion}";
            CambiarSpriteBoton(botonesOpciones[opcionElegida], spriteBotonCorrecto);
            SistemaRayos.instancia.GanarRayo();
        }
        else
        {
            MusicController.instancia?.PlayWrong();
            textoPregunta.text = $"<color=red>¡Incorrecto!</color>\n{p.explicacion}";
            CambiarSpriteBoton(botonesOpciones[opcionElegida], spriteBotonIncorrecto);
            CambiarSpriteBoton(botonesOpciones[p.respuestaCorrecta], spriteBotonCorrecto);
            SistemaRayos.instancia.PerderRayo();
        }

        btnSiguiente.gameObject.SetActive(true);
    }

    void CambiarSpriteBoton(Button btn, Sprite sprite)
    {
        btn.GetComponent<Image>().sprite = sprite;
    }

    void SiguientePregunta()
    {
        indicePreguntaActual++;
        MostrarPregunta();
    }
}