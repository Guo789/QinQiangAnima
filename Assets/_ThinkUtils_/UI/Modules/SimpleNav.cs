//用于生成普通导航栏的脚本(给相关按钮添加事件)

using UnityEngine;
using UnityEngine.UI;

namespace ThinkUtils.UI
{
    /// <summary>
    /// 按钮排列方向枚举
    /// </summary>
    public enum NavOrientation
    {
        /// <summary>
        /// 横向
        /// </summary>
        Horizontal,
        /// <summary>
        /// 纵向
        /// </summary>
        Vertical,
    }
    public class SimpleNav : ModuleBase
    {
        /// <summary>
        /// 是否可以同时高亮多个
        /// </summary>
        public bool multiple = false;
        /// <summary>
        /// 是否作为切换键
        /// </summary>
        public bool asToggle = false;
        /// <summary>
        /// 导航按钮组排列方向
        /// </summary>
        public NavOrientation orientation;
        /// <summary>
        /// 按钮组
        /// </summary>
        public ModuleButton[] buttons;
        [ContextMenu("生成UI")]
        private void Awake()
        {
            //获得按钮原件
            originButton = transform.GetChild(0).gameObject;
            DestroyButtons();
            //生成按钮组
            InstantiateButtons();
        }
        /// <summary>
        /// 生成按钮组的方法
        /// </summary>
        public void InstantiateButtons()
        {
            for (int i = 0; i < buttons.Length; i++)
            {
                //生成按钮
                GameObject newButton = BaseGenerateButtons(orientation == NavOrientation.Horizontal ? transform.GetChild(1) : transform.GetChild(2), buttons[i].label, buttons[i].label);
                //获取按钮Button组件
                buttons[i].button = newButton.GetComponent<Button>();
                //如果不作为切换键，则为按钮添加鼠标悬浮高亮组件，并设置组件样式
                if (!asToggle)
                {
                    HighlightSwapper swapper = buttons[i].button.gameObject.AddComponent<HighlightSwapper>();
                    if (buttons[i].highlightedSprite != null)
                    {
                        swapper.highlightedSprite = buttons[i].highlightedSprite;
                        swapper.normalSprite = buttons[i].normalSprite;
                    }
                    else if (uniformHighlightedSprite != null)
                    {
                        swapper.highlightedSprite = uniformHighlightedSprite;
                        swapper.normalSprite = uniformNormalSprite;
                    }
                    swapper.highlightedColor = normalTextColor;
                    swapper.normalColor = highlightedTextColor;
                }
                //为按钮绑定方法
                int k = i;
                buttons[k].button.onClick.AddListener(() =>
                {
                    //如果可操作，则执行相应方法
                    if (operable)
                    {
                        buttons[k].action.Invoke();
                        Toggle(k);
                        onClick?.Invoke(buttons[k].label, buttons[k]);
                    }
                });
            }
            //生成完成后初始化高亮
            Toggle(-1);
        }
        /// <summary>
        /// 编辑器中清除按钮组的方法
        /// </summary>
        void DestroyButtons()
        {
            int childCount1 = transform.GetChild(1).childCount;
            int childCount2 = transform.GetChild(2).childCount;
            for (int i = 0; i < childCount1; i++)
                DestroyImmediate(transform.GetChild(1).GetChild(0).gameObject);
            for (int i = 0; i < childCount2; i++)
                DestroyImmediate(transform.GetChild(2).GetChild(0).gameObject);
        }
        /// <summary>
        /// 切换按钮样式或颜色的方法
        /// </summary>
        /// <param name="index">要高亮的按钮，若为-1则不高亮任何按钮</param>
        void Toggle(int index)
        {
            //记录按钮状态
            bool isSelected = false;
            if (index != -1)
                isSelected = buttons[index].isSelected;
            //如果不支持多选或index为-1，则先取消所有按钮高亮
            if (!multiple || index == -1)
                foreach (var item in buttons)
                    SetButtonState(item, false, uniformNormalSprite);
            //如果index为-1或不作为切换键，则在取消高亮后直接返回
            if (index == -1 || !asToggle)
                return;
            //如果按钮未选中或不支持取消高亮，则高亮，否则取消高亮
            bool state = !isSelected || !normalizable;
            SetButtonState(buttons[index], state, state ? uniformHighlightedSprite : uniformNormalSprite);
        }
        /// <summary>
        /// 设置按钮选中状态的方法
        /// </summary>
        /// <param name="index">按钮下标</param>
        /// <param name="state">状态</param>
        /// <param name="custom">自定义样式</param>
        public void SetButtonState(int index, bool state, Sprite custom = null)
        {
            if (!multiple)
                Toggle(-1);
            SetButtonState(buttons[index], state, state ? uniformHighlightedSprite : uniformNormalSprite);
            if (custom != null)
                buttons[index].button.image.sprite = custom;
        }
    }
}
