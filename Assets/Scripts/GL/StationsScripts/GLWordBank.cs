using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GLWordBank : MonoBehaviour
{
    private List<string> originalPrompts = new List<string>() {
      "Actua como un", "Se paciente y ", "No cometas errores"
    };
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
