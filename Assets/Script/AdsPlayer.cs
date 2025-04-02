using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Video;
using TMPro;
using UnityEngine.UI;
public class AdsPlayer : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] VideoPlayer videoPlayer;
    [SerializeField] float countdownTime = 5f;
    [SerializeField] TextMeshProUGUI countdownText;
    [SerializeField] Button skipButton;
    [SerializeField] Sprite unskipSprite, skipSprite;
    [SerializeField] CurrencyType currencyType;
    void Start()
    {
        if (skipButton){
            skipButton.interactable = false;
            skipButton.image.sprite = unskipSprite;
            skipButton.onClick.AddListener(() =>
            {
                videoPlayer.Stop();
                skipButton.interactable = false;
                skipButton.image.sprite = unskipSprite;
                countdownText.text = ""+countdownTime;
                GameManager.instance.lobbyController.AddCurrency((int)currencyType);
                GameManager.instance.uiController.CloseAdsPanel();
            });
        } 
    }

    
    void OnVideoEnd(VideoPlayer vp)
    {
        Debug.Log("Video finished playing!");
        countdownText.text = "Close";
    }
    public void ShowAds()
    {
        videoPlayer.Play();
        videoPlayer.loopPointReached += OnVideoEnd;
        StartCoroutine(Countdown(() =>
        {
            countdownText.text = "Skip...";
            skipButton.interactable = true;
            skipButton.image.sprite = skipSprite;
        }));
    }
    private IEnumerator Countdown(Action action)
    {
        float _countdownTime = countdownTime;
        while (_countdownTime > 0)
        {
            countdownText.text = Mathf.Ceil(_countdownTime).ToString();
            _countdownTime -= 1f;

            yield return new WaitForSecondsRealtime(1f);
        }
        if (action != null)
        {
            action.Invoke();
        }
    }
}
