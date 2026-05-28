using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;

[System.Serializable]
public class PreguntaDB
{
    public int id_pregunta;
    public int numero_pregunta;
    public string categoria;
    public string pregunta;
    public string opcion_a;
    public string opcion_b;
    public string opcion_c;
    public string opcion_d;
    public int respuesta_correcta;
    public string explicacion;
}

[System.Serializable]
public class ListaPreguntasDB
{
    public PreguntaDB[] preguntas;
}

// Clase Pregunta original — se mantiene para compatibilidad
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

    [Header("Configuración BD")]
    public string urlAPI = "http://localhost:8000/preguntas";
    public bool usarBD = true; // Si es false, usa preguntas hardcodeadas

    [Header("Preguntas hardcodeadas (fallback)")]
    public List<Pregunta> preguntas;

    // Preguntas cargadas desde BD
    private List<Pregunta> preguntasCargadas = new List<Pregunta>();

    private int indicePreguntaActual = 0;
    private List<int> indicesBarajados = new List<int>();
    private bool respondido = false;
    private bool cargando = false;

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

        if (usarBD)
            StartCoroutine(CargarPreguntasDesdeDB());
        else
            InicializarConPreguntas(preguntas);
    }

    IEnumerator CargarPreguntasDesdeDB()
    {
        cargando = true;
        textoPregunta.text = "Cargando preguntas...";

        using (UnityWebRequest request = UnityWebRequest.Get(urlAPI))
        {
            request.timeout = 10;
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string json = request.downloadHandler.text;
                // Unity no deserializa arrays directamente, usamos wrapper
                string jsonWrapped = "{\"preguntas\":" + json + "}";
                ListaPreguntasDB lista = JsonUtility.FromJson<ListaPreguntasDB>(jsonWrapped);

                preguntasCargadas.Clear();
                foreach (var p in lista.preguntas)
                {
                    Pregunta pregunta = new Pregunta
                    {
                        pregunta         = p.pregunta,
                        opciones         = new string[] { p.opcion_a, p.opcion_b, p.opcion_c, p.opcion_d },
                        respuestaCorrecta = p.respuesta_correcta,
                        explicacion      = p.explicacion
                    };
                    preguntasCargadas.Add(pregunta);
                }

                Debug.Log($"{preguntasCargadas.Count} preguntas cargadas desde BD");
                InicializarConPreguntas(preguntasCargadas);
            }
            else
            {
                Debug.LogWarning($"Error al cargar BD: {request.error}. Usando preguntas locales.");
                InicializarConPreguntas(preguntas); // Fallback
            }
        }

        cargando = false;
    }

    void InicializarConPreguntas(List<Pregunta> lista)
    {
        preguntasCargadas = lista;
        BarajarPreguntas();
    }

    void BarajarPreguntas()
    {
        indicesBarajados.Clear();
        int total = preguntasCargadas.Count;
        for (int i = 0; i < total; i++)
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

        if (cargando)
            textoPregunta.text = "Cargando preguntas...";
        else
            MostrarPregunta();
    }

    public void CerrarGenerador()
    {
        panelGenerador.SetActive(false);
        SistemaCamaras.instancia.AbrirMenuPC();
    }

    void MostrarPregunta()
    {
        if (preguntasCargadas.Count == 0)
        {
            textoPregunta.text = "No hay preguntas disponibles.";
            return;
        }

        if (indicePreguntaActual >= indicesBarajados.Count)
            BarajarPreguntas();

        respondido = false;
        btnSiguiente.gameObject.SetActive(false);

        Pregunta p = preguntasCargadas[indicesBarajados[indicePreguntaActual]];
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

        Pregunta p = preguntasCargadas[indicesBarajados[indicePreguntaActual]];

        foreach (var btn in botonesOpciones)
            btn.interactable = false;

        if (opcionElegida == p.respuestaCorrecta)
        {
            MusicController.instancia?.PlayRight();
            SistemaMonedas.instancia?.RegistrarPreguntaCorrecta();
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