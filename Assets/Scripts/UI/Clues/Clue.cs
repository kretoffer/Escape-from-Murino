using UnityEngine;
using UnityEngine.UI;

public class Clue : MonoBehaviour
{
    [SerializeField] private Image _clueButtonImage;
    [SerializeField] private RectTransform _clueButtonTransform;
    [SerializeField] private Text _text;

    public void Initialize(string text, Sprite sprite)
    {
        float width = (float)(sprite.rect.width * 7.5);
        _clueButtonTransform.sizeDelta = new Vector2(width, _clueButtonTransform.sizeDelta.y);
        if (width > 120)
        {
            Vector2 pos = _clueButtonTransform.anchoredPosition;
            pos.x = 150 - width;
            _clueButtonTransform.anchoredPosition = pos;
        }
        else
        {
            Vector2 pos = _clueButtonTransform.anchoredPosition;
            pos.x = 30;
            _clueButtonTransform.anchoredPosition = pos;
        }
        _clueButtonImage.sprite = sprite;
        _text.text = text;
    }
}
