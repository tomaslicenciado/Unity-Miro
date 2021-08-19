using UnityEngine;

public class Line : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject lineRendererPrefab;
    private GameObject currentLineRenderer;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(currentLineRenderer == null)
        {
            currentLineRenderer = (GameObject) Instantiate(lineRendererPrefab);
        }
    }
}
