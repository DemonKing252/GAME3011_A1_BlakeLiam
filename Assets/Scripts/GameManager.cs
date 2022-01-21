using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;
using UnityEngine.SceneManagement;

public enum Mode
{
    Extract,
    Scan

}


public class GameManager : MonoBehaviour
{
    public bool readyToGo = false;
    [SerializeField] TMP_Text buttonText;
    [SerializeField] TMP_Text modeText;

    public Material scanMaterial;
    public Material extractMaterial;
    public Material extractNoOreMaterial;
    public Material nonRevealedMaterial;
    public Mode mode = Mode.Extract;

    
    public TMP_Text miningUsesText;
    public TMP_Text scanUsesText;
    public TMP_Text scoreText;
    public TMP_Text gameOverText;

    [HideInInspector] public int score = 0;
    [HideInInspector] public int scanUses = 0;
    [HideInInspector] public int miningUses = 0;

    [SerializeField] private int startingMiningUses = 10; 
    [SerializeField] private int startingScanUses = 3;

    public Color fullResourcesColor;
    public Color halfResourcesColor;
    public Color quarterResourcesColor;
    public Color minimalResourcesColor;
    public Color noResourcesColor;

    public static GameManager instance = null;
    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    [SerializeField]
    private GameObject tilePrefab;

    [System.NonSerialized]
    public Vector2Int gridDimensions = new Vector2Int(32, 32);

    [SerializeField]
    private int maxResources;

    [SerializeField]
    private int minResources;

    [SerializeField]
    public TMP_Text gameStat;

    // Delegates for commonly used actions
    public UnityEvent onResourcesDetermined = new UnityEvent();
    public UnityEvent onScanMode = new UnityEvent();
    public UnityEvent onExtractMode = new UnityEvent();
    public UnityEvent onRevealAllTiles = new UnityEvent();

    public GameObject[,] grid = new GameObject[32, 32];

    [SerializeField] private GameObject startCanvas;
    [SerializeField] private GameObject gameCanvas;
    [SerializeField] private GameObject replayCanvas;


    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 0f;

        gameStat.text = "";

        miningUses = startingMiningUses;
        scanUses = startingScanUses;
        RefreshUI();

        for(int row = 0; row < gridDimensions.x; row++)
        {
            for (int col = 0; col < gridDimensions.y; col++)
            {
                GameObject go = Instantiate(tilePrefab, new Vector3((float)row - (0.5f * gridDimensions.x), 0f, (float)col - (0.5f * gridDimensions.y)), Quaternion.identity);
                go.GetComponent<Tile>().row = row;
                go.GetComponent<Tile>().col = col;
                grid[row, col] = go;
                grid[row, col].name = string.Format("Tile ({0})({1})", row, col);
            }
        }
        int numResources = Random.Range(minResources, maxResources + 1);
        for (int i = 0; i < numResources; i++)
        {
            int randomColumn = Random.Range(0, gridDimensions.x);
            int randomRow = Random.Range(0, gridDimensions.y);


            try 
            {
                for(int r = -2; r <= 2; r++)
                {
                    for(int c = -2; c <= 2; c++)
                    {
                        if (r + randomRow >= 0 && r + randomRow < gridDimensions.y && c + randomColumn >= 0 && c + randomColumn < gridDimensions.x)
                            if (grid[r + randomRow, c + randomColumn].GetComponent<Tile>().resourceType != ResourceCount.Half &&
                                grid[r + randomRow, c + randomColumn].GetComponent<Tile>().resourceType != ResourceCount.Full)

                                grid[r + randomRow, c + randomColumn].GetComponent<Tile>().resourceType = ResourceCount.Quarter;
                    }
                }
                for (int r = -1; r <= 1; r++)
                {
                    for (int c = -1; c <= 1; c++)
                    {
                        if (r + randomRow >= 0 && r + randomRow < gridDimensions.y && c + randomColumn >= 0 && c + randomColumn < gridDimensions.x)
                            if (grid[r + randomRow, c + randomColumn].GetComponent<Tile>().resourceType != ResourceCount.Full)
                                grid[r + randomRow, c + randomColumn].GetComponent<Tile>().resourceType = ResourceCount.Half;
                    }
                }

                grid[randomRow, randomColumn].GetComponent<Tile>().resourceType = ResourceCount.Full;
            }
            catch (System.Exception e)
            {
                Debug.LogError("Exception: " + e.Message);
            }
        }

