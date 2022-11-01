using UnityEngine;

namespace Nagih
{
    public class TutorialArrow : MonoBehaviour
    {
#pragma warning disable CS0649
        [SerializeField] private RectTransform Rect;
        [SerializeField] private Vector2 Offset;
#pragma warning restore CS0649

        private RectTransform _canvas;

        public void Show(RectTransform target)
        {
            Show(target, Direction.Bottom);
        }

        public void Show(RectTransform target, Direction direction)
        {
            float multiX = 0.5f - target.pivot.x;
            float offsetX = target.rect.width * multiX + Offset.x;

            float multiY = direction == Direction.Top ? -target.pivot.y : (1 - target.pivot.y);
            float offsetY = target.rect.height * multiY + (Offset.y * multiY);

            Rect.anchorMin = target.anchorMin;
            Rect.anchorMax = target.anchorMax;
            Rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, target.rect.width);
            Rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, target.rect.height);
            Rect.anchoredPosition = new Vector2(target.anchoredPosition.x + offsetX,
                                                target.anchoredPosition.y + offsetY);

            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public enum Direction
        {
            Bottom = 0,
            Top = 1
        }

        public RectTransform Canvas
        {
            get
            {
                if (_canvas == null)
                {
                    _canvas = Manager.GetInstance().Tutorial.Parent.GetComponent<RectTransform>();
                }
                return _canvas;
            }
        }
    }
}