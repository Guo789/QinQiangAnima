//用于检测3D物体事件触发的脚本

//By NeilianAryan

using UnityEngine;
using UnityEngine.Events;

namespace ThinkUtils
{
    [RequireComponent(typeof(Collider))]
    public class ModelTrigger : MonoBehaviour
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
        private void OnMouseEnter()
        {
            isHover = true;
            onEnter?.Invoke();
            TriggerEventDispatcher.Instance?.triggerEvent?.Invoke(gameObject, TriggerType.Enter);
        }
        private void OnMouseOver()
        {
            onOver?.Invoke();
            TriggerEventDispatcher.Instance?.triggerEvent?.Invoke(gameObject, TriggerType.Over);
        }
        private void OnMouseExit()
        {
            isHover = false;
            onExit?.Invoke();
            TriggerEventDispatcher.Instance?.triggerEvent?.Invoke(gameObject, TriggerType.Exit);
        }
        private void OnMouseDown()
        {
            isHold = true;
            onDown?.Invoke();
            TriggerEventDispatcher.Instance?.triggerEvent?.Invoke(gameObject, TriggerType.Down);
        }
        private void OnMouseUp()
        {
            isHold = false;
            onUp?.Invoke();
            TriggerEventDispatcher.Instance?.triggerEvent?.Invoke(gameObject, TriggerType.Up);
        }
        private void OnMouseUpAsButton()
        {
            onClick?.Invoke();
            TriggerEventDispatcher.Instance?.triggerEvent?.Invoke(gameObject, TriggerType.Click);
        }
        private void OnMouseDrag()
        {
            onDrag?.Invoke();
            TriggerEventDispatcher.Instance?.triggerEvent?.Invoke(gameObject, TriggerType.Drag);
        }
    }
}
