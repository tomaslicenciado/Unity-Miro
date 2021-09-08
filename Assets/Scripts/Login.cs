using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Login : MonoBehaviour
{
    public InputField inputUsername;
    public InputField inputPassword;
    public Text textLoginResult;
    public string escena;


    public void login()
    {
        if ((inputUsername.text == "admin" && inputPassword.text == "admin")||(inputUsername.text == "usuario" && inputPassword.text == "usuario")){
            textLoginResult.text = "Login correcto";
            SceneManager.LoadScene(escena);
        }
        else {
            textLoginResult.text = "Login incorrecto";
        }
    }

}
