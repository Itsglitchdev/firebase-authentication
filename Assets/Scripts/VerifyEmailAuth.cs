using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Threading.Tasks;
using Firebase.Auth;
using Firebase.Extensions;

public class VerifyEmailAuth : MonoBehaviour
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

        try
        {
            var result = await auth.CreateUserWithEmailAndPasswordAsync(email, password);
            Debug.Log("User registered: " + result.User.Email);
            await EmailVerification();
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Registration failed: " + ex.Message);
        }
    }


    async Task EmailVerification()
    {
        FirebaseAuth auth = FirebaseAuth.DefaultInstance;
        FirebaseUser user = auth.CurrentUser;

        if (user != null)
        {
            try
            {
                await user.SendEmailVerificationAsync();
                Debug.Log("Verification email sent.");
                loadingPanel.SetActive(false);
                successPanel.SetActive(true);
                successText.text = "Verification email sent! Please check your inbox.";
                // Below portion is not working because need proper handling but code is pefect for the future.
                if (user.IsEmailVerified)
                {
                    successText.text = "Successfully Registered in " + user.Email;
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError("Verification email failed: " + ex.Message);
            }
        }
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