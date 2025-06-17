using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ControllThemAll.Runtime.Gameplay.UI
{
    public class RadialSelectionMenuUIElement : MonoBehaviour
    {
        [SerializeField] private Image backgroundImage;
        [SerializeField] private Image iconImage;
        [SerializeField] private float iconMaxSize = 100f;
        [SerializeField] private float iconMinSize = 75f;
        [SerializeField] private TMP_Text countText;
        [SerializeField] private float countTextMaxSize = 20f;
        [SerializeField] private float countTextMinSize = 15f;
        [SerializeField] private RectTransform contentRect;
        [Range(0f, 1f)]
        [SerializeField] private float contentDistance;


        private string key;


        public string Key => key;
        public Sprite Icon => iconImage.sprite;
        public string CountText => countText.text;


        private void Awake()
        {
            backgroundImage.type = Image.Type.Filled;
            backgroundImage.fillMethod = Image.FillMethod.Radial360;
            backgroundImage.fillOrigin = (int)Image.Origin360.Left;

            AdjustContentPosition();
        }


        public void SetKey(string key)
        {
            this.key = key;
        }

        public void SetIcon(Sprite icon)
        {
            iconImage.sprite = icon;
        }

        public void SetCount(int count)
        {
            countText.text = count.ToString();
        }

        public void SetColor(Color color)
        {
            backgroundImage.color = color;
        }

        public void SetFillAmount(float fillAmount)
        {
            fillAmount = Mathf.Clamp(fillAmount, 0f, 1f);

            backgroundImage.fillAmount = fillAmount;

            AdjustContentPosition();
            AdjustContentSize();
        }

        public void SetRotation(float angle)
        {
            backgroundImage.rectTransform.rotation = Quaternion.Euler(0f, 0f, angle);

            AdjustContentPosition();
        }


        private void AdjustContentPosition()
        {
            float fillAmount = backgroundImage.fillAmount;
            float elementAngle = 360f * fillAmount;
            float centerAngle = elementAngle / 2;

            Vector3 direction = Quaternion.AngleAxis(-centerAngle, Vector3.forward) * backgroundImage.rectTransform.rotation * Vector3.left;
            Vector3 position = direction * (backgroundImage.rectTransform.rect.width * backgroundImage.rectTransform.lossyScale.x * contentDistance / 2);

            contentRect.position = backgroundImage.rectTransform.position + position;
        }

        private void AdjustContentSize()
        {
            float fillAmount = backgroundImage.fillAmount;
            float iconSize = Mathf.Lerp(iconMinSize, iconMaxSize, fillAmount);
            float countTextSize = Mathf.Lerp(countTextMinSize, countTextMaxSize, fillAmount);

            iconImage.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, iconSize);
            iconImage.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, iconSize);
            countText.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, countTextSize);
            countText.rectTransform.anchoredPosition = new Vector3(0, -iconSize / 2 - countTextSize);
        }
    }
}