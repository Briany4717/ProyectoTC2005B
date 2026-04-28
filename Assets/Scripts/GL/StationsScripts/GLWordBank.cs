using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GLWordBank : MonoBehaviour
{

    private List<string> originalPrompts = new List<string>() {
        "Actúa como un experto en el área de...",
        "Asume el rol de un consultor senior especializado en...",
        "Imagina que eres un desarrollador de software explicando a un cliente...",
        "Eres un analista de datos; quiero que interpretes...",
        "Toma la postura de un instructor corporativo enseñando...",
        "Redacta un correo electrónico formal y persuasivo dirigido a...",
        "Resume la siguiente información en puntos clave o viñetas:",
        "Escribe un informe detallado y estructurado sobre...",
        "Explica este concepto técnico como si se lo estuvieras enseñando a un principiante.",
        "Genera una lista de ideas innovadoras para resolver...",
        "Piensa paso a paso antes de llegar a la conclusión final.",
        "Desglosa tu razonamiento de forma lógica y secuencial.",
        "Antes de responder, si necesitas más contexto, hazme las preguntas necesarias.",
        "Analiza detalladamente los pros y contras de esta decisión:",
        "Proporciona una guía paso a paso, numerada y clara, para...",
        "Mantén un tono estrictamente profesional y objetivo en toda la respuesta.",
        "Utiliza un lenguaje claro, conciso y libre de jerga innecesaria.",
        "Responde de manera empática, comprensiva y orientada a dar soluciones.",
        "Sé directo y no añadas información de relleno; ve al grano.",
        "Asegúrate de incluir ejemplos prácticos o analogías en tu explicación.",
        "Devuelve la respuesta estructurada en una tabla con las columnas...",
        "Formatea el resultado utilizando únicamente Markdown.",
        "Presenta la salida exclusivamente en un formato JSON válido.",
        "Agrupa tus recomendaciones en categorías claras con sus respectivos encabezados.",
        "Resalta en negrita los términos más importantes de tu explicación.",
        "Revisa el siguiente documento y corrige cualquier error de redacción.",
        "Evalúa este código e identifica posibles vulnerabilidades o áreas de mejora.",
        "No asumas información externa; básate únicamente en el contexto proporcionado.",
        "Proporciona tres alternativas diferentes con distintos enfoques para...",
        "Actúa como un crítico constructivo y dame retroalimentación específica sobre..."
    };

    //     private List<string> originalPrompts = new List<string>()
    // {
    //     "Tienes que escribir esto letra por letra",
    //     "Tienes que escribir esto letra por letra",
    //     "Tienes que escribir esto letra por letra",
    //     "Tienes que escribir esto letra por letra",
    //     "Tienes que escribir esto letra por letra",
    //     "Tienes que escribir esto letra por letra",
    //     "Tienes que escribir esto letra por letra",
    //     "Tienes que escribir esto letra por letra",
    //     "Tienes que escribir esto letra por letra",
    //     "Tienes que escribir esto letra por letra",
    //     "Tienes que escribir esto letra por letra",
    //     "Tienes que escribir esto letra por letra",
    //     "Tienes que escribir esto letra por letra",
    //     "Tienes que escribir esto letra por letra",
    //     "Tienes que escribir esto letra por letra",
    //     "Tienes que escribir esto letra por letra",
    //     "Tienes que escribir esto letra por letra",
    //     "Tienes que escribir esto letra por letra",
    //     "Tienes que escribir esto letra por letra",
    //     "Tienes que escribir esto letra por letra",
    //     "Tienes que escribir esto letra por letra",
    //     "Tienes que escribir esto letra por letra"
    // };
    private List<string> workingPrompts = new List<string>();

    private void Awake()
    {
        workingPrompts.AddRange(originalPrompts);
        Shuffle(workingPrompts);
    }

    private void Shuffle(List<string> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int random = Random.Range(i, list.Count);

            // subtitute with the random int position
            string temporary = list[i];
            list[i] = list[random];
            list[random] = temporary;
        }
    }

    public string GetPrompt()
    {
        string newWord = string.Empty;

        if (workingPrompts.Count != 0)
        {
            newWord = workingPrompts.Last();
            workingPrompts.Remove(newWord);
        }

        return newWord;
    }
}
