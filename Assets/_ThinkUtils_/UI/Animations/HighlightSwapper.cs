//用于鼠标悬停时更换UI元素样式的脚本

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace ThinkUtils.UI
{
    public class HighlightSwapper : AnimBase, IPointerEnterHandler, IPointerExitHandler
    {
        /// <summary>
        /// 高亮样式|普通样式
        /// </summary>
        public Sprite highlightedSprite, normalSprite;
        /// <summary>
        /// 高亮颜色|普通颜色
        /// </summary>
        public Color highlightedColor, normalColor;
        /// <summary>
        /// 按钮的image组件
        /// </summary>
        Image image;
        private void OnEnable()
        {
            //开启时获取Image组件
            image = GetComponent<Image>();
        }
        private void OnDisable()
        {
            //关闭时恢复样式
            OnPointerExit(null);
        }
        public void OnPointerEnter(PointerEventData e)
        {
            //鼠标悬浮，更换图片或颜色
            if (highlightedSprite != null)
                image.sprite = highlightedSprite;
            else
                image.color = highlightedColor;
            GetComponentInChildren<Text>(true).color = normalColor;
        }
        public void OnPointerExit(PointerEventData e)
        {
            //鼠标移出，恢复图片或颜色
            if (normalSprite != null)
                image.sprite = normalSprite;
            else
                image.color = normalColor;
            GetComponentInChildren<Text>(true).color = highlightedColor;
        }
    }
}
