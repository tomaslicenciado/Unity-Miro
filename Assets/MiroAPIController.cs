using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System.Linq;
using System;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

namespace Miro.Apis
{
    public class MiroAPIController : MonoBehaviour
    {
        public GameObject cube;
        public TextMeshPro text;
        public GameObject line;

        public GameObject padre;
        public float escala;

        #region shapes

        public GameObject star;
        public GameObject circle;

        #endregion

        private readonly string url = "https://api.miro.com/v1/boards";
        //private readonly string token = "Bearer sP3JbsbAFxTUT4x2JzSkQ1wMqJY";
        private readonly string token = "Bearer jD-wagfQoacQgogtzG_d7aNZGiw";
        private readonly string idBoard = "o9J_l52Qrx0=";
        //private readonly string idBoard = "o9J_lbre-C8=";

        private List<GameObject> objetos = new List<GameObject>();

        // Start is called before the first frame update
        void Start()
        {
            StartCoroutine(GetBoardInfo(idBoard));
            StartCoroutine(GetWidgets(idBoard));
        }

        // Update is called once per frame
        void Update()
        {
            ActualizarLineas();
        }

        public IEnumerator GetWidgets(string idBoard)
        {
            Widgets response;
            #region llamadaAPIMiro

            var resource = string.Format(url + "/" + idBoard + "/widgets");
            UnityWebRequest miroRequest = UnityWebRequest.Get(resource);

            miroRequest.SetRequestHeader("Authorization", token);
            miroRequest.SetRequestHeader("Content-Type", "application/json");

            yield return miroRequest.SendWebRequest();

            #endregion
            if (miroRequest.isNetworkError || miroRequest.isHttpError)
            {
                Debug.LogError(miroRequest.error);
                yield break;
            }
            else
            {
                var jsonResponse = miroRequest.downloadHandler.text;
                response = JsonUtility.FromJson<Widgets>(jsonResponse);
                Color color;
                float x = 1, y = 1, z = 1;

                foreach (Datum widget in response.data.Where(x => x.type == "sticker").ToList())
                {
                    GameObject cubotemp = Instantiate(cube);

                    cubotemp.transform.SetParent(padre.transform);
                    cubotemp.name = widget.id;

                    ColorUtility.TryParseHtmlString(widget.style.backgroundColor, out color);
                    cubotemp.GetComponent<Renderer>().material.color = color;
                    
                    cubotemp.transform.localScale = new Vector3(widget.scale, widget.scale, widget.scale);

                    x = (widget.x / 150);
                    y = -(widget.y / 150);

                    cubotemp.transform.position = new Vector3(x, y, z);

                    objetos.Add(cubotemp);

                    #region addText
                    Text texto = cubotemp.GetComponentInChildren<Text>();
                    if (texto != null)
                    {
                        texto.text = widget.text.Replace("<p>", "").Replace("</p>", "");
                    }
                    #endregion
                }
                foreach (Datum shape in response.data.Where(x => x.type == "shape").ToList())
                {
                    var scale = shape.scale != 0 ? shape.scale : (float)(shape.width / shape.height * 0.5);
                    switch (shape.style.shapeType)
                    {
                        case "star":
                            GameObject shapeTemp = Instantiate(star);

                            shapeTemp.transform.SetParent(padre.transform);
                            shapeTemp.name = shape.id;

                            x = (shape.x / 150);
                            y = -(shape.y / 150);

                            shapeTemp.transform.localScale = new Vector3(scale, scale, scale);
                            shapeTemp.transform.position = new Vector3(x, y, z);

                            objetos.Add(shapeTemp);

                            break;

                        case "arrow":
                            break;
                        default:
                            break;
                    }
                }

                #region flechas

                foreach (Datum arrow in response.data.Where(x => x.type == "line" && !string.IsNullOrEmpty(x.endWidget.id)).ToList())
                {
                    ColorUtility.TryParseHtmlString(arrow.style.borderColor, out color);

                    GameObject lineTemp = Instantiate(line);
                    
                    lineTemp.transform.SetParent(padre.transform);
                    lineTemp.tag = "Linea";

                    lineTemp.GetComponent<LineRenderer>().positionCount = 2;

                    lineTemp.GetComponent<LineRenderer>().SetColors(color, color);

                    var initial = response.data.FirstOrDefault(x => x.id == arrow.startWidget.id);
                    var ending = response.data.FirstOrDefault(x => x.id == arrow.endWidget.id);

                    lineTemp.name = initial.id + " " + ending.id;

                    lineTemp.GetComponent<LineRenderer>().SetPosition(0, new Vector3(initial.x / 150, -(initial.y / 150), z));
                    lineTemp.GetComponent<LineRenderer>().SetPosition(1, new Vector3(ending.x / 150, -(ending.y / 150), z));

                    objetos.Add(lineTemp);
                }

                #endregion

                #region TextWidgets
                //foreach (Datum widget in response.data.Where(x => x.type == "text").ToList())
                //{
                //    TextMeshPro textTemp = Instantiate(text);
                //    ColorUtility.TryParseHtmlString(widget.style.backgroundColor, out color);
                //    textTemp.GetComponent<Renderer>().material.color = color;

                //    x = (widget.x / 150);
                //    y = Math.Abs(widget.y / 150);

                //    // textTemp.transform.localScale = new Vector3(widget.scale, widget.scale, widget.scale);
                //    textTemp.transform.position = new Vector3(x, y, z);
                //    textTemp.transform.Rotate(widget.rotation, 0, 0);

                //    textTemp.text = widget.text.Replace("<p>", "").Replace("</p>", "");
                //    textTemp.fontSize = widget.style.fontSize;

                //}
                #endregion

                Debug.Log(response);
            }

            padre.transform.localScale /= escala;
            ActualizarLineas();
        }

        public IEnumerator GetBoardInfo(string idBoard)
        {
            Board response;
            #region llamadaAPIMiro

            var resource = string.Format(url + "/" + idBoard);
            UnityWebRequest miroRequest = UnityWebRequest.Get(resource);

            miroRequest.SetRequestHeader("Authorization", token);
            miroRequest.SetRequestHeader("Content-Type", "application/json");

            yield return miroRequest.SendWebRequest();

            #endregion
            if (miroRequest.isNetworkError || miroRequest.isHttpError)
            {
                Debug.LogError(miroRequest.error);
                yield break;
            }
            else
            {
                var jsonResponse = miroRequest.downloadHandler.text;
                response = JsonUtility.FromJson<Board>(jsonResponse);
            }
        }

        public void ActualizarLineas(){
            foreach(GameObject obj in objetos){
                if (obj.CompareTag("Linea")){
                    string[] ids = obj.name.Split(' ');
                    GameObject first = GameObject.Find(ids[0]);
                    GameObject last = GameObject.Find(ids[1]);
                    obj.GetComponent<LineRenderer>().SetWidth(1/(escala * 10),1/(escala * 10));
                    obj.GetComponent<LineRenderer>().SetPosition(0, first.transform.position);
                    obj.GetComponent<LineRenderer>().SetPosition(1, last.transform.position);
                }
            }
        }
    }
}