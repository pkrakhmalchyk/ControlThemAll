using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using LitMotion;
using LitMotion.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ControllThemAll.Runtime.Gameplay.UI
{
    public class RadialSelectionMenuUIView : MonoBehaviour
    {
        public Action<string> Selected;
        public Action Opened;
        public Action Closed;


        [SerializeField] private RadialSelectionMenuUIElement elementPrefab;
        [SerializeField] private RectTransform elementsContainer;
        [SerializeField] private Button activeElementButton;
        [SerializeField] private EventTrigger activeElementButtonEventTrigger;
        [SerializeField] private TMP_Text activeElementButtonCountText;
        [SerializeField] private Image activeElementButtonIcon;
        [SerializeField] private Color elementHoverColor = Color.yellow;
        [SerializeField] private Color elementBaseColor = Color.white;
        [Range(0f, 360f)]
        [SerializeField] private float menuRadius = 180f;

        private readonly List<RadialSelectionMenuUIElement> elements = new();
        private RadialSelectionMenuUIElement activeElement;
        private RectTransform activeElementButtonRectTransform;
        private EventTrigger.Entry pointerDownEvent;
        private EventTrigger.Entry pointerUpEvent;
        private CancellationTokenSource elementSelectionCts;
        private ITouchInput input;
        private MotionHandle showHideHandle;

        private float segmentAngle;
        private float segmentFillAmount;


        public void Initialize(ITouchInput input)
        {
            this.input = input;
        }


        private void Awake()
        {
            activeElementButtonRectTransform = activeElementButton.GetComponent<RectTransform>();
            pointerDownEvent = new EventTrigger.Entry
            {
                eventID = EventTriggerType.PointerDown
            };
            pointerUpEvent = new EventTrigger.Entry
            {
                eventID = EventTriggerType.PointerUp
            };

            pointerDownEvent.callback.AddListener((eventData) => OnActiveElementButtonDown());
            pointerUpEvent.callback.AddListener((eventData) => OnActiveElementButtonUp());
            elementsContainer.gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            activeElementButtonEventTrigger.triggers.Add(pointerDownEvent);
            activeElementButtonEventTrigger.triggers.Add(pointerUpEvent);
        }

        private void OnDisable()
        {
            activeElementButtonEventTrigger.triggers.Remove(pointerDownEvent);
            activeElementButtonEventTrigger.triggers.Remove(pointerUpEvent);
        }


        public void AddRadialSelectionMenuUIElement(string key, Sprite icon, int count)
        {
            RadialSelectionMenuUIElement item = Instantiate(elementPrefab, elementsContainer.transform, false);

            item.SetColor(elementBaseColor);
            item.SetKey(key);
            item.SetIcon(icon);
            item.SetCount(count);

            elements.Add(item);

            UpdateElementsSize();
        }

        public void RemoveRadialSelectionMenuUIElement(string key)
        {
            RadialSelectionMenuUIElement element = elements.Find(existingElement => existingElement.Key == key);

            if (element == null)
            {
                return;
            }

            if (element.Key == activeElement.Key)
            {
                activeElement = null;

                UpdateActiveElementButton();
            }

            elements.Remove(element);
            Destroy(element.gameObject);
            UpdateElementsSize();
        }

        public void RemoveAllRadialSelectionMenuUIElements()
        {
            foreach (RadialSelectionMenuUIElement element in elements)
            {
                Destroy(element.gameObject);
            }

            elements.Clear();

            activeElement = null;
            UpdateActiveElementButton();
        }

        public void SetRadialSelectionMenuUIElementCount(string key, int count)
        {
            RadialSelectionMenuUIElement element = elements.Find(existingElement => existingElement.Key == key);

            if (element == null)
            {
                return;
            }

            element.SetCount(count);
            UpdateActiveElementButton();
        }

        public void SetRadialSelectionMenuUIActiveElement(string key)
        {
            activeElement = elements.Find(existingElement => existingElement.Key == key);

            UpdateActiveElementButton();
        }


        private void OnActiveElementButtonDown()
        {
            if (elementSelectionCts != null)
            {
                return;
            }

            elementSelectionCts = new CancellationTokenSource();

            SelectNewElement(elementSelectionCts.Token).Forget();
        }

        private void OnActiveElementButtonUp()
        {
            elementSelectionCts?.Cancel();
            elementSelectionCts?.Dispose();

            elementSelectionCts = null;
        }

        private bool IsOnActiveElementButton()
        {
            if (activeElementButtonRectTransform == null)
            {
                return false;
            }

            Vector2 localMousePosition = activeElementButtonRectTransform.InverseTransformPoint(input.Position);

            return activeElementButtonRectTransform.rect.Contains(localMousePosition);
        }

        private async UniTaskVoid SelectNewElement(CancellationToken token)
        {
            if (showHideHandle.IsActive())
            {
                return;
            }

            Opened?.Invoke();

            elementsContainer.gameObject.SetActive(true);

            Vector3 scale = elementsContainer.transform.localScale;
            showHideHandle = LMotion.Create(Vector3.zero, elementsContainer.transform.localScale, 0.15f)
                .WithEase(Ease.Linear)
                .BindToLocalScale(elementsContainer.transform); ;

            RadialSelectionMenuUIElement selectedElement = await GetSelectedElement(token);

            UpdateElementsColor();

            showHideHandle.TryCancel();

            showHideHandle = LMotion.Create(elementsContainer.transform.localScale, Vector3.zero, 0.15f)
                .WithEase(Ease.Linear)
                .BindToLocalScale(elementsContainer.transform);

            if (selectedElement != null)
            {
                activeElement = selectedElement;

                UpdateActiveElementButton();
                Selected?.Invoke(selectedElement.Key);
            }

            await showHideHandle;

            elementsContainer.gameObject.SetActive(false);
            elementsContainer.localScale = scale;

            Closed?.Invoke();
        }

        private async UniTask<RadialSelectionMenuUIElement> GetSelectedElement(CancellationToken token)
        {
            RadialSelectionMenuUIElement selectedElement = null;

            while (!token.IsCancellationRequested)
            {
                Vector2 direction = input.Position - elementsContainer.position;
                float angle = Vector2.SignedAngle(direction, Vector2.left);
                angle = angle < 0 ? 360 + angle : angle;

                if (elements.Count == 0 || angle < 0 || angle > menuRadius || IsOnActiveElementButton())
                {
                    selectedElement = null;
                }
                else
                {
                    int selectedElementIndex = Mathf.Clamp(Mathf.FloorToInt(angle / segmentAngle), 0, elements.Count - 1);
                    selectedElement = elements[selectedElementIndex];
                }

                UpdateElementsColor(selectedElement);

                await UniTask.Yield();
            }

            return selectedElement;
        }

        private void UpdateActiveElementButton()
        {
            if (activeElement != null)
            {
                activeElementButtonCountText.text = activeElement.CountText;
                activeElementButtonIcon.sprite = activeElement.Icon;
                activeElementButtonIcon.gameObject.SetActive(true);
            }
            else
            {
                activeElementButtonCountText.text = string.Empty;
                activeElementButtonIcon.sprite = null;
                activeElementButtonIcon.gameObject.SetActive(false);
            }
        }

        private void UpdateElementsColor(RadialSelectionMenuUIElement selectedElement = null)
        {
            foreach (RadialSelectionMenuUIElement element in elements)
            {
                element.SetColor(selectedElement?.Key == element.Key ? elementHoverColor : elementBaseColor);
            }
        }

        private void UpdateElementsSize()
        {
            int elementsCount = elements.Count;
            segmentAngle = menuRadius / elementsCount;
            segmentFillAmount = segmentAngle / 360f;

            for (int i = 0; i < elementsCount; i++)
            {
                elements[i].SetFillAmount(segmentFillAmount);
                elements[i].SetRotation(-segmentAngle * i);
            }
        }
    }
}