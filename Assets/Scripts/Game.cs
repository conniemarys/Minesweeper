using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Game : MonoBehaviour
{
    //=====Summary=====
    //Contains all the logic for the game, outsourcing the presentation of the tilemap to Board.cs and the properties of each tile to Cell.cs

    //Width and height of the board determined by user input, but default set to 16x16, the 'normal' difficulty rating. 
    public int width;
    public int height;
    //changes according to size of grid, see EasyMode(), NormalMode(), and HardMode()
    public int mineCount = 32;

    private Board board;

    //This 2D array gets sent to Board.cs as an ordered set of data for each cell in the tilemap.
    private Cell[,] state;
    private bool gameOver;
    [SerializeField]
    private UIManager uI;
    private bool helper;
    int flaggedMines;

    private void Awake()
    {
        board = GetComponentInChildren<Board>();
    }

    //Upon start, the Menu Screen should be visible.
    private void Start()
    {
        MenuScreen();
        uI.quitUI.SetActive(false);
        uI.winMessage.SetActive(false);
        uI.lostMessage.SetActive(false);
        uI.customMenu.SetActive(false);
    }

    private void MenuScreen()
    {
        uI.lostMessage.SetActive(false);
        uI.winMessage.SetActive(false);

        //this erases the board underneath completely so if the user downsizes the board,
        //it draws a new board from scratch, respecting the downsize
        state = new Cell[0, 0];
        board.Draw(state);
        uI.menuUI.SetActive(true);

        //The Menu Screen only hides the board underneath with an opaque background,
        //gameOver must be set to true so user input does not impact the board
        gameOver = true;

        uI.easyButton.onClick.AddListener(EasyMode);
        uI.normalButton.onClick.AddListener(NormalMode);
        uI.hardButton.onClick.AddListener(HardMode);
        uI.customButton.onClick.AddListener(customMode);
    }

    //EasyMode uses a grid of 9x9 with 5 mines.
    //These settings can be changed within this codeblock
    private void EasyMode()
    {
        width = 9;
        height = 9;
        mineCount = 5;

        uI.menuUI.SetActive(false);
        NewGame();
    }

    //EasyMode uses a grid of 16x16 with 32 mines.
    //These settings can be changed within this codeblock
    private void NormalMode()
    {
        width = 16;
        height = 16;
        mineCount = 32;

        uI.menuUI.SetActive(false);
        NewGame();
    }

    //EasyMode uses a grid of 20x20 with 50 mines.
    //These settings can be changed within this codeblock
    private void HardMode()
    {
        width = 20;
        height = 20;
        mineCount = 50;

        uI.menuUI.SetActive(false);
        NewGame();
    }

    private void customMode()
    {
        uI.menuUI.SetActive(false);
        uI.customMenu.SetActive(true);
        width = 2;
        height = 2;
        uI.mineInput.maxValue = (int)(width * height * 0.75);
        uI.mineInput.value = 1;
        uI.widthInput.value = width;
        uI.heightInput.value = height;

        uI.widthInput.onValueChanged.AddListener((w) =>
        {
            uI.widthInputText.text = w.ToString();
            width = (int)w;
            uI.mineInput.maxValue = (int)(width * height * 0.75);
        });

        uI.heightInput.onValueChanged.AddListener((h) =>
        {
            uI.heightInputText.text = h.ToString();
            height = (int)h;
            uI.mineInput.maxValue = (int)(width * height * 0.75);
        });

        uI.mineInput.onValueChanged.AddListener((m) =>
        {
            uI.mineInputText.text = m.ToString();
            mineCount = (int)m;
        });

        uI.customStartButton.onClick.AddListener(NewGame);
    }

    private void NewGame()
    {
        uI.customMenu.SetActive(false);
        uI.menuUI.SetActive(false);
        helper = false;
        uI.inGameHelper.SetActive(false);
        flaggedMines = 0;
        uI.inGameMinesFlagged.text = flaggedMines.ToString();
        uI.inGameMineCount.text = mineCount.ToString();

        Camera.main.transform.position = new Vector3(0, height / 2f, -10f);
        Camera cam = Camera.main.GetComponent<Camera>();
        if (height > width)
        {
            cam.orthographicSize = height * 0.65f;
        }
        else
        {
            cam.orthographicSize = width * 0.65f;
        }

        //makes the grid responsive to user input
        gameOver = false;

        //calls a new state each time NewGame is called, freshly creating the grid.
        state = new Cell[width, height];

        GenerateCells();
        GenerateMines();
        GenerateNumbers();

        board.Draw(state);
    }

    private void GenerateCells()
    {
        for(int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Cell cell = new Cell();
                cell.position = new Vector3Int(x, y, 0);
                cell.type = Cell.Type.Empty;
                state[x, y] = cell;
            }
        }
    }

    //The number of mines is preset by variable mineCount, but their position is randomly generated
    private void GenerateMines()
    {
        for (int i = 0; i < mineCount; i++)
        {
            int x = Random.Range(0, width);
            int y = Random.Range(0, height);

            //if statement ensures that a mine location isn't repeated
            if(state[x, y].type == Cell.Type.Mine)
            {
                i--;
                continue;
            }

            state[x, y].type = Cell.Type.Mine;
        }
    }

    //Method to determine which tiles are number tiles by check all non-mine tiles for surrounding mines.
    //Calls CountMines to determine their corresponding number
    private void GenerateNumbers()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Cell cell = state[x, y];
                if (cell.type == Cell.Type.Mine)
                {
                    continue;
                }

                cell.number = CountMines(x, y);

                if(cell.number > 0)
                {
                    cell.type = Cell.Type.Number;
                }

                state[x, y] = cell;
         
            }
        }
    }

    //Method counts the number of mines surrounding a given non-mine tile
    private int CountMines(int cellX, int cellY)
    {
        int count = 0;

        for (int adjacentx = -1; adjacentx <= 1; adjacentx++)
        {
            for (int adjacenty = -1; adjacenty <= 1; adjacenty++)
            {
                //does not include itself in the count
                if(adjacentx == 0 && adjacenty == 0)
                {
                    continue;
                }

                int x = cellX + adjacentx;
                int y = cellY + adjacenty;

                if(GetCell(x, y).type == Cell.Type.Mine)
                {
                    count++;
                }
            }
        }

        return count;
    }

    //Update very minimal, only checking for user input and calling relevant methods
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            MenuScreen();
        }

        //cannot interact with board if gameOver == false
        if (!gameOver)
        {
            if (Input.GetMouseButtonDown(1))
            {
                Flag();
            }

            if (Input.GetMouseButtonDown(0))
            {
                Reveal();
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            QuitMenu();
        }

        if (Input.GetKeyDown(KeyCode.H))
        {
            if(helper)
            {
                uI.inGameHelper.SetActive(false);
                helper = false;
            }
            else if(!helper)
            {
                uI.inGameHelper.SetActive(true);
                helper = true;
            }
        }
    }

    //flags a given cell, if that cell is not revealed
    private void Flag()
    {
        //determines which cell has been clicked
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int cellPosition = board.tilemap.WorldToCell(worldPosition);
        Cell cell = GetCell(cellPosition.x, cellPosition.y);
        flaggedMines = 0;

        if(cell.type == Cell.Type.Invalid || cell.revealed)
        {
            return;
        }

        cell.flagged = !cell.flagged;
        state[cellPosition.x, cellPosition.y] = cell;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                cell = state[x, y];

                if(cell.flagged && cell.type == Cell.Type.Mine)
                {
                    flaggedMines++;
                }
            }
        }

        uI.inGameMinesFlagged.text = flaggedMines.ToString();

        //redraws the board after each interaction
        board.Draw(state);

    }

    //reveals a cell if that cell is not flagged or already revealed.
    //also calls CheckWinCondition() after each interaction
    private void Reveal()
    {
        //determines which cell has been clicked
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int cellPosition = board.tilemap.WorldToCell(worldPosition);
        Cell cell = GetCell(cellPosition.x, cellPosition.y);

        if(cell.type == Cell.Type.Invalid || cell.revealed || cell.flagged)
        {
            return;
        }

        switch (cell.type)
        {
            case Cell.Type.Mine:
                Explode(cell);
                break;
            case Cell.Type.Empty:
                Flood(cell);
                CheckWinCondition();
                break;
            default:
                cell.revealed = true;
                state[cellPosition.x, cellPosition.y] = cell;
                CheckWinCondition();
                break;

        }
        //redraws the board after each interaction
        board.Draw(state);
    }

    //Recursion Flood method, called if an empty cell is revealed
    //also reveals all surrounding empty and number cells until it hits a mine
    private void Flood(Cell cell)
    {
        //exit conditions
        if (cell.revealed) return;
        if (cell.type == Cell.Type.Mine || cell.type == Cell.Type.Invalid) return;

        cell.revealed = true;
        state[cell.position.x, cell.position.y] = cell;

        //recursion statement
        if(cell.type == Cell.Type.Empty)
        {
            Flood(GetCell(cell.position.x - 1, cell.position.y));
            Flood(GetCell(cell.position.x + 1, cell.position.y));
            Flood(GetCell(cell.position.x, cell.position.y - 1));
            Flood(GetCell(cell.position.x, cell.position.y + 1));
        }
    }

    //if you hit a mine(mushroom), that mushroom will present red and all other mushrooms will be revealed
    private void Explode(Cell cell)
    {
        gameOver = true;

        uI.lostMessage.SetActive(true);

        cell.revealed = true;
        cell.exploded = true;
        state[cell.position.x, cell.position.y] = cell;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                cell = state[x, y];

                if (cell.type == Cell.Type.Mine)
                {
                    cell.revealed = true;
                    state[x, y] = cell;
                }
            }
        }
    }

    //called when converting world location to cell location
    private Cell GetCell(int x, int y)
    {
        if (IsValid(x, y))
        {
            return state[x, y];
        }
        else
        {   //creates invalid cell if out of bounds
            return new Cell();
        }
    }

    private bool IsValid(int x, int y)
    {
        return x >= 0 && x < width && y >= 0 && y < height;
    }

    //Conditions for winning
    // - if all Mines are not revealed but all others are
    private void CheckWinCondition()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Cell cell = state[x, y];

                //will only skip this if statement if it finds a cell that is not revealed AND not a mine.
                //if skipped, it enters the win condition
                if (cell.type != Cell.Type.Mine && !cell.revealed)
                {
                    return;
                }

            }
        }

        gameOver = true;

        uI.winMessage.SetActive(true);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Cell cell = state[x, y];

                if (cell.type == Cell.Type.Mine)
                {
                    //flags all the mines so the user can see where they were
                    cell.flagged = true;
                    state[x, y] = cell;
                }
            }
        }



    }

    private void QuitMenu()
    {
        gameOver = true;
        uI.quitUI.SetActive(true);
        uI.yesButton.onClick.AddListener(yesQuit);
        uI.noButton.onClick.AddListener(noQuit);

    }

    private void yesQuit()
    {
        Application.Quit();
    }

    private void noQuit()
    {
        gameOver = false;
        uI.quitUI.SetActive(false);
    }

}
