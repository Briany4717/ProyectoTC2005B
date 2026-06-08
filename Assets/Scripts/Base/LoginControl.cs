using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngineInternal;
using System.Collections;
using WhirlpoolPromptWeb.Models;
using Newtonsoft.Json;
using UnityEngine.SocialPlatforms;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class LoginControl : MonoBehaviour
{

    [SerializeField] private TMP_InputField emailInput;
    [SerializeField] private TMP_InputField passwordInput;
    [SerializeField] private TextMeshProUGUI errorText;


    public void OnLoginButtonClicked()
    {
        string email = emailInput.text;
        string password = passwordInput.text;

        AuthenticateUser(email, password);

    }

    private void AuthenticateUser(string _email, string _password)
    {
        var payload = new
        {
            email = _email,
            password = _password
        };

        string json = JsonConvert.SerializeObject(payload);

        ApiManager.Instance.Post("authenticate",
        json,
        onSuccess: (jsonResponse) =>
        {
            if (string.IsNullOrWhiteSpace(jsonResponse) || jsonResponse == "[]")
            {
                errorText.text = "Inicio de sesión fallido";
                return;
            }

            List<UserSession> sessions = JsonConvert.DeserializeObject<List<UserSession>>(jsonResponse);

            if (sessions == null || sessions.Count == 0)
            {
                errorText.text = "No se encontró el usuario";
                return;
            }

            UserSession userSession = sessions[0];

            PlayerPrefs.SetInt("id_usuario", userSession.id_usuario);
            PlayerPrefs.SetString("nombre_usuario", userSession.nombre);
            PlayerPrefs.SetInt("saldo_total", userSession.saldo_total);

            SceneManager.LoadScene("MenuScene");
        },
        onError: (error) =>
        {
            Debug.LogError("Error de autentificacion: " + error);
        }
        );
    }



}
