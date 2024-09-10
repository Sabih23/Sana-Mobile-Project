// // Import statements introduce all the necessary classes for this example.
// using Facebook.Unity;
// using PlayFab;
// using PlayFab.ClientModels;
// using UnityEngine;
// using LoginResult = PlayFab.ClientModels.LoginResult;

// public class FBLogin : MonoBehaviour
// {
//     // holds the latest message to be displayed on the screen
//     private string _message;

//     public void Start()
//     {
//         SetMessage("Initializing Facebook..."); // logs the given message and displays it on the screen using OnGUI method

//         // This call is required before any other calls to the Facebook API. We pass in the callback to be invoked once initialization is finished
//         FB.Init(OnFacebookInitialized);  

//     }

//     private void OnFacebookInitialized()
//     {
//         SetMessage("Logging into Facebook...");

//         // Once Facebook SDK is initialized, if we are logged in, we log out to demonstrate the entire authentication cycle.
//         if (FB.IsLoggedIn)
//             FB.LogOut();

//         // We invoke basic login procedure and pass in the callback to process the result
//         FB.LogInWithReadPermissions(null, OnFacebookLoggedIn);
//     }

//     private void OnFacebookLoggedIn(ILoginResult result)
//     {
//         // If result has no errors, it means we have authenticated in Facebook successfully
//         if (result == null || string.IsNullOrEmpty(result.Error))
//         {
//             SetMessage("Facebook Auth Complete! Access Token: " + AccessToken.CurrentAccessToken.TokenString + "\nLogging into PlayFab...");

//             /*
//              * We proceed with making a call to PlayFab API. We pass in current Facebook AccessToken and let it create
//              * and account using CreateAccount flag set to true. We also pass the callback for Success and Failure results
//              */
//             PlayFabClientAPI.LoginWithFacebook(new LoginWithFacebookRequest { CreateAccount = true, AccessToken = AccessToken.CurrentAccessToken.TokenString},
//                 OnPlayfabFacebookAuthComplete, OnPlayfabFacebookAuthFailed);
//         }
//         else
//         {
//             // If Facebook authentication failed, we stop the cycle with the message
//             SetMessage("Facebook Auth Failed: " + result.Error + "\n" + result.RawResult, true);
//         }
//     }

//     // When processing both results, we just set the message, explaining what's going on.
//     private void OnPlayfabFacebookAuthComplete(LoginResult result)
//     {
//         SetMessage("PlayFab Facebook Auth Complete. Session ticket: " + result.SessionTicket);
//     }

//     private void OnPlayfabFacebookAuthFailed(PlayFabError error)
//     {
//         SetMessage("PlayFab Facebook Auth Failed: " + error.GenerateErrorReport(), true);
//     }

//     public void SetMessage(string message, bool error = false)
//     {
//         _message = message;
//         if (error)
//             Debug.LogError(_message);
//         else
//             Debug.Log(_message);
//     }

//     public void OnGUI()
//     {
//         var style = new GUIStyle { fontSize = 40, normal = new GUIStyleState { textColor = Color.white }, alignment = TextAnchor.MiddleCenter, wordWrap = true };
//         var area = new Rect(0,0,Screen.width,Screen.height);
//         GUI.Label(area, _message,style);
//     }
// }


using UnityEngine;
using UnityEngine.UI;
using Facebook.Unity;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine.SceneManagement;


public class FBLogin : MonoBehaviour
{
    private string _message;

    public void Start()
    {
        SetMessage("Initializing Facebook...");
        FB.Init(OnFacebookInitialized);
    }

    private void OnFacebookInitialized()
    {
        SetMessage("Logging into Facebook...");
        if (FB.IsLoggedIn)
            FB.LogOut();
    }

    public void OnFacebookLoginButtonClick()
    {
        FB.LogInWithReadPermissions(null, OnFacebookLoggedIn);
    }

    private void OnFacebookLoggedIn(ILoginResult result)
    {
        if (result == null || string.IsNullOrEmpty(result.Error))
        {
            SetMessage("Facebook Auth Complete! Access Token: " + AccessToken.CurrentAccessToken.TokenString + "\nLogging into PlayFab...");
            FB.API("/me?fields=name", HttpMethod.GET, SetUsername);

            PlayFabClientAPI.LoginWithFacebook(new LoginWithFacebookRequest
            {
                CreateAccount = true,
                AccessToken = AccessToken.CurrentAccessToken.TokenString
            },
            OnPlayfabFacebookAuthComplete, OnPlayfabFacebookAuthFailed);
        }
        else
        {
            SetMessage("Facebook Auth Failed: " + result.Error + "\n" + result.RawResult, true);
        }
    }

    private void OnPlayfabFacebookAuthComplete(PlayFab.ClientModels.LoginResult result)
    {
        SetMessage("PlayFab Facebook Auth Complete. Session ticket: " + result.SessionTicket);
        LoadMainMenu();
    }

    private void OnPlayfabFacebookAuthFailed(PlayFabError error)
    {
        SetMessage("PlayFab Facebook Auth Failed: " + error.GenerateErrorReport(), true);
    }

    public void SetMessage(string message, bool error = false)
    {
        _message = message;
        if (error)
            Debug.LogError(_message);
        else
            Debug.Log(_message);
    }

    private void LoadMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }


    void SetUsername(IResult result)
    {
        if (result.Error == null)
        {
            Debug.Log(result.RawResult);
            string name = "" + result.ResultDictionary["name"];
            string ID = "" + result.ResultDictionary["id"];
            PlayerPrefs.SetString("Username", name); //I will also need to get some ID to set for currentSkin Prefs
            PlayerPrefs.SetString("PlayerID", ID);
            Debug.Log("" + name);
        }
        else
        {
            Debug.Log(result.Error);
        }
    }

    // public void OnGUI()
    // {
    //     var style = new GUIStyle { fontSize = 40, normal = new GUIStyleState { textColor = Color.white }, alignment = TextAnchor.MiddleCenter, wordWrap = true };
    //     var area = new Rect(0, 0, Screen.width, Screen.height);
    //     GUI.Label(area, _message, style);
    // }
}
