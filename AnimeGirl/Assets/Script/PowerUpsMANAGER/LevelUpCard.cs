using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelUpCard : MonoBehaviour
{
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI descriptionText;
    public TextMeshProUGUI countText;    // optional small text to show duplicate count
    public Image iconImage;
    public Button button;

    private Action onClick;

    public void Setup(string title, string description, Sprite icon, int count, Action onClickCallback)
    {
        if (titleText != null) titleText.text = title;
        if (descriptionText != null) descriptionText.text = description;
        if (countText != null)
        {
            countText.gameObject.SetActive(count > 1);
            countText.text = $"x{count}";
        }

        if (iconImage != null) iconImage.sprite = icon;

        onClick = onClickCallback;
        if (button != null)
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => onClick?.Invoke());
        }
    }
}