        onResourcesDetermined.Invoke();

    }
    public void OnStartGame()
    {
        readyToGo = true;
        Time.timeScale = 1f;
        startCanvas.SetActive(false);
        gameCanvas.SetActive(true);
    }
    public void OnPlayAgain()
    {
        // Reload the scene
        SceneManager.LoadScene("SampleScene");
    }
    public void OnQuit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
    Application.Quit();
#endif

    }

    public void OnGameOver()
    {
        gameOverText.text = "Game Over! All tiles have been revealed. Your score is: " + score.ToString();
        onRevealAllTiles.Invoke();
        readyToGo = false;
        gameCanvas.SetActive(false);
        replayCanvas.SetActive(true);
    }

    public void OnModeButtonClicked()
    {
        mode = (mode == Mode.Extract ? Mode.Scan : Mode.Extract);
        if (mode == Mode.Extract)
        {
            buttonText.text = "Scan mode";
            modeText.text = "Extract mode";
            onExtractMode.Invoke();
            onResourcesDetermined.Invoke();
        }
        else
        {
            buttonText.text = "Extract mode";
            modeText.text = "Scan mode";
            onScanMode.Invoke();
        }

    }

    public void RefreshUI()
    {
        scoreText.text = "Gold: " + score.ToString();
        miningUsesText.text = "Mining Uses: " + miningUses.ToString();
        scanUsesText.text = "Scan Uses: " + scanUses.ToString(); 
    }

    public void ExtractResourceAt(int row, int col)
    {
        miningUses--;

        if (miningUses <= 0)
            OnGameOver();


        try
        {
            switch (grid[row, col].GetComponent<Tile>().resourceType)
            {
                case ResourceCount.Full:

                    GameManager.instance.gameStat.text = string.Format("Extracted {0} gold at tile -> (x: {1}, y: {2})", 40, col, row);
                    score += 40;
                    break;
                case ResourceCount.Half:
                    GameManager.instance.gameStat.text = string.Format("Extracted {0} gold at tile -> (x: {1}, y: {2})", 20, col, row);
                    score += 20;
                    break;
                case ResourceCount.Quarter:
                    GameManager.instance.gameStat.text = string.Format("Extracted {0} gold at tile -> (x: {1}, y: {2})", 10, col, row);
                    score += 10;
                    break;
                case ResourceCount.Minimal:
                    GameManager.instance.gameStat.text = string.Format("Extracted {0} gold at tile -> (x: {1}, y: {2})", 5, col, row);
                    score += 5;
                    break;
                case ResourceCount.None:
                    GameManager.instance.gameStat.text = string.Format("Extracted {0} gold at tile -> (x: {1}, y: {2})", 0, col, row);
                    break;

            }

            for (int r = -2; r <= 2; r++)
            {
                for (int c = -2; c <= 2; c++)
                {
                    if (r + row >= 0 && r + row < gridDimensions.y && c + col >= 0 && c + col < gridDimensions.x)
            
                        switch(grid[r + row, c + col].GetComponent<Tile>().resourceType)
                        {
                            case ResourceCount.Full:
                                grid[r + row, c + col].GetComponent<Tile>().resourceType = ResourceCount.Half;
                                break;
            
                            case ResourceCount.Half:
                                grid[r + row, c + col].GetComponent<Tile>().resourceType = ResourceCount.Quarter;
                                break;
            
                            case ResourceCount.Quarter:
                                grid[r + row, c + col].GetComponent<Tile>().resourceType = ResourceCount.Minimal;
                                break;

                            case ResourceCount.Minimal:
                                grid[r + row, c + col].GetComponent<Tile>().resourceType = ResourceCount.None;
                                break;

                        } 
                }
            }

            

            RefreshUI();

            grid[row, col].GetComponent<Tile>().resourceType = ResourceCount.None;
            grid[row, col].GetComponent<Tile>().extracted = true;
            onResourcesDetermined.Invoke();
        }
        catch (System.Exception e)
        {
            Debug.LogError("Exception: " + e.Message);
        }
    }

}
