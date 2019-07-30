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
    public GameObject lvl2;
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

            if(_gameState == GameState.Menu)
            {
                gameField = new GameObject("Game_Field").transform;
                CreateGrid();
            }
            else if(_gameState == GameState.Playing)
            {
                InputManager.dir = Vector2.zero;
            }else if(_gameState == GameState.GameOver)
            {
                GameObject g = Instantiate(cubeParticles, playerScript.transform.localPosition, playerScript.transform.localRotation);
                Destroy(g, 1.0f);
                Invoke("ShowGameOVer", 0.5f);
            }
            else if(_gameState == GameState.LevelUp)
            {
                playerScript.MoveToNextLevel();
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
        if(instance == null)
        {
            instance = this;
        }else if(instance != this)
        {
            DestroyImmediate(gameObject);
        }
        gameState = GameState.Menu;
    }

    private void Start()
    {
        playerScript = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
        Square startTile = GetBlock(0, 4);
        playerScript.mSquare = startTile;
        startTile.SetPixel(2);
        Vector3 playerStartPos = startTile.transform.localPosition;
        playerScript.transform.localPosition = startTile.transform.localPosition;
    }
    public void Fill()
    {
        if(border.Count > 0)
        {
            Square fillPoint = GetFloodPoint(border);
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

        return gridSquares[row * maxCols + col];
    }
    Square GetFloodPoint(List<Square> nodes)
    {
        int matches = 0;
        Square point = null;
        for(int i=nodes.Count -1; i >= 0; i--)
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
       // if(lastPoint.col == 0 || lastPoint.col == maxCols-1)
       // {
            if (lastPoint.row < midRow && lastPoint.col < midCol)                          // Bottom Left Corner
            {
                Square corner = GetBlock(lastPoint.row - 1, lastPoint.col);
                if (corner)
                {
                    if (corner._color == 0)
                        return corner;
                    else
                    {
                    Square s = GetBlock(lastPoint.row, lastPoint.col - 1);
                    if (s && s._color == 0)
                        return s;
                   // if (lastPoint.col < midCol)
                     //       return GetBlock(lastPoint.row, lastPoint.col - 1);
                       // else
                         //   return GetBlock(lastPoint.row, lastPoint.col - 1);
                    }
                }
                   
            }
            else if(lastPoint.row > midRow && lastPoint.col > midCol)                               // Top Right Corner 
            {
                Square corner = GetBlock(lastPoint.row + 1, lastPoint.col);
                if (corner)
                {
                    if (corner._color == 0)
                        return corner;
                    else
                    {
                    Square s = GetBlock(lastPoint.row, lastPoint.col + 1);
                    if (s && s._color == 0)
                        return s;
                   // if (lastPoint.col > midCol)
                   //         return GetBlock(lastPoint.row, lastPoint.col + 1);
                   //     else
                   //         return GetBlock(lastPoint.row, lastPoint.col - 1);
                    }
                }
            }
       // else if(lastPoint.row > midRow && lastPoint.col < midCol)
       // {
            else if(lastPoint.row > midRow && lastPoint.col < midCol)                                  // Top Left Corner
            {
                Square corner = GetBlock(lastPoint.row , lastPoint.col - 1);
                if (corner)
                {
                    if (corner._color == 0)
                        return corner;
                    else
                    {
                    Square s = GetBlock(lastPoint.row + 1, lastPoint.col);
                    if (s && s._color == 0)
                        return s;
                   // if (lastPoint.row < midRow)
                   //         return GetBlock(lastPoint.row + 1, lastPoint.col);
                   //     else
                   //         return GetBlock(lastPoint.row - 1, lastPoint.col);
                    }
                }
            }
            else                                                                                         // Bottom Right Corner
            {
                Square corner = GetBlock(lastPoint.row, lastPoint.col + 1);
                if (corner)
                {
                    if (corner._color == 0)
                        return corner;
                    else
                    {
                    Square s = GetBlock(lastPoint.row - 1, lastPoint.col);
                    if (s && s._color == 0)
                        return s;
                   // if (lastPoint.row > midRow)
                   //         return GetBlock(lastPoint.row - 1, lastPoint.col);
                   //     else
                   //         return GetBlock(lastPoint.row - 1, lastPoint.col);
                    }
                }
            }
       // }*/
        return point;
    }
    private IEnumerator ISLvelCleared()
    {
        if(gameState == GameState.Playing)
        {
            for (int row = 0; row < maxRows; row++)
            {
                for (int col = 0; col < maxCols; col++)
                {
                    if (GetBlock(row, col)._color != 2)
                    {
                        yield break;
                    }
                }
            }
            yield return new WaitForEndOfFrame();
            gameState = GameState.LevelUp;
        }
    }
    public void ChangeGridPos()
    {
          for (int row = 0; row < maxRows; row++)
          {
              for (int col = 0; col < maxCols; col++)
              {
                //  GetBlock(row, col).transform.localPosition += Vector3.forward * 15;
                  GetBlock(row, col).SetPixel(0);
              }
          }
        gameField.position += Vector3.forward * 19;
    }
    private void CreateGrid()
    {
        float offsetX = (-maxCols / 2f) * blockWidth + blockWidth / 2f;
        float offsetZ = -(maxRows / 2f) * blockHeight + blockHeight / 2f;
        gridSquares = new Square[maxRows*maxCols];
        Vector3 spawnPos = new Vector3(offsetX, 0.0f, offsetZ);
        for (int row = 0; row < maxRows; row++)
        {
            for (int col = 0; col < maxCols; col++)
            {
                Square b = Instantiate(cubePrefab, gameField);
                b.transform.position = spawnPos;
                b.transform.SetParent(gameField);

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
}
public enum GameState
{
    Menu,
    PrepareGame,
    Playing,
    GameOver,
    LevelUp
}
