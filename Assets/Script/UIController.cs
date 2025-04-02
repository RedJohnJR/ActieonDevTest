using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using TMPro;
using System;
using System.Text.RegularExpressions;
public class UIController : MonoBehaviour
{
    [SerializeField] private GameObject loginPanel;
    [SerializeField] private GameObject signUpPanel;
    [SerializeField] private GameObject lobbyPanel;
    [SerializeField] private GameObject adsPanel;
    [SerializeField] private GameObject msgBoxPanel;
    TextMeshProUGUI passwordStrengthText;
    TextMeshProUGUI confirmPasswordStrengthText;
    TMP_InputField loginUsername;
    TMP_InputField loginPassword;
    TMP_InputField signupUsername;
    TMP_InputField signupPassword;
    TMP_InputField signupConfirmPassword;



    void Awake()
    {
        if (loginPanel == null) loginPanel = GameObject.Find("LoginPanel");
        if (signUpPanel == null) signUpPanel = GameObject.Find("SignUpPanel");
        if (lobbyPanel == null) lobbyPanel = GameObject.Find("LobbyPanel");
        if (adsPanel == null) adsPanel = GameObject.Find("AdsPanel");
        if (msgBoxPanel == null) msgBoxPanel = GameObject.Find("MessageBox");

        if (loginPanel) loginPanel.SetActive(true);
        if (signUpPanel) signUpPanel.SetActive(false);
        if (lobbyPanel) lobbyPanel.SetActive(false);
        if (adsPanel) adsPanel.SetActive(false);
        if (msgBoxPanel) msgBoxPanel.SetActive(false);

        loginUsername = loginPanel.transform.Find("Panel/Username").GetComponent<TMP_InputField>();
        loginPassword = loginPanel.transform.Find("Panel/Password").GetComponent<TMP_InputField>();
        signupUsername = signUpPanel.transform.Find("Panel/Username").GetComponent<TMP_InputField>();
        signupPassword = signUpPanel.transform.Find("Panel/Password").GetComponent<TMP_InputField>();
        signupConfirmPassword = signUpPanel.transform.Find("Panel/ConfirmPass").GetComponent<TMP_InputField>();
        passwordStrengthText = signUpPanel.transform.Find("Panel/Password/PasswordStrength").GetComponent<TextMeshProUGUI>();
        confirmPasswordStrengthText = signUpPanel.transform.Find("Panel/ConfirmPass/ConfirmPassStrength").GetComponent<TextMeshProUGUI>();

        signupPassword.onValueChanged.AddListener((value) => CheckPassword(value, passwordStrengthText));
        signupConfirmPassword.onValueChanged.AddListener((value) => CheckPassword(value, confirmPasswordStrengthText));
    }

    public void CheckPassword(string password, TextMeshProUGUI resultText)
    {
        string strength = GetPasswordStrength(password);
        resultText.text = "Strength: " + strength;

        switch (strength)
        {
            case "Weak":
                resultText.color = Color.red;
                break;
            case "Medium":
                resultText.color = Color.yellow;
                break;
            case "Strong":
                resultText.color = Color.green;
                break;
        }
    }

    private string GetPasswordStrength(string password)
    {
        if (password.Length < 6)
            return "Weak";

        bool hasLower = Regex.IsMatch(password, "[a-z]");
        bool hasUpper = Regex.IsMatch(password, "[A-Z]");
        bool hasDigit = Regex.IsMatch(password, "[0-9]");
        bool hasSpecial = Regex.IsMatch(password, "[!@#$%^&*(),.?\":{}|<>]");

        int score = 0;

        if (hasLower)
            score++;

        if (hasUpper)
            score++;

        if (hasDigit)
            score++;

        if (hasSpecial)
            score++;

        if (password.Length >= 8 && score >= 3)
            return "Strong";

        return "Medium";
    }
    void ShowPanel(GameObject panel, bool doScale = true)
    {
        if (panel == null) return;

        panel.SetActive(true);

        if (!doScale) return;
        panel.transform.localScale = Vector3.zero;
        panel.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack);
    }
    void HidePanel(GameObject panel, Action action = null)
    {
        if (panel == null) return;

        panel.transform.DOScale(Vector3.zero, 0.25f).SetEase(Ease.InBack).OnComplete(() =>
        {
            panel.SetActive(false);
            if (action != null)
            {
                action.Invoke();
            }
        });
    }

    public void LoginButtonClick()
    {
        if (GameManager.instance.onMessageBox) return;

        Debug.Log("Login button clicked!");

        if (loginPanel.activeSelf)
        {
            string username = loginUsername.text;
            string password = loginPassword.text;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                Debug.Log("Username or password is empty!");
                OpenMessageBox("Username or password is empty!");
                return;
            }

            GameManager.instance.Login(username, password, () =>
            {
                HidePanel(loginPanel);
                ShowPanel(lobbyPanel, false);
                GameManager.instance.lobbyController.RefreshData();
            });
        }
        else
        {
            ShowPanel(loginPanel);
        }
    }
    public void SignUpButtonClick()
    {
        if (GameManager.instance.onMessageBox) return;

        Debug.Log("Register button clicked!");
        if (signUpPanel.activeSelf)
        {
            string username = signupUsername.text;
            string password = signupPassword.text;
            string confirmPassword = signupConfirmPassword.text;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(confirmPassword))
            {
                Debug.Log("Username or password is empty!");
                OpenMessageBox("Username or password is empty!");
                return;
            }

            if (password != confirmPassword)
            {
                Debug.Log("Passwords do not match!");
                OpenMessageBox("Passwords do not match!");
                return;
            }

            GameManager.instance.SignUp(username, password, () =>
            {
                GameManager.instance.Login(username, password, () =>
                {
                    HidePanel(loginPanel);
                    HidePanel(signUpPanel);
                    ShowPanel(lobbyPanel, false);
                    GameManager.instance.lobbyController.RefreshData();

                });
            });
        }
        else
        {
            ShowPanel(signUpPanel);
        }
    }
    public void CloseSignUpPanel()
    {
        if (GameManager.instance.onMessageBox) return;

        HidePanel(signUpPanel, () =>
        {
            ClearInputFields();
        });
    }
    public void CloseMessageBox()
    {
        HidePanel(msgBoxPanel);
        GameManager.instance.onMessageBox = false;
    }
    public void OpenMessageBox(string message)
    {
        if(GameManager.instance.onAdsPanel) return;

        if (msgBoxPanel.activeSelf) return;
        ShowPanel(msgBoxPanel);
        msgBoxPanel.transform.Find("Panel/Message").GetComponent<TextMeshProUGUI>().text = message;
        GameManager.instance.onMessageBox = true;
    }
    public bool OpenAdsPanel()
    {
        if (GameManager.instance.onMessageBox) return false;
        if (adsPanel.activeSelf) return false;

        ShowPanel(adsPanel);
        adsPanel.GetComponent<AdsPlayer>().ShowAds();
        GameManager.instance.onAdsPanel = true;
        

        return true;
    }
    public void CloseAdsPanel()
    {
        if (GameManager.instance.onMessageBox) return;
        
        HidePanel(adsPanel);
        GameManager.instance.onAdsPanel = false;

    }
    public void ClearInputFields()
    {
        loginUsername.text = string.Empty;
        loginPassword.text = string.Empty;
        signupUsername.text = string.Empty;
        signupPassword.text = string.Empty;
        signupConfirmPassword.text = string.Empty;
    }
}
