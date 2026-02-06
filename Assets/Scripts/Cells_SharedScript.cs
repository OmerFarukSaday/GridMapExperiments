using TMPro;
using UnityEngine;

public class Cells_SharedScript : MonoBehaviour
{
    public int MapLocation_X;
    public int MapLocation_Z;
    public Grid_Manager.TileRoles Role;
    public Grid_Manager.Occupancy Occupant;
    public bool IsBlocked;
    public int CellPower = 0; //WE ONLY USE THIS IN THE BEGINNING. THEN WE GIVE RESPONSIBILITIES TO CELLS (the array).
    [SerializeField] private Material[] AlignmentMaterials;
    [SerializeField] private TextMeshProUGUI CellPower_Text;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SetAlignment();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetAlignment()
    {
        if(Role == Grid_Manager.TileRoles.Stronghold || Role == Grid_Manager.TileRoles.Mine)
        {
            switch (Occupant)
            {
                case Grid_Manager.Occupancy.Neutral:
                    gameObject.GetComponent<Renderer>().material = AlignmentMaterials[0];
                    break;
                case Grid_Manager.Occupancy.Blue:
                    gameObject.GetComponent<Renderer>().material = AlignmentMaterials[1];
                    break;
                case Grid_Manager.Occupancy.Red:
                    gameObject.GetComponent<Renderer>().material = AlignmentMaterials[2];
                    break;
            }
        }
    }

    public void SetCellPower(int Power)
    {
        CellPower = Power;
        if(CellPower_Text != null)
        {
            CellPower_Text.text = Power.ToString();
        }
    }
}
