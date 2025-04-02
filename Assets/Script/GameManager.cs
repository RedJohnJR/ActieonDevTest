using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using Newtonsoft.Json;
using System;

public class GameManager : MonoBehaviour
{
    [SerializeField] private string serverURL = "";
    public static GameManager instance { get; private set; }
    public UIController uiController;
    public LobbyController lobbyController;
    [HideInInspector] public string userName;
    [HideInInspector] public int diamonds;
    [HideInInspector] public int hearts;
    [HideInInspector] public bool onMessageBox = false;
    [HideInInspector] public bool onAdsPanel = false;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    public void SignUp(string user, string pass, Action action)
    {
        StartCoroutine(SignUpRequest(user, pass, action));
    }

    IEnumerator SignUpRequest(string user, string pass, Action action)
    {
        WWWForm form = new WWWForm();
        form.AddField("username", user);
        form.AddField("password", pass);

        if (String.IsNullOrEmpty(serverURL))
        {
            uiController.OpenMessageBox("Server URL is not set.");
            Debug.LogError("Server URL is not set.");
            yield return null;

        }

        using (UnityWebRequest www = UnityWebRequest.Post(serverURL + "SignUp.php", form))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("SignUp Response: " + www.downloadHandler.text);

                SignUpResponse response = JsonConvert.DeserializeObject<SignUpResponse>(www.downloadHandler.text);

                if (response.status == "success")
                {
                    Debug.Log($"SignUp Successful: {response.message}");

                    if (action != null)
                    {
                        action.Invoke();
                    }
                }
                else
                {
                    Debug.LogError($"SignUp Failed: {response.message}");
                    uiController.OpenMessageBox("SignUp failed: " + response.message);
                }
            }
            else
            {
                Debug.LogError("SignUp Request Failed: " + www.error);
                uiController.OpenMessageBox("SignUp failed. Please try again.");
            }
        }
    }

    public void Login(string user, string pass, Action action)
    {
        StartCoroutine(LoginRequest(user, pass, action));
    }

    IEnumerator LoginRequest(string user, string pass, Action action)
    {
        WWWForm form = new WWWForm();
        form.AddField("username", user);
        form.AddField("password", pass);

        if (String.IsNullOrEmpty(serverURL))
        {
            uiController.OpenMessageBox("Server URL is not set.");
            Debug.LogError("Server URL is not set.");
            yield return null;
        }

        using (UnityWebRequest www = UnityWebRequest.Post(serverURL + "Login.php", form))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Login Success: " + www.downloadHandler.text);

                try
                {
                    string jsonResponse = www.downloadHandler.text;
                    UserData data = JsonConvert.DeserializeObject<UserData>(jsonResponse);

                    if (data.status == "error")
                    {
                        Debug.LogError("Login Failed: " + data.message);
                        uiController.OpenMessageBox(data.message);
                        yield break;
                    }

                    userName = data.username;
                    diamonds = data.diamond;
                    hearts = data.heart;

                    Debug.Log($"User: {userName}, Diamonds: {diamonds}, Hearts: {hearts}");

                    action?.Invoke();
                }
                catch (JsonException e)
                {
                    Debug.LogError("Error deserializing response: " + e.Message);
                    uiController.OpenMessageBox("Error processing login response.");
                }
            }
            else
            {
                Debug.LogError("Login Failed: " + www.error);
                uiController.OpenMessageBox("Login failed." + www.error);
            }
        }
    }

    public void UpdateData(int newDiamonds, int newHearts)
    {
        StartCoroutine(UpdateDataRequest(newDiamonds, newHearts));
    }

    IEnumerator UpdateDataRequest(int newDiamonds, int newHearts)
    {
        WWWForm form = new WWWForm();
        form.AddField("username", userName);
        form.AddField("diamond", newDiamonds);
        form.AddField("heart", newHearts);

        if (String.IsNullOrEmpty(serverURL))
        {
            uiController.OpenMessageBox("Server URL is not set.");
            Debug.LogError("Server URL is not set.");
            yield return null;
        }

        using (UnityWebRequest www = UnityWebRequest.Post(serverURL + "UpdateData.php", form))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Update Success: " + www.downloadHandler.text);
                diamonds = newDiamonds;
                hearts = newHearts;
            }
            else
            {
                Debug.LogError("Update Failed: " + www.error);
            }
        }
    }
    public void DelayAction(float delay, Action action)
    {
        StartCoroutine(Wait(delay, action));
    }
    IEnumerator Wait(float delay, Action action)
    {
        yield return new WaitForSeconds(delay);
        if (action != null)
        {
            action.Invoke();
        }
    }
}

[System.Serializable]
public class UserData
{
    public string username;
    public int diamond;
    public int heart;
    public string status;
    public string message;
}
class SignUpResponse
{
    public string status;
    public string message;
}
