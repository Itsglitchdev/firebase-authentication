using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Firebase.Auth;
using Firebase.Extensions;

public class PhoneAuth : MonoBehaviour
{

    [Header("Panels")]
    [SerializeField] private GameObject phoneNoAuthPanel;
    [SerializeField] private GameObject otpNoAuthPanel;
    [SerializeField] private GameObject sucessPanel;
    [SerializeField] private GameObject loadingPanel;

    [Header("Buttons")]
    [SerializeField] private Button otpSendButton;
    [SerializeField] private Button submitButton;

    [Header("Input")]
    [SerializeField] private TMP_InputField phoneNoInput;
    [SerializeField] private TMP_InputField otpInput;

    [Header("Text")]
    [SerializeField] private TMP_Text successText;

    void Start()
    {
        InitializedUI();
        ButtonEventHandler();
    }

    void InitializedUI()
    {
        phoneNoAuthPanel.SetActive(true);
        otpNoAuthPanel.SetActive(false);
        sucessPanel.SetActive(false);
        loadingPanel.SetActive(false);
    }

    void ButtonEventHandler()
    {
        otpSendButton.onClick.AddListener(delegate { otpSendButton.interactable = false; otpNoAuthPanel.SetActive(true); SendOTP(); });
        submitButton.onClick.AddListener(delegate { phoneNoAuthPanel.SetActive(false); otpNoAuthPanel.SetActive(false); loadingPanel.SetActive(true); SubmitOTP(); });
    }

    private void SendOTP()
    {
        FirebaseAuth auth = FirebaseAuth.DefaultInstance;
        PhoneAuthProvider provider = PhoneAuthProvider.GetInstance(auth);

        string userNumber = "+91" + phoneNoInput.text;


        provider.VerifyPhoneNumber(
        new PhoneAuthOptions
        {
            PhoneNumber = userNumber,
            TimeoutInMilliseconds = 60,
            ForceResendingToken = null
        },
        verificationCompleted: (credential) =>
        {
            Debug.Log("Verification completed");
            // Auto-sms-retrieval or instant validation has succeeded (Android only).
            // There is no need to input the verification code.
            // `credential` can be used instead of calling GetCredential().
        },
          verificationFailed: (error) =>
          {
              Debug.Log("Error: " + error);
              // The verification code was not sent.
              // `error` contains a human readable explanation of the problem.
          },
          codeSent: (id, token) =>
          {
              // Verification code was successfully sent via SMS.
              // `id` contains the verification id that will need to passed in with
              // the code from the user when calling GetCredential().
              PlayerPrefs.SetString("verificationId",id);
              // `token` can be used if the user requests the code be sent again, to
              // tie the two requests together.
          },
          codeAutoRetrievalTimeOut: (id) =>
          {
              // Called when the auto-sms-retrieval has timed out, based on the given
              // timeout parameter.
              // `id` contains the verification id of the request that timed out.
          });
    }
    
    private void SubmitOTP()
    {
        string verificationId = PlayerPrefs.GetString("verificationId");
        string verificationCode = otpInput.text;
        FirebaseAuth auth = FirebaseAuth.DefaultInstance;
        PhoneAuthProvider provider = PhoneAuthProvider.GetInstance(auth);
        PhoneAuthCredential credential = provider.GetCredential(verificationId, verificationCode);
    
        auth.SignInWithCredentialAsync(credential).ContinueWithOnMainThread(task => {
            if (task.IsFaulted)
            {
                Debug.LogError("SignInAndRetrieveDataWithCredentialAsync encountered an error: " +
                               task.Exception);
                return;
            }

            if (task.IsCompleted)
            {
                loadingPanel.SetActive(false);
                sucessPanel.SetActive(true);
                successText.text = "Successfully logged in via phone number" + task.Result.PhoneNumber;
            }
        });
    }


}
