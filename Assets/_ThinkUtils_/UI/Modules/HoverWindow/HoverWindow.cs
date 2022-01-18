//用于显示浮窗的脚本

//By NeilianAryan

using System;
using UnityEngine;
using UnityEngine.UI;

namespace ThinkUtils.UI
{
    public class HoverWindow : Singleton<HoverWindow>
    {
        /// <summary>
        /// 悬浮窗
        /// </summary>
        GameObject window;
        /// <summary>
        /// 悬浮窗的rect
        /// </summary>
        RectTransform windowRect;
        /// <summary>
        /// 文本信息
        /// </summary>
        Text info;
        /// <summary>
        /// 是否进入展示浮窗模式
        /// </summary>
        bool hoverMode;
        /// <summary>
        /// 是否处于展示浮窗模式
        /// </summary>
        /// <value></value>
        public bool IsHoverMode { get { return hoverMode; } }
        /// <summary>
        /// 是否展示浮窗
        /// </summary>
        bool show;
        /// <summary>
        /// 点击物体的事件
        /// </summary>
        public Action<string, GameObject> onClick;
        private void Awake()
        {
            //获取相关组件
            window = transform.GetChild(0).gameObject;
            windowRect = window.GetComponent<RectTransform>();
            info = GetComponentInChildren<Text>(true);
        }
        /// <summary>
        /// 开启浮窗模式
        /// </summary>
        public void StartHoverMode()
        {
            hoverMode = true;
        }
        /// <summary>
        /// 退出浮窗模式
        /// </summary>
        public void StopHoverMode()
        {
            Hide();
            hoverMode = false;
        }
        /// <summary>
        /// 展示浮窗
        /// </summary>
        /// <param name="content">文本信息</param>
        public void Show(string content)
        {
            if (!hoverMode) return;
            window.SetActive(true);
            info.text = content;
            show = true;
        }
        /// <summary>
        /// 关闭浮窗
        /// </summary>
        public void Hide()
        {
            if (!hoverMode) return;
            window.SetActive(false);
            show = false;
        }
        private void Update()
        {
            if (!hoverMode) return;
            if (show)
            {
                //更新浮窗位置跟随鼠标
                windowRect.position = Input.mousePosition;
            }
        }
    }
}
