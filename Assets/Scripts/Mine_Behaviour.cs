using TMPro;
using UnityEngine;

public class Mine_Behaviour : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI Mine_Text;
    public int MineValue = 1;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SetMineText();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void TurnStarted_UpdateValue()
    {
        MineValue = Mathf.Min(MineValue + 1, 5);
        SetMineText();
    }

    public void SetMineText()
    {
        Mine_Text.text = MineValue.ToString();
    }
}
