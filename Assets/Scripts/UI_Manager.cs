using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UI_Manager : MonoBehaviour
{
    [SerializeField] private GameObject StrongholdCanvas;
    [SerializeField] private Slider StrongholdCanvas_Slider;
    [SerializeField] private TextMeshProUGUI StrongholdCanvas_Text;
    private GameObject ClickedStronghold;
    [SerializeField] private GameObject GameOverCanvas;
    [SerializeField] private TextMeshProUGUI GameOverCanvas_Text;
    private Grid_Manager GridManager;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GridManager = FindFirstObjectByType<Grid_Manager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StrongHoldCanvas_ToggleOn(Cells_SharedScript Stronghold_SharedScript, GameObject Stronghold_GameObject)
    {
        StrongholdCanvas.SetActive(true);
        StrongholdCanvas_Slider.maxValue = Stronghold_SharedScript.CellPower;
        ClickedStronghold = Stronghold_GameObject;
    }

    public void StrongholdCanvas_ToggleOff()
    {
        ClickedStronghold = null;
        StrongholdCanvas.SetActive(false);
    }

    public void StrongholdCanvas_SliderValueChange()
    {
        StrongholdCanvas_Text.text = StrongholdCanvas_Slider.value.ToString();
    }

    public void StrongHoldCanvas_SpawnUnit_ButtonClick()
    {
        var X = ClickedStronghold.GetComponent<Cells_SharedScript>().MapLocation_X;
        var Z = ClickedStronghold.GetComponent<Cells_SharedScript>().MapLocation_Z;
        if(StrongholdCanvas_Slider.value != 0 && GridManager.Cells[X,Z].CellPower >= StrongholdCanvas_Slider.value)
        {
            ClickedStronghold.GetComponent<Stronghold_Behaviour>().SpawnUnit((int)StrongholdCanvas_Slider.value);
        }
    }

    public void GameOverCanvas_ToggleOn(Grid_Manager.Occupancy WinnerSide)
    {
        GameOverCanvas.SetActive(true);
        GameOverCanvas_Text.text = "Game Over - " + WinnerSide.ToString() + " Won!";
    }

    public void ReplayLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene(0);
    }
}
