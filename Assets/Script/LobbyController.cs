using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Data;
using System;

public enum CurrencyType
{
    Diamonds,
    Hearts
}
public class LobbyController : MonoBehaviour
{
    [Header("Diamonds Properties")]
    [SerializeField] private int addDiamondAmount = 100;
    [SerializeField] private TextMeshProUGUI diamondText;
    private float currentDiamonds = 0;

    [Header("Hearts Properties")]
    [SerializeField] private Slider heartSlider;
    private int currentHearts = 0;
    [SerializeField] private int maxHearts = 5;
    [SerializeField] private int decreaseHeartsAmount = 5;
    [SerializeField] private int increaseHeartsAmount = 5;


    void Start()
    {
        UpdateDiamondText();
        UpdateHeartUI();
    }
    public void RefreshData()
    {

        currentDiamonds = GameManager.instance.diamonds;
        currentHearts = GameManager.instance.hearts;

        UpdateDiamondText();
        UpdateHeartUI();
    }
    public void AddCurrency(int type)
    {
        CurrencyType currencyType = (CurrencyType)type;
        switch (currencyType)
        {
            case CurrencyType.Diamonds:
                AddDiamonds();
                break;
            case CurrencyType.Hearts:
                AddHeart();
                break;
        }
    }
    public void PlayAds()
    {
        if (GameManager.instance.uiController.OpenAdsPanel())
        {
            UseHeart();
        }
    }


    public void AddDiamonds()
    {
        if (GameManager.instance.onAdsPanel) return;
        if (GameManager.instance.onMessageBox) return;

        if (currentDiamonds < 10000)
        {
            currentDiamonds += addDiamondAmount;

            if(currentDiamonds > 10000)
            {
                currentDiamonds = 10000;
            }

            UpdateDiamondText();
            GameManager.instance.UpdateData(Convert.ToInt32(currentDiamonds), currentHearts);
        }
        else
        {
            GameManager.instance.uiController.OpenMessageBox("You have enough diamonds!");
        }
    }

    private void UpdateDiamondText()
    {
        if (currentDiamonds >= 100000)
        {
            if (currentDiamonds < 1000000)
            {
                float thousands = currentDiamonds / 1000f;
                diamondText.text = thousands.ToString("0.##") + "K";
            }
            else if (currentDiamonds < 1000000000)
            {
                float millions = currentDiamonds / 1000000f;
                diamondText.text = millions.ToString("0.##") + "M";
            }
            else if (currentDiamonds < 1000000000000)
            {
                float billions = currentDiamonds / 1000000000f;
                diamondText.text = billions.ToString("0.##") + "B";
            }
            else
            {
                float trillions = currentDiamonds / 1000000000000f;
                diamondText.text = trillions.ToString("0.##") + "T";
            }
        }
        else
        {
            diamondText.text = currentDiamonds.ToString();
        }
    }

    public void AddHeart()
    {
        if (currentHearts < maxHearts)
        {
            currentHearts += increaseHeartsAmount;
            if (currentHearts > maxHearts)
            {
                currentHearts = maxHearts;
            }
            UpdateHeartUI();
            GameManager.instance.UpdateData(Convert.ToInt32(currentDiamonds), currentHearts);

        }
        else
        {
            GameManager.instance.uiController.OpenMessageBox("You have enough hearts!");
        }
    }

    public void UseHeart()
    {
        if (currentHearts > 0)
        {
            currentHearts -= decreaseHeartsAmount;
            if(currentHearts < 0)
            {
                currentHearts = 0;
            }
            UpdateHeartUI();
            GameManager.instance.UpdateData(Convert.ToInt32(currentDiamonds), currentHearts);

        }
        else
        {
            GameManager.instance.uiController.OpenMessageBox("You don't have enough hearts!");
        }

    }

    private void UpdateHeartUI()
    {
        heartSlider.value = (float)currentHearts;
    }
}