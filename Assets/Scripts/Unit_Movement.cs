using TMPro;
using UnityEngine;

public class Unit_Movement : MonoBehaviour
{
    public int GridX;
    public int GridZ;
    public bool IsSelectable = false; //If it is Blue or Red turn
    public bool IsMovable = false; //If the object is clicked on / clicked off
    [SerializeField] private Grid_Manager GridManager;
    public Grid_Manager.Occupancy Alignment;
    public Material[] AlignmentMaterials;
    public int UnitPower;
    public TextMeshProUGUI UnitPower_Text;
    void Start()
    {
        GridManager = FindFirstObjectByType<Grid_Manager>();
        SetAlignment();
        SetUnitPower();

    }

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
                    // This unit was clicked
                    IsMovable = true;
                }
                else
                {
                    // Only set unclicked if this unit was previously selected
                    if (IsMovable)
                    {
                        IsMovable = false;
                    }
                }
            }
            else
            {
                // Only set unclicked if this unit was previously selected
                if (IsMovable)
                {
                    IsMovable = false;
                }
            }
        }

        if(!IsMovable)
        {
            return;
        }

        Vector2Int Direction = Vector2Int.zero;

        if(Input.GetKeyDown(KeyCode.W)) { Direction = Vector2Int.up; }
        if(Input.GetKeyDown(KeyCode.S)) { Direction = Vector2Int.down; }
        if(Input.GetKeyDown(KeyCode.A)) { Direction = Vector2Int.left; }
        if(Input.GetKeyDown(KeyCode.D)) { Direction = Vector2Int.right; }

        if(Direction != Vector2Int.zero)
        {
            int TargetX = GridX + Direction.x;
            int TargetZ = GridZ + Direction.y;

            Collider[] colliders = Physics.OverlapBox(new Vector3(TargetX * 10, 1, TargetZ * 10), new Vector3(4, 2, 4), Quaternion.identity);
            foreach(Collider IndividualCollider in colliders)
            {
                Debug.Log(IndividualCollider.gameObject.name);
                if(IndividualCollider.gameObject.tag == "Unit")
                {
                    if(IndividualCollider.GetComponent<Unit_Movement>().Alignment != Alignment)
                    {
                        GridManager.Combat(gameObject, IndividualCollider.gameObject, TargetX, TargetZ);
                        return;
                    }
                }
            }

            if(GridManager.TryMovement(TargetX, TargetZ, Alignment, GetComponent<Unit_Movement>()) == true)
            {
                GridX = TargetX;
                GridZ = TargetZ;
                transform.position = new Vector3(TargetX * 10, 1, TargetZ * 10);
            }
        }
    }

    public void SetAlignment()
    {
        switch(Alignment)
        {
            case Grid_Manager.Occupancy.Neutral:
                gameObject.GetComponent<Renderer>().material = AlignmentMaterials[0]; break;
            case Grid_Manager.Occupancy.Blue:
                gameObject.GetComponent<Renderer>().material = AlignmentMaterials[1]; break;
            case Grid_Manager.Occupancy.Red:
                gameObject.GetComponent<Renderer>().material = AlignmentMaterials[2]; break;
        }
    }

    public void SetUnitPower()
    {
        UnitPower_Text.text = UnitPower.ToString();
    }

    public void Invade(int X, int Z)
    {
        int cellPower = GridManager.Cells[X, Z].CellPower;

        if (cellPower > UnitPower || cellPower == UnitPower)
        {
            GridManager.Cells[X, Z].CellPower -= UnitPower;
            GridManager.Cells[X, Z].CellGameObjects.GetComponent<Cells_SharedScript>().SetCellPower(GridManager.Cells[X, Z].CellPower);
            Destroy(gameObject);
            return;
        }

        if (cellPower < UnitPower)
        {
            int NewCellPower = UnitPower - cellPower;

            GridManager.Cells[X, Z].CellPower = NewCellPower;
            GridManager.Cells[X, Z].CellGameObjects.GetComponent<Cells_SharedScript>().CellPower = NewCellPower;

            GridManager.Cells[X, Z].Occupancy = Alignment;

            var CellScript = GridManager.Cells[X, Z].CellGameObjects.GetComponent<Cells_SharedScript>();
            CellScript.Occupant = Alignment;

            if (GridManager.Cells[X, Z].Role == Grid_Manager.TileRoles.Stronghold)
            {
                var Stronghold = GridManager.Cells[X, Z].CellGameObjects.GetComponent<Cells_SharedScript>();
                Stronghold.SetAlignment();
                Stronghold.SetCellPower(NewCellPower);
            }

            if (GridManager.Cells[X, Z].Role == Grid_Manager.TileRoles.Mine)
            {
                var Mine = GridManager.Cells[X, Z].CellGameObjects.GetComponent<Cells_SharedScript>();
                Mine.SetAlignment();
                Mine.SetCellPower(NewCellPower);

                var Mine_Mine = GridManager.Cells[X, Z].CellGameObjects.GetComponent<Mine_Behaviour>();
                Mine_Mine.MineValue = 1;
                Mine_Mine.SetMineText();
            }

            Destroy(gameObject);
        }
    }

    public void Supply(int X, int Z)
    {
        GridManager.Cells[X, Z].CellPower += UnitPower;
        GridManager.Cells[X, Z].CellGameObjects.GetComponent<Cells_SharedScript>().SetCellPower(GridManager.Cells[X,Z].CellPower);

        Destroy(gameObject);
    }
}