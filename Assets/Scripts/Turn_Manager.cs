using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class Turn_Manager : MonoBehaviour
{

    private int MovementPool;
    private int Coinflip_Result;
    public Grid_Manager.Occupancy CurrentTurn;
    private Grid_Manager GridManager;
    private UI_Manager UIManager;
    private int MineValue_Total;
    [SerializeField] private TextMeshProUGUI RemainingMoves_1;
    [SerializeField] private TextMeshProUGUI RemainingMoves_2;

    private int PlayedTurns = 0;
    private int RoundTurnCounter = 0;

    private bool IsGameOver = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GridManager = FindFirstObjectByType<Grid_Manager>();
        UIManager = FindFirstObjectByType<UI_Manager>();
        Coinflip_Function();
    }

    void Coinflip_Function()
    {
        Coinflip_Result = Random.Range(0, 2);
        if (Coinflip_Result == 0)
        {
            CurrentTurn = Grid_Manager.Occupancy.Blue;
            Blue_TurnStart();
        }

        if (Coinflip_Result == 1)
        {
            CurrentTurn = Grid_Manager.Occupancy.Red;
            Red_TurnStart();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Blue_TurnStart()
    {
        RoundTurnCounter++;

        MineValue_Total = 0;
        Debug.Log("It's Blue Player's Turn!");

        MovementPool = 5;
        CurrentTurn = Grid_Manager.Occupancy.Blue;

        RemainingMoves_1.text = CurrentTurn.ToString() + "\nRemaining Moves";
        RemainingMoves_2.text = MovementPool.ToString();

        // Calculate mine income (BLUE only)
        foreach (var cell in GridManager.Cells)
        {
            if (cell == null) continue;

            if (cell.Role == Grid_Manager.TileRoles.Mine &&
                cell.Occupancy == Grid_Manager.Occupancy.Blue)
            {
                MineValue_Total += cell.CellGameObjects
                    .GetComponent<Mine_Behaviour>()
                    .MineValue;
            }
        }

        Debug.Log("Blue's total mine value is " + MineValue_Total);

        BluePayday();

        // End-of-round mine growth (BOTH players)
        if (RoundTurnCounter >= 2)
        {
            RoundTurnCounter = 0;

            foreach (var cell in GridManager.Cells)
            {
                if (cell == null) continue;

                if (cell.Role == Grid_Manager.TileRoles.Mine &&
                    cell.Occupancy != Grid_Manager.Occupancy.Neutral)
                {
                    cell.CellGameObjects
                        .GetComponent<Mine_Behaviour>()
                        .TurnStarted_UpdateValue();
                }
            }
        }
    }

    public void Red_TurnStart()
    {
        RoundTurnCounter++;

        MineValue_Total = 0;
        Debug.Log("It's Red Player's Turn!");

        MovementPool = 5;
        CurrentTurn = Grid_Manager.Occupancy.Red;

        RemainingMoves_1.text = CurrentTurn.ToString() + "\nRemaining Moves";
        RemainingMoves_2.text = MovementPool.ToString();

        // Calculate mine income (RED only)
        foreach (var cell in GridManager.Cells)
        {
            if (cell == null) continue;

            if (cell.Role == Grid_Manager.TileRoles.Mine &&
                cell.Occupancy == Grid_Manager.Occupancy.Red)
            {
                MineValue_Total += cell.CellGameObjects
                    .GetComponent<Mine_Behaviour>()
                    .MineValue;
            }
        }

        Debug.Log("Red's total mine value is " + MineValue_Total);

        RedPayday();

        // End-of-round mine growth (BOTH players)
        if (RoundTurnCounter >= 2)
        {
            RoundTurnCounter = 0;

            foreach (var cell in GridManager.Cells)
            {
                if (cell == null) continue;

                if (cell.Role == Grid_Manager.TileRoles.Mine &&
                    cell.Occupancy != Grid_Manager.Occupancy.Neutral)
                {
                    cell.CellGameObjects
                        .GetComponent<Mine_Behaviour>()
                        .TurnStarted_UpdateValue();
                }
            }
        }
    }

    public bool UnitMovementCheck(Unit_Movement Script)
    {
        if (Script.Alignment != CurrentTurn)
            return false;

        if (MovementPool <= 0)
            return false;

        // Consume a move
        MovementPool--;
        RemainingMoves_2.text = MovementPool.ToString();

        // If that was the last move, end the turn AFTER this move
        if (MovementPool == 0)
        {
            if (CurrentTurn == Grid_Manager.Occupancy.Blue)
                Red_TurnStart();
            else
                Blue_TurnStart();
        }

        return true;
    }

    public void EndTurnButton_Function()
    {
        CheckGameOver();

        if(!IsGameOver)
        {
            if (CurrentTurn == Grid_Manager.Occupancy.Blue)
            {
                Red_TurnStart();
            }
            else if (CurrentTurn == Grid_Manager.Occupancy.Red)
            {
                Blue_TurnStart();
            }
        }
    }

    private void CheckGameOver()
    {
        int BlueStrongholdCount = 0;
        foreach(var IndividualCell in GridManager.Cells)
        {
            if(IndividualCell.Role == Grid_Manager.TileRoles.Stronghold && IndividualCell.Occupancy == Grid_Manager.Occupancy.Blue)
            {
                BlueStrongholdCount++;
            }
        }
        if(BlueStrongholdCount <= 0 )
        {
            IsGameOver = true;
            UIManager.GameOverCanvas_ToggleOn(Grid_Manager.Occupancy.Red);
        }

        int RedStrongholdCount = 0;
        foreach (var IndividualCell in GridManager.Cells)
        {
            if (IndividualCell.Role == Grid_Manager.TileRoles.Stronghold && IndividualCell.Occupancy == Grid_Manager.Occupancy.Red)
            {
                RedStrongholdCount++;
            }
        }
        if (RedStrongholdCount <= 0)
        {
            IsGameOver = true;
            UIManager.GameOverCanvas_ToggleOn(Grid_Manager.Occupancy.Blue);
        }
    }

    private void BluePayday()
    {
        foreach(var IndividualCell in GridManager.Cells)
        {
            if(IndividualCell.Role == Grid_Manager.TileRoles.Stronghold)
            {
                if(IndividualCell.Occupancy == Grid_Manager.Occupancy.Blue)
                {
                    IndividualCell.CellPower += 3 + MineValue_Total;
                    IndividualCell.CellGameObjects.GetComponent<Cells_SharedScript>().SetCellPower(IndividualCell.CellPower);
                }
            }
        }
    }

    private void RedPayday()
    {
        foreach (var IndividualCell in GridManager.Cells)
        {
            if (IndividualCell.Role == Grid_Manager.TileRoles.Stronghold)
            {
                if (IndividualCell.Occupancy == Grid_Manager.Occupancy.Red)
                {
                    IndividualCell.CellPower += 3 + MineValue_Total;
                    IndividualCell.CellGameObjects.GetComponent<Cells_SharedScript>().SetCellPower(IndividualCell.CellPower);
                }
            }
        }
    }
}