//用于挂载在要显示浮窗的物体上的脚本

//By NeilianAryan

using UnityEngine;

namespace ThinkUtils.UI
{
    [RequireComponent(typeof(Collider))]
    public class HoverWindowTarget : MonoBehaviour
    {
        /// <summary>
        /// 浮窗显示内容
        /// </summary>
        public string content;
        private void OnMouseEnter()
        {
            if (content == "")
                content = gameObject.name;
            HoverWindow.Instance.Show(content);
        }
        private void OnMouseExit()
        {
            HoverWindow.Instance.Hide();
        }
        private void OnMouseUpAsButton()
        {
            if (HoverWindow.Instance.IsHoverMode)
                HoverWindow.Instance.onClick?.Invoke(content, gameObject);
        }
    }
}
