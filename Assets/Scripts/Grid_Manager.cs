using UnityEngine;

public class Grid_Manager : MonoBehaviour
{
    public enum TileRoles
    {
        Terrain,
        Stronghold,
        Mine
    }

    public enum Occupancy
    {
        Neutral,
        Blue,
        Red
    }

    public class Cell
    {
        public bool IsBlocked;
        public Occupancy Occupancy;
        public TileRoles Role;
        public int CellPower = 0;
        public GameObject CellGameObjects;
    }

    public Cell[,] Cells;
    public GameObject[] Cells_GameObjects;
    private Turn_Manager TurnManager;


    public int Map_MaxX = 0;
    public int Map_MaxZ = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        TurnManager = FindFirstObjectByType<Turn_Manager>();
        AssignCellMapLocations();
        BuildCellMap();
        AssignUnitXZ();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void AssignCellMapLocations() //Strictly about establishing Cell locations
    {
        GameObject[] HowManyCellsInScene = GameObject.FindGameObjectsWithTag("Cell");

        foreach(GameObject IndividualCell in HowManyCellsInScene)
        {
            var IndividualCell_X = IndividualCell.transform.position.x / 10;
            var IndividualCell_Z = IndividualCell.transform.position.z / 10;

            IndividualCell.GetComponent<Cells_SharedScript>().MapLocation_X = (int)IndividualCell_X;
            IndividualCell.GetComponent<Cells_SharedScript>().MapLocation_Z = (int)IndividualCell_Z;
        }

        Cells_GameObjects = HowManyCellsInScene;
    }

    private void BuildCellMap() //Establishes Cell roles and occupancy
    {

        foreach(var IndividualCell in Cells_GameObjects) //Establishing the grid's max size
        {
            if(IndividualCell.GetComponent<Cells_SharedScript>().MapLocation_X > Map_MaxX)
            {
                Map_MaxX = IndividualCell.GetComponent<Cells_SharedScript>().MapLocation_X;
            }
            if(IndividualCell.GetComponent<Cells_SharedScript>().MapLocation_Z > Map_MaxZ)
            {
                Map_MaxZ = IndividualCell.GetComponent<Cells_SharedScript>().MapLocation_Z;
            }

        }
        Cells = new Cell[Map_MaxX + 1, Map_MaxZ + 1]; //Filling the spaces using the max size

        for (int x = 0; x <= Map_MaxX; x++)
        {
            for (int z = 0; z <= Map_MaxZ; z++)
            {
                Cells[x, z] = new Cell();
            }
        }


        foreach (var IndividualCell in Cells_GameObjects) //Filling the spaces' infos
        {
            var CellScript = IndividualCell.GetComponent<Cells_SharedScript>();
            Cells[CellScript.MapLocation_X, CellScript.MapLocation_Z].IsBlocked = CellScript.IsBlocked;
            Cells[CellScript.MapLocation_X, CellScript.MapLocation_Z].Occupancy = CellScript.Occupant;
            Cells[CellScript.MapLocation_X, CellScript.MapLocation_Z].Role = CellScript.Role;
            Cells[CellScript.MapLocation_X, CellScript.MapLocation_Z].CellPower = CellScript.CellPower;
            Cells[CellScript.MapLocation_X, CellScript.MapLocation_Z].CellGameObjects = IndividualCell;

            CellScript.SetCellPower(Cells[CellScript.MapLocation_X, CellScript.MapLocation_Z].CellPower);

            if (IndividualCell.GetComponent<Cells_SharedScript>().Role == TileRoles.Stronghold)
            {
                IndividualCell.GetComponent<Stronghold_Behaviour>().SetSurroundings();
            }
        }


    }

    private void AssignUnitXZ()
    {
        foreach (var IndividualUnit in GameObject.FindGameObjectsWithTag("Unit"))
        {
            RaycastHit Hit;
            if(Physics.Raycast(IndividualUnit.transform.position, Vector3.down, out Hit, 10f))
            {
                GameObject CellBeneathUnit = Hit.collider.gameObject;
                IndividualUnit.GetComponent<Unit_Movement>().GridX = CellBeneathUnit.GetComponent<Cells_SharedScript>().MapLocation_X;
                IndividualUnit.GetComponent<Unit_Movement>().GridZ = CellBeneathUnit.GetComponent<Cells_SharedScript>().MapLocation_Z;
            }
        }
    }

    public bool CellCheck_IsWalkable(int Index_X, int Index_Z, Unit_Movement Script)
    {
        if(Index_X < 0 || Index_Z < 0 || Index_X > Map_MaxX || Index_Z > Map_MaxZ)
        {
            return false;
        }
        if (Cells[Index_X, Index_Z].IsBlocked)
        {
            return false;
        }
        if (Cells[Index_X,Index_Z].Occupancy == Script.Alignment)
        {
            Script.Supply(Index_X, Index_Z);
            return false;
        }

        return true;
    }

    public bool TryMovement(int TargetX, int TargetZ, Occupancy Alignment, Unit_Movement UnitMovementInstance)
    {
        if (!CellCheck_IsWalkable(TargetX, TargetZ, UnitMovementInstance))
        {
            return false;
        }

        if (!TurnManager.UnitMovementCheck(UnitMovementInstance.GetComponent<Unit_Movement>()))
        {
            return false;
        }

        if (Cells[TargetX, TargetZ].Occupancy != Alignment)
        {
            if (Cells[TargetX, TargetZ].Role == TileRoles.Stronghold)
            {
                UnitMovementInstance.Invade(TargetX, TargetZ);
                return false;
            }

            if (Cells[TargetX, TargetZ].Role == TileRoles.Mine)
            {
                UnitMovementInstance.Invade(TargetX,TargetZ);
                return false;
            }
        }

        return true;
    }

    public void Combat(GameObject Unit, GameObject EnemyUnit, int TargetX, int TargetZ)
    {
        Debug.Log("Combat function is called");
        var Unit_Script = Unit.GetComponent<Unit_Movement>();
        var EnemyUnit_Script = EnemyUnit.GetComponent<Unit_Movement>();

        if(Unit_Script.UnitPower > EnemyUnit_Script.UnitPower)
        {
            Unit_Script.UnitPower -= EnemyUnit_Script.UnitPower;
            Unit_Script.SetUnitPower();
            Destroy(EnemyUnit);
            Unit_Script.GridX = TargetX;
            Unit_Script.GridZ = TargetZ;
            Unit.transform.position = new Vector3(TargetX * 10, 1, TargetZ * 10);
        }
        else if(Unit_Script.UnitPower < EnemyUnit_Script.UnitPower)
        {
            EnemyUnit_Script.UnitPower -= Unit_Script.UnitPower;
            EnemyUnit_Script.SetUnitPower();
            Destroy(Unit);
        }
        else if(Unit_Script.UnitPower == EnemyUnit_Script.UnitPower)
        {
            Destroy(EnemyUnit); Destroy(Unit);
        }
    }
}