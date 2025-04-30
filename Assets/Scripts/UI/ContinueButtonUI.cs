using UnityEngine;
using UnityEngine.UI;

public class ContinueButtonUI : MonoBehaviour
{

    [SerializeField] private Button button;

    public void Start()
    {
        button.onClick.AddListener(() =>
        {
            OnButtonPressed();   
        });
    }

    private void OnButtonPressed()
    {
        RoundManager.Instance.ContinueButtonPressed();
    }
}
