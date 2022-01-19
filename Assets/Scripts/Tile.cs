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
    public bool extracted = false;
    public bool scanRevealed = false;
    public int row = -1;
    public int col = -1;
    public ResourceCount resourceType = ResourceCount.None;

    void OnMouseDown()
    {
        if (!GameManager.instance.readyToGo)
            return;

        if (GameManager.instance.mode == Mode.Scan)
        {
            if (GameManager.instance.scanUses > 0)
            {
                GameManager.instance.scanUses--;
                GameManager.instance.RefreshUI();
            }
            else return;

            for (int r = -1; r <= 1; r++)
            {
                for (int c = -1; c <= 1; c++)
                {
                    if (r+row < 0 || r+row > GameManager.instance.gridDimensions.y - 1 || c+col < 0 || c+col > GameManager.instance.gridDimensions.x - 1)
                        continue;
                    
                      GameManager.instance.grid[r + row, c + col].GetComponent<Tile>().scanRevealed = true;
                }
            }
            GameManager.instance.onResourcesDetermined.Invoke();
        }
        else
        {
            GameManager.instance.ExtractResourceAt(row, col);
        }

    }
    void Awake()
    {
        GameManager.instance.onResourcesDetermined.AddListener(OnResourcesChanged);    
        GameManager.instance.onExtractMode.AddListener(OnExtractMode);    
        GameManager.instance.onScanMode.AddListener(OnScanMode);    
        GameManager.instance.onRevealAllTiles.AddListener(OnRevealTile);    
    }
    public void OnRevealTile()
    {
        scanRevealed = true;
        OnResourcesChanged();
    }

    public void OnResourcesChanged()
    {
        //Debug.Log("This should be getting called!!");
        
        if (GameManager.instance.mode == Mode.Scan)
        {
            if (!scanRevealed)
            {
                gameObject.GetComponent<MeshRenderer>().material = GameManager.instance.nonRevealedMaterial;
                gameObject.GetComponent<MeshRenderer>().material.color = GameManager.instance.noResourcesColor;
                return;
            }
            else
            {
                gameObject.GetComponent<MeshRenderer>().material = GameManager.instance.scanMaterial;

            }


            switch (resourceType)
            {
                case ResourceCount.Full:
                    gameObject.GetComponent<MeshRenderer>().material.color = GameManager.instance.fullResourcesColor;
                    break;
                case ResourceCount.Half:
                    gameObject.GetComponent<MeshRenderer>().material.color = GameManager.instance.halfResourcesColor;
                    break;
                case ResourceCount.Quarter:
                    gameObject.GetComponent<MeshRenderer>().material.color = GameManager.instance.quarterResourcesColor;
                    break;
                case ResourceCount.Minimal:
                    gameObject.GetComponent<MeshRenderer>().material.color = GameManager.instance.minimalResourcesColor;
                    break;
                case ResourceCount.None:
                    gameObject.GetComponent<MeshRenderer>().material.color = GameManager.instance.noResourcesColor;
                    break;
            }
        }
        else
        {
            if (!scanRevealed)
            {
                gameObject.GetComponent<MeshRenderer>().material = GameManager.instance.extractMaterial;
                gameObject.GetComponent<MeshRenderer>().material.color = GameManager.instance.noResourcesColor;
                return;
            }
            else
            {
                gameObject.GetComponent<MeshRenderer>().material = GameManager.instance.scanMaterial;

            }


            switch (resourceType)
            {
                case ResourceCount.Full:
                    gameObject.GetComponent<MeshRenderer>().material.color = GameManager.instance.fullResourcesColor;
                    break;
                case ResourceCount.Half:
                    gameObject.GetComponent<MeshRenderer>().material.color = GameManager.instance.halfResourcesColor;
                    break;
                case ResourceCount.Quarter:
                    gameObject.GetComponent<MeshRenderer>().material.color = GameManager.instance.quarterResourcesColor;
                    break;
                case ResourceCount.Minimal:
                    gameObject.GetComponent<MeshRenderer>().material.color = GameManager.instance.minimalResourcesColor;
                    break;
                case ResourceCount.None:
                    gameObject.GetComponent<MeshRenderer>().material.color = GameManager.instance.noResourcesColor;
                    break;
            }
            //if (extracted)
            //    gameObject.GetComponent<MeshRenderer>().material = GameManager.instance.extractNoOreMaterial;
            //else
            //    gameObject.GetComponent<MeshRenderer>().material = GameManager.instance.extractMaterial;
        }
    }
    public void OnScanMode()
    {
        OnResourcesChanged();
    }
    public void OnExtractMode()
    {
        gameObject.GetComponent<MeshRenderer>().material = GameManager.instance.extractMaterial;
    }
}
