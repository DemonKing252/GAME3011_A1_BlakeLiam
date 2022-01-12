using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ResourceCount
{
    None,
    Minimal,
    Quarter,
    Half,
    Full
}


public class Tile : MonoBehaviour
{
    public int row = -1;
    public int col = -1;
    public ResourceCount resourceType = ResourceCount.Minimal;

    void OnMouseDown()
    {
        Debug.Log("Clicked on: " + gameObject.name);
        GameManager.instance.ExtractResourceAt(row, col);
        //gameObject.GetComponent<MeshRenderer>().material.color = Color.red;
    }
    void Awake()
    {
        GameManager.instance.onResourcesDetermined.AddListener(OnResourcesChanged);    
    }
    public void OnResourcesChanged()
    {
        Debug.Log("This should be getting called!!");


        //gameObject.GetComponent<MeshRenderer>().material.color = Color.green;
        switch (resourceType)
        {
            case ResourceCount.Full:
                gameObject.GetComponent<MeshRenderer>().material.color = new Color(0.0f, 1.0f, 0.0f);
                break;
            case ResourceCount.Half:
                gameObject.GetComponent<MeshRenderer>().material.color = new Color(0.0f, 0.5f, 0.0f);
                break;
            case ResourceCount.Quarter:
                gameObject.GetComponent<MeshRenderer>().material.color = new Color(0.0f, 0.25f, 0.0f);
                break;
            case ResourceCount.Minimal:
                gameObject.GetComponent<MeshRenderer>().material.color = new Color(0.0f, 0.125f, 0.0f);
                break;
            case ResourceCount.None:
                gameObject.GetComponent<MeshRenderer>().material.color = new Color(0.0f, 0.0f, 0.0f);
                break;
        }
    }
}
