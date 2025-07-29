using UnityEngine;
using UnityEngine.UI;
using TMPro; // TMProを追加

public class SliderFeedback : MonoBehaviour
{
    public Slider slider;
    public TextMeshProUGUI feedbackText; // TMProに変更

    void Start()
    {
        if (slider != null)
        {
            slider.onValueChanged.AddListener(UpdateFeedback);
            UpdateFeedback(slider.value); // 初期値を反映
        }
    }

    void UpdateFeedback(float value)
    {
        // 0 を 1 に、1 を -1 にマッピング
        float mappedValue = 1 - 2 * value;

        // UIに表示（オプション）
        if (feedbackText != null)
        {
            feedbackText.text = mappedValue.ToString("F2");
        }
    }

    void OnDestroy()
    {
        if (slider != null)
        {
            slider.onValueChanged.RemoveListener(UpdateFeedback);
        }
    }
}
