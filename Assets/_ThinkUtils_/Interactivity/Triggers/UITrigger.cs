//用于检测UI物体事件触发的脚本

//By NeilianAryan

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace ThinkUtils
{
    [RequireComponent(typeof(RectTransform))]
    public class UITrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler, IDragHandler
    {
        public UnityEvent onEnter, onOver, onExit, onDown, onUp, onClick, onDrag;
        bool isHover, isHold;
        /// <summary>
        /// 是否处于鼠标悬浮状态
        /// </summary>
        public bool IsHover { get { return isHover; } }
        /// <summary>
        /// 是否处于鼠标按住状态
        /// </summary>
        public bool IsHold { get { return isHold; } }
        public void OnPointerEnter(PointerEventData e)
        {
            isHover = true;
            onEnter?.Invoke();
            TriggerEventDispatcher.Instance?.triggerEvent?.Invoke(gameObject, TriggerType.Enter);
        }
        public void OnPointerExit(PointerEventData e)
        {
            isHover = false;
            onExit?.Invoke();
            TriggerEventDispatcher.Instance?.triggerEvent?.Invoke(gameObject, TriggerType.Exit);
        }
        public void OnPointerDown(PointerEventData e)
        {
            isHold = true;
            onDown?.Invoke();
            TriggerEventDispatcher.Instance?.triggerEvent?.Invoke(gameObject, TriggerType.Down);
        }
        public void OnPointerUp(PointerEventData e)
        {
            isHold = false;
            onUp?.Invoke();
            TriggerEventDispatcher.Instance?.triggerEvent?.Invoke(gameObject, TriggerType.Up);
        }
        public void OnPointerClick(PointerEventData e)
        {
            onClick?.Invoke();
            TriggerEventDispatcher.Instance?.triggerEvent?.Invoke(gameObject, TriggerType.Click);
        }
        public void OnDrag(PointerEventData e)
        {
            onDrag?.Invoke();
            TriggerEventDispatcher.Instance?.triggerEvent?.Invoke(gameObject, TriggerType.Drag);
        }
        private void Update()
        {
            if (isHover)
            {
                onOver?.Invoke();
                TriggerEventDispatcher.Instance?.triggerEvent?.Invoke(gameObject, TriggerType.Over);
            }
        }
    }
}
