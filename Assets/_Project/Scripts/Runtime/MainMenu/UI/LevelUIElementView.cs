using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ControllThemAll.Runtime.MainMenu.UI
{
    public class LevelUIElementView : MonoBehaviour
    {
        public Action<LevelUIElementView> Selected;

        [SerializeField] private Sprite openLevelSprite;
        [SerializeField] private Sprite closedLevelSprite;
        [SerializeField] private TMP_Text levelNameText;
        [SerializeField] private Button button;
        [SerializeField] private Image levelButtonImage;

        private string key;


        public string Key => key;


        private void OnEnable()
        {
            button.onClick.AddListener(OnButtonClick);
        }

        private void OnDisable()
        {
            button.onClick.RemoveListener(OnButtonClick);
        }


        public void SetKey(string key)
        {
            this.key = key;
        }

        public void SetLevelText(string levelName)
        {
            levelNameText.text = levelName;
        }

        public void SetLevelOpen(bool open)
        {
            levelButtonImage.sprite = open ? openLevelSprite : closedLevelSprite;
            button.interactable = open;
        }


        private void OnButtonClick()
        {
            Selected?.Invoke(this);
        }
    }
}