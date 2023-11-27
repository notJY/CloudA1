using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using TMPro;
using UnityEngine.SceneManagement;

public class PFUserMgt : MonoBehaviour
{
    [SerializeField] TMP_Text msgBox;
    [SerializeField] TMP_InputField if_username, if_password, if_email;

    private string playfabID, displayName;

    public void OnButtonRegUser()
    {
        var regReq = new RegisterPlayFabUserRequest
        {
            Email = if_email.text,
            Password = if_password.text,
            Username = if_username.text,
            DisplayName = if_username.text
        };
        PlayFabClientAPI.RegisterPlayFabUser(regReq, OnRegSucc, OnError);
    }

    void OnRegSucc(RegisterPlayFabUserResult r)
    {
        msgBox.text = "Success! " + r.PlayFabId;
    }

    void OnError(PlayFabError e)
    {
        msgBox.text = "Error " + e.GenerateErrorReport();
    }

    public void OnButtonLogin()
    {
        Debug.Log(if_username.text);
        if (if_username.text != "")
        {
            var loginReq = new LoginWithPlayFabRequest
            {
                Username = if_username.text,
                Password = if_password.text
            };
            PlayFabClientAPI.LoginWithPlayFab(loginReq, OnLoginSucc, OnError);
        }
        else
        {
            var loginReq = new LoginWithEmailAddressRequest
            {
                Email = if_email.text,
                Password = if_password.text
            };
            PlayFabClientAPI.LoginWithEmailAddress(loginReq, OnLoginSucc, OnError);
        }
    }

    string GetPlayerID()
    {
        var accReq = new GetAccountInfoRequest();
        PlayFabClientAPI.GetAccountInfo(accReq, r => { playfabID = r.AccountInfo.PlayFabId; }, OnError);
        return playfabID;
    }

    void GetPlayerDisplayName()
    {

        var profileReq = new GetPlayerProfileRequest
        {
            PlayFabId = GetPlayerID()
        };
        PlayFabClientAPI.GetPlayerProfile(profileReq, r => { displayName = r.PlayerProfile.DisplayName; }, OnError);
    }

    void updateDisplayName()
    {
        var updateNameReq = new UpdateUserTitleDisplayNameRequest
        {
            DisplayName = "Guest"
        };
        PlayFabClientAPI.UpdateUserTitleDisplayName(updateNameReq, OnDisplayNameUpdate, OnError);
    }

    void OnDisplayNameUpdate(UpdateUserTitleDisplayNameResult r)
    {
        Debug.Log("Display name updated: " + r.DisplayName);
    }

    void OnLoginSucc(LoginResult r)
    {
        GetPlayerDisplayName();
        //If no display name, set display name to "Guest"
        if ((displayName == null) || (displayName == ""))
        {
            updateDisplayName();
        }

        msgBox.text = "Success " + r.PlayFabId;
        SceneManager.LoadScene("Menu");
    }

    public void OnResetPassword()
    { 
        var resetReq = new SendAccountRecoveryEmailRequest
        {
            Email = if_email.text,
            TitleId = PlayFabSettings.TitleId
        };
        PlayFabClientAPI.SendAccountRecoveryEmail(resetReq, OnResetSucc, OnError);
    }

    void OnResetSucc(SendAccountRecoveryEmailResult r)
    {
        msgBox.text = "Email sent ";
    }

    public void OnGuestLogin()
    {
        var guestReq = new LoginWithCustomIDRequest
        {
            CreateAccount = true,
            CustomId = PlayFabSettings.DeviceUniqueIdentifier,
        };
        PlayFabClientAPI.LoginWithCustomID(guestReq, OnLoginSucc, OnError);
    }
}
