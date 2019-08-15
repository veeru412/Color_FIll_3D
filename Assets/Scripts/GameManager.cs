using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ColorFill.Core;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public Square cubePrefab;
    public int maxRows;
    public int maxCols;
    public readonly float blockWidth = 1.0f;
    public readonly float blockHeight = 1.0f;
    public Material[] tileMats;
    public List<Square> border = new List<Square>();
    public GameObject cubeParticles;
    public GameObject obstracleParticle;
    public int levelGap = 21;
    [Header("Menu")]
    public GameObject gameOverMenu;

    Square[] gridSquares;
    [HideInInspector]
    public Transform gameField;
    GameState _gameState;
    PlayerController playerScript;

    public GameState gameState
    {
        get
        {
            return _gameState;
        }
        set
        {
            _gameState = value;

            if (_gameState == GameState.Menu)
            {
                gameField = new GameObject("Game_Field").transform;
                levelInfo[currentLvl].lvl.transform.position = Vector3.forward * currentLvl * levelGap;
                levelInfo[currentLvl].lvl.SetActive(true);
                Vector3 v3 = Camera.main.transform.position;
                v3.z = currentLvl * levelGap;
                Camera.main.transform.position = v3;
                CreateGrid();
            }
            else if (_gameState == GameState.Playing)
            {
                InputManager.dir = Vector2.zero;
            }
            else if (_gameState == GameState.GameOver)
            {
                Destroy(playerScript.gameObject);
                GameObject g = Instantiate(cubeParticles, playerScript.transform.localPosition, Quaternion.identity);
                Destroy(g, 1.0f);
                Invoke("ShowGameOVer", 0.5f);
            }
            else if (_gameState == GameState.LevelUp)
            {
               // playerScript.MoveToNextLevel();
                StartCoroutine(ChangeGridPos());
            }
        }
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            gameState = GameState.LevelUp;
        }
    }
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            DestroyImmediate(gameObject);
        }
        gameState = GameState.Menu;
    }
    public void DestroyObstracle(Transform obs)
    {
        GameObject g = Instantiate(obstracleParticle, obs.localPosition, Quaternion.identity);
        Destroy(g, 1.0f);
    }
    private void Start()
    {
        playerScript = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
        Square startTile = GetBlock(0, 5);
        playerScript.mSquare = startTile;
        startTile.SetPixel(2);
        Vector3 playerStartPos = startTile.transform.localPosition;
        playerScript.transform.localPosition = startTile.transform.localPosition;
    }
    public void Fill(bool stirighLine = false)
    {
        if (border.Count > 0)
        {
            Square fillPoint = GetFloodPoint(border, stirighLine);
            if (fillPoint == null)
            {
                fillPoint = border[0];
            }
            // print(fillPoint.row + "  " + fillPoint.col);
            FloodFillAI.FloodFill(fillPoint, false);
            border = new List<Square>();
        }
        StartCoroutine(ISLvelCleared());
    }

    public Square GetBlock(int row, int col)
    {
        if (row >= maxRows || row < 0 || col >= maxCols || col < 0)
            return null;
        Square s = gridSquares[row * maxCols + col];
        if (s.squareType == SquareType.None)
            return null;
        return s;
    }
    Square GetFloodPoint(List<Square> nodes,bool strighLine = false)
    {
        int matches = 0;
        Square point = null;
        for (int i = nodes.Count - 1; i >= 0; i--)
        {
            Square s = nodes[i];

            Square rightNeighb = GetBlock(s.row, s.col + 1);
            if (rightNeighb != null && rightNeighb.GetMatches() >= 2)
                return rightNeighb;

            Square leftNeighb = GetBlock(s.row, s.col - 1);
            if (leftNeighb != null && leftNeighb.GetMatches() >= 2)
                return leftNeighb;

            Square frontNeighb = GetBlock(s.row - 1, s.col);
            if (frontNeighb != null && frontNeighb.GetMatches() >= 2)
                return frontNeighb;

            Square backNeighb = GetBlock(s.row + 1, s.col);
            if (backNeighb != null && backNeighb.GetMatches() >= 2)
                return backNeighb;
        }
        Square lastPoint = nodes[nodes.Count - 1];

        int midCol = maxCols / 2;
        int midRow = maxRows / 2;

        if(lastPoint.row == 0 || lastPoint.col == 0 || lastPoint.row == maxRows-1 || lastPoint.col == maxCols - 1)
        {
            if (lastPoint.row <= midRow && lastPoint.col <= midCol)                          // Bottom Left Corner
            {
                Square corner = GetBlock(lastPoint.row - 1, lastPoint.col);
                if (corner && corner._color == 0)
                {
                    return corner;
                }
                else if (lastPoint.col != 0)
                {
                    for (int i = nodes.Count - 1; i >= 0; i--)
                    {
                        Square s = GetBlock(nodes[i].row, nodes[i].col - 1);
                        if (s && s._color == 0)
                            return s;
                    }
                }
            }
            else if (lastPoint.row >= midRow && lastPoint.col >= midCol)                               // Top Right Corner 
            {
                Square corner = GetBlock(lastPoint.row + 1, lastPoint.col);
                if (corner && corner._color == 0)
                {
                    return corner;
                }
                else if (lastPoint.col < maxCols)
                {
                    for (int i = nodes.Count - 1; i >= 0; i--)
                    {
                        Square s = GetBlock(nodes[i].row, nodes[i].col + 1);
                        if (s && s._color == 0)
                            return s;
                    }
                }
            }

            else if (lastPoint.row >= midRow && lastPoint.col <= midCol)                                  // Top Left Corner
            {
                Square corner = GetBlock(lastPoint.row, lastPoint.col - 1);
                if (corner && corner._color == 0)
                {
                    return corner;
                }
                else if (lastPoint.col != 0)
                {
                    for (int i = nodes.Count - 1; i >= 0; i--)
                    {
                        Square s = GetBlock(nodes[i].row, nodes[i].col - 1);
                        if (s && s._color == 0)
                            return s;
                    }
                }
            }
            else                                                                                         // Bottom Right Corner
            {
                Square corner = GetBlock(lastPoint.row - 1, lastPoint.col);
                if (corner && corner._color == 0)
                {
                    return corner;
                }
                else if (lastPoint.col < maxCols)
                {
                    for (int i = nodes.Count - 1; i >= 0; i--)
                    {
                        Square s = GetBlock(nodes[i].row, nodes[i].col + 1);
                        if (s && s._color == 0)
                            return s;
                    }
                }
            }
        }     
        if(strighLine)
        {
            if (nodes.Count == 1 || lastPoint.col == nodes[nodes.Count - 2].col)
            {
                for(int col = lastPoint.col - 1; col > 0; col--)
                {
                    Square s = GetBlock(lastPoint.row, col);
                    if(s && s._color == 2)
                    {
                        return GetBlock(lastPoint.row, col + 1);
                    }
                }
                for (int col = lastPoint.col + 1; col < maxCols; col++)
                {
                    Square s = GetBlock(lastPoint.row, col);
                    if (s && s._color == 2)
                    {
                        return GetBlock(lastPoint.row, col - 1);
                    }
                }
            }
            if (nodes.Count == 1 || lastPoint.row == nodes[nodes.Count - 2].row)
            {
                for (int row = lastPoint.row - 1; row > 0; row--)
                {
                    Square s = GetBlock(row, lastPoint.col);
                    if(s && s._color == 2)
                    {
                        return GetBlock(row + 1, lastPoint.col);
                    }
                }
                for (int row = lastPoint.row + 1; row < maxRows; row++)
                {
                    Square s = GetBlock(row, lastPoint.col);
                    if (s && s._color == 2)
                    {
                        return GetBlock(row - 1, lastPoint.col);
                    }
                }
            }
        }
       
        return point;
    }
    private IEnumerator ISLvelCleared()
    {
        if (gameState == GameState.Playing)
        {
            for (int row = 0; row < maxRows; row++)
            {
                for (int col = 0; col < maxCols; col++)
                {
                    Square check = GetBlock(row, col);
                    if (check && check._color != 2)
                    {
                        yield break;
                    }
                }
            }
            yield return new WaitForEndOfFrame();
            gameState = GameState.LevelUp;
        }
    }
    public static int currentLvl = 0;
    IEnumerator ChangeGridPos()
    {
        Square[] oldSquares = gridSquares;

        if (currentLvl < levelInfo.Length - 1)
            currentLvl++;

        levelInfo[currentLvl].lvl.transform.position = Vector3.forward * currentLvl * levelGap;
        levelInfo[currentLvl].lvl.SetActive(true);
        yield return new WaitForEndOfFrame();
        CreateGrid();

        yield return new WaitForEndOfFrame();   
       
        Vector3 pPos = playerScript.transform.position;
        while(Mathf.Abs(playerScript.transform.position.x - 0) > 0.05f)
        {
            pPos.x = Mathf.Lerp(pPos.x, 0, Time.deltaTime * 5);
            playerScript.transform.position = pPos;
            yield return null;
        }
        pPos.x = 0;
        playerScript.transform.position = pPos;

        playerScript.mSquare = GetBlock(0, 5);
        Vector3 targetPos = playerScript.mSquare.transform.position;
        print(targetPos);
        float progress = 0.0f;
        while(progress <= 1)
        {
            progress += Time.deltaTime;
            playerScript.transform.position = Vector3.Lerp(playerScript.transform.position, playerScript.mSquare.transform.position, progress);
            Vector3 camPOs = new Vector3(0,22.0f, Mathf.Lerp(Camera.main.transform.position.z, currentLvl * levelGap, progress));
            Camera.main.transform.position = camPOs;
            yield return null;
        }

        yield return new WaitForEndOfFrame();
        GetBlock(0, 5).SetPixel(2);
        gameState = GameState.Playing;
        for (int i = 0; i < oldSquares.Length; i++)
        {
            Destroy(oldSquares[i].gameObject);
        }
        levelInfo[currentLvl-1].lvl.SetActive(false);
    }

    private void CreateGrid()
    {
        maxRows = levelInfo[currentLvl].rowCount;
        maxCols = levelInfo[currentLvl].colCount;
        
         float offsetZ = currentLvl * levelGap;

        float offsetX = (-maxCols / 2f) * blockWidth + blockWidth / 2f;
        offsetZ += -(maxRows / 2f) * blockHeight + blockHeight / 2f;
        gridSquares = new Square[maxRows * maxCols];
        Vector3 spawnPos = new Vector3(offsetX, 0.0f, offsetZ);
        for (int row = 0; row < maxRows; row++)
        {
            for (int col = 0; col < maxCols; col++)
            {
                Square b = Instantiate(cubePrefab, gameField);
                b.transform.position = spawnPos;

                spawnPos.x += blockWidth;

                b.row = row;
                b.col = col;
                gridSquares[row * maxCols + col] = b;
            }
            spawnPos.x = offsetX;
            spawnPos.z += blockHeight;
        }
    }
    void ShowGameOVer()
    {
        gameOverMenu.SetActive(true);
    }
    public void Restart()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }
    public Level[] levelInfo;
    [System.Serializable]
    public class Level
    {
        public int rowCount;
        public int colCount;
        public GameObject lvl;
    }
}
public enum GameState
{
    Menu,
    PrepareGame,
    Playing,
    GameOver,
    LevelUp
}
