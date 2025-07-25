using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;
using Firebase.Extensions;
using Firebase.Auth;
using TMPro;

public class AnonymousAuth : MonoBehaviour
{

    [Header("Reference")]
    [SerializeField] private GameObject logInPanel;
    [SerializeField] private GameObject successPanel;
    [SerializeField] private GameObject loadingPanel;
    [SerializeField] private Button annonymousLogInButton;
    [SerializeField] private TextMeshProUGUI successText;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        InitializedUI();
        ButtonEvnet();
    }

    private void InitializedUI()
    { 
        logInPanel.SetActive(true);
        successPanel.SetActive(false);
        loadingPanel.SetActive(false);
    }

    private void ButtonEvnet()
    {
        annonymousLogInButton.onClick.AddListener(delegate { logInPanel.SetActive(false); loadingPanel.SetActive(true); AnonymousLogin(); });
    }

    private async void AnonymousLogin()
    {
        await AnonymousLogInAuth();
    }

    private async Task AnonymousLogInAuth()
    {
        FirebaseAuth auth = FirebaseAuth.DefaultInstance;
        await auth.SignInAnonymouslyAsync().ContinueWithOnMainThread(task => {
        if (task.IsCanceled) {
            Debug.LogError("SignInAnonymouslyAsync was canceled.");
            return;
        }
        if (task.IsFaulted) {
            Debug.LogError("SignInAnonymouslyAsync encountered an error: " + task.Exception);
            return;
        }
        loadingPanel.SetActive(false);
        successPanel.SetActive(true);
        AuthResult result = task.Result;
        successText.text = "Successfully signed in " + result.User.UserId;
        Debug.LogFormat("User signed in successfully: {0} ({1})",
            result.User.DisplayName, result.User.UserId);
        });
    }    

}
