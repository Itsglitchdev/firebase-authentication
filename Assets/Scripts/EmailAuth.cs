using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Firebase.Extensions;
using Firebase.Auth;
using Firebase;
using Unity.VisualScripting;

public class EmailAuth : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject emailLoginPanel;
    [SerializeField] private GameObject emailRegisterPanel;
    [SerializeField] private GameObject loadingPanel;
    [SerializeField] private GameObject successPanel;
    [Header("Buttons")]
    [SerializeField] private Button emailLoginButton;
    [SerializeField] private Button emailRegisterButton;
    [SerializeField] private Button inLoginHaveRegisterButton;
    [SerializeField] private Button inRegisterHaveLoginButton;
    [Header("Inputs")]
    [SerializeField] private TMP_InputField registerEmailInput;
    [SerializeField] private TMP_InputField registerpasswordInput;
    [SerializeField] private TMP_InputField registerConfirmPasswordInput;
    [SerializeField] private TMP_InputField loginEmailInput;
    [SerializeField] private TMP_InputField loginPasswordInput;
    [Header("Texts")]
    [SerializeField] private TextMeshProUGUI successText;


    void Start()
    {
        InitializedUI();
        ButtonEventHandler();
    }

    void InitializedUI()
    {
        emailLoginPanel.SetActive(true);
        emailRegisterPanel.SetActive(false);
        loadingPanel.SetActive(false);
        successPanel.SetActive(false);
    }

    void ButtonEventHandler()
    {
        emailLoginButton.onClick.AddListener(delegate { emailLoginPanel.SetActive(false); loadingPanel.SetActive(true); LogIn(); });
        emailRegisterButton.onClick.AddListener(delegate { emailRegisterPanel.SetActive(false); loadingPanel.SetActive(true); Register(); });
        inLoginHaveRegisterButton.onClick.AddListener(delegate { emailLoginPanel.SetActive(false); emailRegisterPanel.SetActive(true); });
        inRegisterHaveLoginButton.onClick.AddListener(delegate { emailLoginPanel.SetActive(true); emailRegisterPanel.SetActive(false); });
    }

    async void Register()
    {
        await EmailRegister();
    }

    async void LogIn()
    {
        await EmailLogInAuth();
    }

    async Task EmailRegister()
    {
        FirebaseAuth auth = FirebaseAuth.DefaultInstance;
        string email = registerEmailInput.text;
        string password = registerpasswordInput.text;
        await auth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled)
            {
                Debug.LogError("CreateUserWithEmailAndPasswordAsync was canceled.");
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("CreateUserWithEmailAndPasswordAsync encountered an error: " + task.Exception);
                return;
            }
            loadingPanel.SetActive(false);
            successPanel.SetActive(true);

            // Firebase user has been created.
            AuthResult result = task.Result;
            successText.text = "Successfully Registered in " + result.User.Email;
           
            Debug.LogFormat("Firebase user created successfully: {0} ({1})",
                result.User.DisplayName, result.User.UserId);

        });
    }

    async Task EmailLogInAuth()
    {
        FirebaseAuth auth = FirebaseAuth.DefaultInstance;
        string email = loginEmailInput.text;
        string password = loginPasswordInput.text;
        await auth.SignInWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled)
            {
                Debug.LogError("SignInWithEmailAndPasswordAsync was canceled.");
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("SignInWithEmailAndPasswordAsync encountered an error: " + task.Exception);
                return;
            }
            loadingPanel.SetActive(false);
            successPanel.SetActive(true);
           
            AuthResult result = task.Result;
            successText.text = "Successfully logged in " + task.Result.User.Email;
            Debug.LogFormat("User signed in successfully: {0} ({1})",
                result.User.DisplayName, result.User.UserId);
        });
    }

}
