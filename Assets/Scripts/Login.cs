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

    IEnumerator ieLogin()
    {
        WWWForm dataForm = new WWWForm();
        dataForm.AddField("Username", inputUsername.text);
        dataForm.AddField("Password", inputPassword.text);

        string uri = "http://ec2-52-91-89-222.compute-1.amazonaws.com/login/login.php";

        UnityWebRequest webRequest = UnityWebRequest.Post(uri, dataForm);
        webRequest.chunkedTransfer = false;

        yield return webRequest.SendWebRequest();


        string result = webRequest.downloadHandler.text;


        if ((result != "0")&&(result != "Connection Error!"))
        {
            textLoginResult.text = "Sign in successfully !";
            PlayerPrefs.SetString("user_name", result);
            SceneManager.LoadScene(escena);
        }

        else if( result == "0")
        {
            textLoginResult.text = "You have to register !";
        }


        else
        {
            textLoginResult.text = "Check your connection !";
        }



    }

    public void login()
    {
        StartCoroutine(ieLogin());
        // if ((inputUsername.text == "admin" && inputPassword.text == "admin")||(inputUsername.text == "usuario" && inputPassword.text == "usuario")){
        //     textLoginResult.text = "Login correcto";
        //     SceneManager.LoadScene(escena);
        // }
        // else {
        //     textLoginResult.text = "Login incorrecto";
        // }
    }
}
