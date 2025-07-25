using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{

    [Header("Buttons")]
    [SerializeField] private Button anonymousAuthButton;
    [SerializeField] private Button phoneNoAuthButton;
    [SerializeField] private Button emailAuthButton;

    [Header("Panels")]
    [SerializeField] private GameObject menuPanel;
    [SerializeField] private GameObject anonymousAuthPanel;
    [SerializeField] private GameObject phoneNoAuthPanel;
    [SerializeField] private GameObject emailAuthPanel;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        InitializedUI();
        ButtonEventHandler();

    }

    void InitializedUI()
    {
        menuPanel.SetActive(true);
        anonymousAuthPanel.SetActive(false);
        phoneNoAuthPanel.SetActive(false);
        emailAuthPanel.SetActive(false);
    }

    void ButtonEventHandler()
    { 
        anonymousAuthButton.onClick.AddListener(delegate { menuPanel.SetActive(false); anonymousAuthPanel.SetActive(true); });
        phoneNoAuthButton.onClick.AddListener(delegate { menuPanel.SetActive(false); phoneNoAuthPanel.SetActive(true); });
        emailAuthButton.onClick.AddListener(delegate { menuPanel.SetActive(false); emailAuthPanel.SetActive(true); });
    }

}
