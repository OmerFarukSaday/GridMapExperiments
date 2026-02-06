using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class Stronghold_Behaviour : MonoBehaviour
{
    public class Surroundings_Coordinates
    {
        public int x;
        public int Z;
    }
    private int Offset1 = 1;
    private int Offset2 = 1;

    private System.Random Surroundings_RandomNumber = new System.Random();

    private int SetSurroundings_Count = 0;
    private int ValidSurroundings_Count = 0;
    public Surroundings_Coordinates[] TotalSurroundings = new Surroundings_Coordinates[8];
    public Surroundings_Coordinates[] ValidSurroundings;
    private UI_Manager UIManager;
    private Grid_Manager GridManager;
    private Turn_Manager TurnManager;
    [SerializeField] private GameObject UnitPrefab;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        UIManager = FindFirstObjectByType<UI_Manager>();
        GridManager = FindFirstObjectByType<Grid_Manager>();
        TurnManager = FindFirstObjectByType<Turn_Manager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // 0 = left mouse button
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform == transform)
                {
                    var Script = gameObject.GetComponent<Cells_SharedScript>();
                    if (Script.Occupant == Grid_Manager.Occupancy.Blue || Script.Occupant == Grid_Manager.Occupancy.Red)
                    {
                        if (Script.Occupant == TurnManager.CurrentTurn)
                        {
                            UIManager.StrongHoldCanvas_ToggleOn(gameObject.GetComponent<Cells_SharedScript>(), gameObject);
                        }
                    }
                }
            }
        }
    }

    public void SetSurroundings()
    {
        SetSurroundings_Count = 0;
        ValidSurroundings_Count = 0;

        for (int i = 0; i < TotalSurroundings.Length; i++)
        {
            TotalSurroundings[i] = new Surroundings_Coordinates();
        }
        for(int i = -1; i <= Offset1;  i++)
        {
            for(int j = -1; j <= Offset2; j++)
            {
                if (i == 0 && j == 0)
                {
                    continue;
                }

                if(GetComponent<Cells_SharedScript>().MapLocation_X + i < 0 || GetComponent<Cells_SharedScript>().MapLocation_Z + j < 0
                || GetComponent<Cells_SharedScript>().MapLocation_X + i > GridManager.Map_MaxX || GetComponent<Cells_SharedScript>().MapLocation_Z + j > GridManager.Map_MaxZ)
                {
                    continue;
                }
                TotalSurroundings[SetSurroundings_Count].x = gameObject.GetComponent<Cells_SharedScript>().MapLocation_X + i;
                TotalSurroundings[SetSurroundings_Count].Z = gameObject.GetComponent<Cells_SharedScript>().MapLocation_Z + j;
                SetSurroundings_Count++;
            }
        }

        if (ValidSurroundings == null || ValidSurroundings.Length != SetSurroundings_Count)
        {
            ValidSurroundings = new Surroundings_Coordinates[SetSurroundings_Count];
        }

        for (int i = 0; i < SetSurroundings_Count; i++)
        {
            var Coord = TotalSurroundings[i];

            if (Coord == null) continue;

            // safe bounds check (extra safety)
            if (Coord.x < 0 || Coord.x > GridManager.Map_MaxX || Coord.Z < 0 || Coord.Z > GridManager.Map_MaxZ)
                continue;

            var currentCell = GridManager.Cells[Coord.x, Coord.Z];

            // only add valid terrain cells
            if (currentCell.Role == Grid_Manager.TileRoles.Terrain && !currentCell.IsBlocked)
            {
                ValidSurroundings[ValidSurroundings_Count] = new Surroundings_Coordinates();
                ValidSurroundings[ValidSurroundings_Count].x = Coord.x;
                ValidSurroundings[ValidSurroundings_Count].Z = Coord.Z;
                ValidSurroundings_Count++;
            }
        }
    }

    public void SpawnUnit(int SpawnedUnit_Power)
    {
        SetSurroundings();
        if (ValidSurroundings_Count == 0)
        {
            Debug.LogWarning("No valid surrounding cells to spawn a unit!");
            return;
        }

        GridManager.Cells[gameObject.GetComponent<Cells_SharedScript>().MapLocation_X, gameObject.GetComponent<Cells_SharedScript>().MapLocation_Z].CellPower -= SpawnedUnit_Power;
        gameObject.GetComponent<Cells_SharedScript>().SetCellPower(GridManager.Cells[gameObject.GetComponent<Cells_SharedScript>().MapLocation_X, gameObject.GetComponent<Cells_SharedScript>().MapLocation_Z].CellPower);

        int RandomIndex = Surroundings_RandomNumber.Next(0, ValidSurroundings_Count);
        var spawnCoord = ValidSurroundings[RandomIndex];

        Vector3 spawnPosition = new Vector3(spawnCoord.x * 10, 1, spawnCoord.Z * 10);
        GameObject SpawnedUnit = Instantiate(UnitPrefab, spawnPosition, Quaternion.identity);
        SpawnedUnit.GetComponent<Unit_Movement>().UnitPower = SpawnedUnit_Power;
        SpawnedUnit.GetComponent<Unit_Movement>().Alignment = gameObject.GetComponent<Cells_SharedScript>().Occupant;
        SpawnedUnit.GetComponent<Unit_Movement>().GridX = spawnCoord.x;
        SpawnedUnit.GetComponent<Unit_Movement>().GridZ = spawnCoord.Z;
    }
}