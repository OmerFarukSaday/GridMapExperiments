using UnityEngine;

public class MapGen_Manager : MonoBehaviour
{
    public int GridMaxSize_X;
    public int GridMaxSize_Z;
    [SerializeField] private GameObject TerrainPrefab;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        for(int i = 0; i < GridMaxSize_X; i++)
        {
            for(int j = 0; j  < GridMaxSize_Z; j++)
            {
                Vector3 WorldPos = new Vector3(i * 10, 0, j * 10);
                Vector3 HalfExtents = new Vector3(4.5f, 1f, 4.5f);

                if (!Physics.CheckBox(WorldPos, HalfExtents))
                {
                    var TerrainObject = Instantiate(TerrainPrefab, WorldPos, Quaternion.identity);
                    var TerrainObject_Script = TerrainObject.GetComponent<Cells_SharedScript>();
                    TerrainObject_Script.MapLocation_X = i;
                    TerrainObject_Script.MapLocation_Z = j;
                    TerrainObject_Script.Role = Grid_Manager.TileRoles.Terrain;
                    TerrainObject_Script.Occupant = Grid_Manager.Occupancy.Neutral;
                    TerrainObject_Script.IsBlocked = false;
                    TerrainObject_Script.CellPower = 0;
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
