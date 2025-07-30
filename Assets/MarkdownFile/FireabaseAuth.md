# Firebase Authentication Guide for Unity

## Table of Contents
1. [What is Firebase Authentication?](#what-is-firebase-authentication)
2. [Why Do We Need Authentication?](#why-do-we-need-authentication)
3. [Firebase Console Setup](#firebase-console-setup)
4. [Unity Project Setup](#unity-project-setup)
5. [Anonymous Authentication](#anonymous-authentication)
6. [Email Authentication](#email-authentication)
7. [Phone Number Authentication](#phone-number-authentication)
8. [Best Practices](#best-practices)

---

## What is Firebase Authentication?

Firebase Authentication is a service provided by Google Firebase that offers backend services and SDKs to authenticate users in your application. It provides a complete identity solution, supporting authentication using passwords, phone numbers, popular federated identity providers like Google, Facebook, Twitter, and more.

### Key Features:
- **Multiple Authentication Methods**: Email/password, phone, anonymous, social providers
- **Security**: Built-in security features and token-based authentication
- **Cross-Platform**: Works across web, mobile, and desktop platforms
- **Real-time Updates**: Automatic token refresh and user state management
- **Easy Integration**: Simple SDKs and APIs

---

## Why Do We Need Authentication?

Authentication is crucial for modern applications for several reasons:

### 1. **User Identity Management**
- Identify unique users across sessions and devices
- Maintain user preferences and game progress
- Enable personalized experiences

### 2. **Data Security**
- Protect user data and prevent unauthorized access
- Secure API calls and database operations
- Implement proper access controls

### 3. **Feature Access Control**
- Enable premium features for registered users
- Implement user roles and permissions
- Track user engagement and analytics

### 4. **Business Benefits**
- Build user communities and social features
- Enable in-app purchases and subscriptions
- Gather user insights for product improvement

---

## Firebase Console Setup

### Step 1: Create a Firebase Project

1. **Go to Firebase Console**
   - Navigate to [https://console.firebase.google.com/](https://console.firebase.google.com/)
   - Sign in with your Google account

2. **Create New Project**
   - Click "Create a project"
   - Enter your project name
   - Choose whether to enable Google Analytics (recommended)
   - Select your Analytics account (if enabled)
   - Click "Create project"

### Step 2: Add Your Unity App

1. **Add App to Project**
   - In your project dashboard, click the Unity icon
   - Or go to Project Settings → General → Your apps

2. **Configure Your App**
   - **Package Name**: Enter your Unity package identifier (e.g., `com.yourcompany.yourgame`)
   - **App Nickname**: Choose a friendly name for your app
   - **Debug Signing Certificate**: Add SHA-1 and SHA-256 (required for phone auth)

3. **Download Configuration File**
   - Download the `google-services.json` file (Android)
   - Download the `GoogleService-Info.plist` file (iOS)
   - Keep these files safe - you'll need them in Unity

### Step 3: Enable Authentication Methods

1. **Go to Authentication**
   - In Firebase Console, navigate to Authentication
   - Click on "Get started" if it's your first time

2. **Configure Sign-in Methods**
   - Go to "Sign-in method" tab
   - Enable the authentication methods you want to use:
     - **Anonymous**: For guest users
     - **Email/Password**: For email-based registration
     - **Phone**: For SMS-based authentication
     - **Google, Facebook, etc.**: For social login

---

## Unity Project Setup

### Step 1: Install Firebase SDK

1. **Download Firebase Unity SDK**
   - Go to [Firebase Unity SDK Download](https://firebase.google.com/download/unity)
   - Download the latest version

2. **Import Firebase Auth Package**
   - In Unity, go to Assets → Import Package → Custom Package
   - Select `FirebaseAuth.unitypackage` from the downloaded SDK
   - Import all files

### Step 2: Add Configuration Files

1. **Add Google Services File**
   - Copy `google-services.json` to `Assets/StreamingAssets/` (Android)
   - Copy `GoogleService-Info.plist` to `Assets/StreamingAssets/` (iOS)

2. **Configure Build Settings**
   - Go to File → Build Settings
   - Select your target platform (Android/iOS)
   - Configure platform-specific settings

### Step 3: Initialize Firebase

```csharp
using Firebase;
using Firebase.Auth;
using UnityEngine;

public class FirebaseManager : MonoBehaviour
{
    private FirebaseAuth auth;
    
    void Start()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task => {
            var dependencyStatus = task.Result;
            if (dependencyStatus == DependencyStatus.Available)
            {
                InitializeFirebase();
            }
            else
            {
                Debug.LogError($"Could not resolve all Firebase dependencies: {dependencyStatus}");
            }
        });
    }
    
    void InitializeFirebase()
    {
        auth = FirebaseAuth.DefaultInstance;
        Debug.Log("Firebase Auth initialized successfully");
    }
}
```

---

## Anonymous Authentication

### What is Anonymous Authentication?

Anonymous authentication allows users to use your app without providing personal information. It creates a temporary account that can later be upgraded to a permanent account.

### Why Use Anonymous Authentication?

- **Instant Access**: Users can start using your app immediately
- **No Barriers**: No registration forms or personal data required
- **Game Progress**: Save progress even for guest users
- **Easy Upgrade**: Can convert to permanent account later
- **Testing**: Perfect for app testing and demos

### Implementation

```csharp
using Firebase.Auth;
using UnityEngine;
using System.Threading.Tasks;

public class AnonymousAuth : MonoBehaviour
{
    private FirebaseAuth auth;
    
    void Start()
    {
        auth = FirebaseAuth.DefaultInstance;
    }
    
    public void SignInAnonymously()
    {
        auth.SignInAnonymouslyAsync().ContinueWith(task => {
            if (task.IsCanceled)
            {
                Debug.LogError("Anonymous sign-in was canceled.");
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("Anonymous sign-in failed: " + task.Exception);
                return;
            }

            FirebaseUser newUser = task.Result.User;
            Debug.Log($"Anonymous user signed in: {newUser.UserId}");
            Debug.Log($"Is Anonymous: {newUser.IsAnonymous}");
            
            // Handle successful sign-in
            OnSignInSuccess(newUser);
        });
    }
    
    private void OnSignInSuccess(FirebaseUser user)
    {
        // Update UI or navigate to main game
        Debug.Log("User authenticated successfully!");
        // Load user data, enable features, etc.
    }
}
```

### Advanced Anonymous Auth with Error Handling

```csharp
public async Task<bool> SignInAnonymouslyAsync()
{
    try
    {
        AuthResult result = await auth.SignInAnonymouslyAsync();
        FirebaseUser user = result.User;
        
        Debug.Log($"Anonymous sign-in successful");
        Debug.Log($"User ID: {user.UserId}");
        Debug.Log($"Creation Time: {user.Metadata.CreationTimestamp}");
        
        return true;
    }
    catch (System.Exception ex)
    {
        Debug.LogError($"Anonymous sign-in failed: {ex.Message}");
        return false;
    }
}
```

---

## Email Authentication

### What is Email Authentication?

Email authentication allows users to create accounts and sign in using their email address and password. This provides a permanent, recoverable account.

### Why Use Email Authentication?

- **Permanent Accounts**: Users can recover their accounts
- **Cross-Device Access**: Sign in from multiple devices
- **Password Recovery**: Built-in password reset functionality
- **User Communication**: Can send emails to users
- **Trusted Identity**: More reliable than anonymous accounts

### Sign Up Implementation

```csharp
using Firebase.Auth;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;

public class EmailAuth : MonoBehaviour
{
    [Header("UI References")]
    public InputField emailInput;
    public InputField passwordInput;
    public InputField confirmPasswordInput;
    public Button signUpButton;
    public Button signInButton;
    public Text statusText;
    
    private FirebaseAuth auth;
    
    void Start()
    {
        auth = FirebaseAuth.DefaultInstance;
        
        signUpButton.onClick.AddListener(SignUpWithEmail);
        signInButton.onClick.AddListener(SignInWithEmail);
    }
    
    public void SignUpWithEmail()
    {
        string email = emailInput.text;
        string password = passwordInput.text;
        string confirmPassword = confirmPasswordInput.text;
        
        // Validation
        if (!ValidateInput(email, password, confirmPassword))
            return;
            
        SignUpAsync(email, password);
    }
    
    private async void SignUpAsync(string email, string password)
    {
        try
        {
            statusText.text = "Creating account...";
            
            AuthResult result = await auth.CreateUserWithEmailAndPasswordAsync(email, password);
            FirebaseUser newUser = result.User;
            
            Debug.Log($"User created successfully: {newUser.Email}");
            statusText.text = "Account created successfully!";
            
            // Send verification email
            await SendVerificationEmail(newUser);
            
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Sign up failed: {ex.Message}");
            statusText.text = $"Sign up failed: {ex.Message}";
        }
    }
    
    private async Task SendVerificationEmail(FirebaseUser user)
    {
        try
        {
            await user.SendEmailVerificationAsync();
            Debug.Log("Verification email sent");
            statusText.text = "Verification email sent! Please check your inbox.";
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Failed to send verification email: {ex.Message}");
        }
    }
}
```

### Sign In Implementation

```csharp
public void SignInWithEmail()
{
    string email = emailInput.text;
    string password = passwordInput.text;
    
    if (!ValidateSignInInput(email, password))
        return;
        
    SignInAsync(email, password);
}

private async void SignInAsync(string email, string password)
{
    try
    {
        statusText.text = "Signing in...";
        
        AuthResult result = await auth.SignInWithEmailAndPasswordAsync(email, password);
        FirebaseUser user = result.User;
        
        Debug.Log($"User signed in: {user.Email}");
        Debug.Log($"Email verified: {user.IsEmailVerified}");
        
        if (user.IsEmailVerified)
        {
            statusText.text = "Sign in successful!";
            OnSignInSuccess(user);
        }
        else
        {
            statusText.text = "Please verify your email before signing in.";
            await SendVerificationEmail(user);
        }
    }
    catch (System.Exception ex)
    {
        Debug.LogError($"Sign in failed: {ex.Message}");
        statusText.text = $"Sign in failed: {ex.Message}";
    }
}

private bool ValidateInput(string email, string password, string confirmPassword)
{
    if (string.IsNullOrEmpty(email))
    {
        statusText.text = "Please enter an email address";
        return false;
    }
    
    if (string.IsNullOrEmpty(password))
    {
        statusText.text = "Please enter a password";
        return false;
    }
    
    if (password.Length < 6)
    {
        statusText.text = "Password must be at least 6 characters";
        return false;
    }
    
    if (password != confirmPassword)
    {
        statusText.text = "Passwords do not match";
        return false;
    }
    
    return true;
}
```

### Password Reset

```csharp
public async void SendPasswordResetEmail()
{
    string email = emailInput.text;
    
    if (string.IsNullOrEmpty(email))
    {
        statusText.text = "Please enter your email address";
        return;
    }
    
    try
    {
        await auth.SendPasswordResetEmailAsync(email);
        statusText.text = "Password reset email sent!";
        Debug.Log("Password reset email sent successfully");
    }
    catch (System.Exception ex)
    {
        Debug.LogError($"Failed to send password reset email: {ex.Message}");
        statusText.text = $"Error: {ex.Message}";
    }
}
```

---

## Phone Number Authentication

### What is Phone Number Authentication?

Phone number authentication allows users to sign in using their phone number via SMS verification. This provides a secure, convenient authentication method.

### Setup Requirements

Before implementing phone authentication, you need to:

1. **Enable Phone Authentication** in Firebase Console
2. **Add Test Phone Numbers** for development
3. **Configure SHA Keys** for your Android app
4. **Set up reCAPTCHA** for web builds

### Getting SHA Keys

```bash
# For debug keystore (development)
keytool -list -v -keystore ~/.android/debug.keystore -alias androiddebugkey -storepass android -keypass android

# For release keystore (production)
keytool -list -v -keystore your-release-key.keystore -alias your-key-alias
```

### Test Phone Numbers Setup

1. Go to Firebase Console → Authentication → Sign-in method
2. Click on Phone → Advanced → Add test phone numbers
3. Add phone numbers with custom verification codes (e.g., +1234567890 → 123456)

### Implementation

```csharp
using Firebase.Auth;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;

public class PhoneAuth : MonoBehaviour
{
    [Header("UI References")]
    public InputField phoneNumberInput;
    public InputField verificationCodeInput;
    public Button sendCodeButton;
    public Button verifyCodeButton;
    public Text statusText;
    
    private FirebaseAuth auth;
    private PhoneAuthProvider provider;
    private string verificationId;
    
    void Start()
    {
        auth = FirebaseAuth.DefaultInstance;
        provider = PhoneAuthProvider.GetInstance(auth);
        
        sendCodeButton.onClick.AddListener(SendVerificationCode);
        verifyCodeButton.onClick.AddListener(VerifyCode);
        
        // Initially disable verify button
        verifyCodeButton.interactable = false;
    }
    
    public void SendVerificationCode()
    {
        string phoneNumber = phoneNumberInput.text;
        
        if (!ValidatePhoneNumber(phoneNumber))
            return;
            
        SendCodeAsync(phoneNumber);
    }
    
    private async void SendCodeAsync(string phoneNumber)
    {
        try
        {
            statusText.text = "Sending verification code...";
            sendCodeButton.interactable = false;
            
            // Send verification code
            verificationId = await provider.VerifyPhoneNumberAsync(
                phoneNumber,
                60000, // 60 seconds timeout
                null   // No auto-verification callback for Unity
            );
            
            Debug.Log($"Verification code sent to {phoneNumber}");
            statusText.text = "Verification code sent! Please check your SMS.";
            
            // Enable verify button
            verifyCodeButton.interactable = true;
            
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Failed to send verification code: {ex.Message}");
            statusText.text = $"Error: {ex.Message}";
        }
        finally
        {
            sendCodeButton.interactable = true;
        }
    }
    
    public void VerifyCode()
    {
        string verificationCode = verificationCodeInput.text;
        
        if (string.IsNullOrEmpty(verificationCode))
        {
            statusText.text = "Please enter the verification code";
            return;
        }
        
        VerifyCodeAsync(verificationCode);
    }
    
    private async void VerifyCodeAsync(string verificationCode)
    {
        try
        {
            statusText.text = "Verifying code...";
            verifyCodeButton.interactable = false;
            
            // Create credential
            Credential credential = provider.GetCredential(verificationId, verificationCode);
            
            // Sign in with credential
            AuthResult result = await auth.SignInWithCredentialAsync(credential);
            FirebaseUser user = result.User;
            
            Debug.Log($"Phone authentication successful: {user.PhoneNumber}");
            statusText.text = "Phone verification successful!";
            
            OnSignInSuccess(user);
            
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Phone verification failed: {ex.Message}");
            statusText.text = $"Verification failed: {ex.Message}";
        }
        finally
        {
            verifyCodeButton.interactable = true;
        }
    }
    
    private bool ValidatePhoneNumber(string phoneNumber)
    {
        if (string.IsNullOrEmpty(phoneNumber))
        {
            statusText.text = "Please enter a phone number";
            return false;
        }
        
        // Basic validation - should start with + and country code
        if (!phoneNumber.StartsWith("+"))
        {
            statusText.text = "Phone number must include country code (e.g., +1234567890)";
            return false;
        }
        
        return true;
    }
    
    private void OnSignInSuccess(FirebaseUser user)
    {
        Debug.Log("Phone authentication completed successfully!");
        // Navigate to main app or update UI
    }
}
```

### Advanced Phone Auth with Country Code Selection

```csharp
[System.Serializable]
public struct CountryCode
{
    public string name;
    public string code;
    public string flag;
}

public class AdvancedPhoneAuth : MonoBehaviour
{
    [Header("Country Codes")]
    public CountryCode[] countryCodes = new CountryCode[]
    {
        new CountryCode { name = "United States", code = "+1", flag = "🇺🇸" },
        new CountryCode { name = "India", code = "+91", flag = "🇮🇳" },
        new CountryCode { name = "United Kingdom", code = "+44", flag = "🇬🇧" },
        // Add more countries as needed
    };
    
    [Header("UI References")]
    public Dropdown countryDropdown;
    public InputField phoneNumberInput;
    public Text fullPhoneNumberText;
    
    void Start()
    {
        SetupCountryDropdown();
        phoneNumberInput.onValueChanged.AddListener(UpdateFullPhoneNumber);
        countryDropdown.onValueChanged.AddListener(delegate { UpdateFullPhoneNumber(phoneNumberInput.text); });
    }
    
    private void SetupCountryDropdown()
    {
        countryDropdown.options.Clear();
        
        foreach (var country in countryCodes)
        {
            countryDropdown.options.Add(new Dropdown.OptionData($"{country.flag} {country.name} ({country.code})"));
        }
        
        countryDropdown.value = 0;
        countryDropdown.RefreshShownValue();
    }
    
    private void UpdateFullPhoneNumber(string phoneNumber)
    {
        string selectedCountryCode = countryCodes[countryDropdown.value].code;
        string fullNumber = selectedCountryCode + phoneNumber;
        fullPhoneNumberText.text = fullNumber;
    }
    
    public string GetFormattedPhoneNumber()
    {
        string selectedCountryCode = countryCodes[countryDropdown.value].code;
        return selectedCountryCode + phoneNumberInput.text;
    }
}
```

---

## Best Practices

### 1. Error Handling

```csharp
public static class AuthErrorHandler
{
    public static string GetUserFriendlyMessage(System.Exception exception)
    {
        string message = exception.Message.ToLower();
        
        if (message.Contains("network"))
            return "Please check your internet connection and try again.";
        else if (message.Contains("email") && message.Contains("already"))
            return "An account with this email already exists.";
        else if (message.Contains("password") && message.Contains("weak"))
            return "Password is too weak. Please use at least 6 characters.";
        else if (message.Contains("email") && message.Contains("invalid"))
            return "Please enter a valid email address.";
        else if (message.Contains("user") && message.Contains("not found"))
            return "No account found with this email. Please sign up first.";
        else if (message.Contains("password") && message.Contains("wrong"))
            return "Incorrect password. Please try again.";
        else
            return "An error occurred. Please try again later.";
    }
}
```

### 2. User Session Management

```csharp
public class UserSessionManager : MonoBehaviour
{
    private FirebaseAuth auth;
    
    void Start()
    {
        auth = FirebaseAuth.DefaultInstance;
        auth.StateChanged += AuthStateChanged;
        
        // Check if user is already signed in
        CheckCurrentUser();
    }
    
    private void AuthStateChanged(object sender, System.EventArgs eventArgs)
    {
        if (auth.CurrentUser != null)
        {
            Debug.Log($"User is signed in: {auth.CurrentUser.Email ?? auth.CurrentUser.UserId}");
            OnUserSignedIn(auth.CurrentUser);
        }
        else
        {
            Debug.Log("User is signed out");
            OnUserSignedOut();
        }
    }
    
    private void CheckCurrentUser()
    {
        if (auth.CurrentUser != null)
        {
            OnUserSignedIn(auth.CurrentUser);
        }
        else
        {
            OnUserSignedOut();
        }
    }
    
    private void OnUserSignedIn(FirebaseUser user)
    {
        // Load user data, show main menu, etc.
        Debug.Log("User authenticated - loading user data...");
    }
    
    private void OnUserSignedOut()
    {
        // Show login screen, clear user data, etc.
        Debug.Log("User not authenticated - showing login screen...");
    }
    
    public void SignOut()
    {
        auth.SignOut();
    }
    
    void OnDestroy()
    {
        if (auth != null)
        {
            auth.StateChanged -= AuthStateChanged;
        }
    }
}
```

### 3. Security Best Practices

- **Never store passwords** in plain text or PlayerPrefs
- **Always validate input** on both client and server side
- **Use HTTPS** for all network communications
- **Implement rate limiting** for authentication attempts
- **Verify email addresses** before allowing full account access
- **Use Firebase Security Rules** to protect your database
- **Implement proper session management** and token refresh

### 4. User Experience Tips

- **Provide clear feedback** during authentication processes
- **Show loading states** for async operations
- **Implement auto-sign-in** for returning users
- **Offer multiple authentication methods** for user convenience
- **Handle offline scenarios** gracefully
- **Provide easy account recovery** options

This comprehensive guide covers all aspects of Firebase Authentication in Unity, from basic setup to advanced implementation. Each authentication method includes complete code examples and best practices for production use.