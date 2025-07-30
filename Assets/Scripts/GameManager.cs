using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{

    [Header("Buttons")]
    [SerializeField] private Button anonymousAuthButton;
    [SerializeField] private Button phoneNoAuthButton;
    [SerializeField] private Button simpleEmailAuthButton;
    [SerializeField] private Button verifyEmailAuthButton;

    [Header("Panels")]
    [SerializeField] private GameObject menuPanel;
    [SerializeField] private GameObject anonymousAuthPanel;
    [SerializeField] private GameObject phoneNoAuthPanel;
    [SerializeField] private GameObject simpleEmailAuthPanel;
    [SerializeField] private GameObject verifyEmailAuthPanel;


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
        simpleEmailAuthPanel.SetActive(false);
        verifyEmailAuthPanel.SetActive(false);
    }

    void ButtonEventHandler()
    {
        anonymousAuthButton.onClick.AddListener(delegate { menuPanel.SetActive(false); anonymousAuthPanel.SetActive(true); });
        phoneNoAuthButton.onClick.AddListener(delegate { menuPanel.SetActive(false); phoneNoAuthPanel.SetActive(true); });
        simpleEmailAuthButton.onClick.AddListener(delegate { menuPanel.SetActive(false); simpleEmailAuthPanel.SetActive(true); });
        verifyEmailAuthButton.onClick.AddListener(delegate { menuPanel.SetActive(false); verifyEmailAuthPanel.SetActive(true); });
    }

}
