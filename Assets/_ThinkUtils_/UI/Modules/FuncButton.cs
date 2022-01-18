//自定义功能按钮脚本

//By NeilianAryan

using System;
using UnityEngine;
using UnityEngine.UI;

namespace ThinkUtils.UI
{
    public class FuncButton : MonoBehaviour
    {
        /// <summary>
        /// 按钮
        /// </summary>
        Button button;
        /// <summary>
        /// 按钮文本
        /// </summary>
        Text text;
        /// <summary>
        /// 按钮淡入淡出组件
        /// </summary>
        InOutFader fader;
        /// <summary>
        /// 设置并显示按钮的方法
        /// </summary>
        /// <param name="name">按钮文本</param>
        /// <param name="func">按钮执行操作</param>
        /// <param name="closeAfterTrigger">是否点击后关闭按钮</param>
        public void SetFuncButton(string name, Action func, bool closeAfterTrigger = true)
        {
            //初始化组件
            InitialComponents();
            //设置按钮文本
            text.text = name;
            //移除按钮所有事件
            button.onClick.RemoveAllListeners();
            //显示按钮
            fader.FadeIn();
            //为按钮绑定新方法
            button.onClick.AddListener(() =>
            {
                //点击按钮后移除所有事件，触发已有事件，并根据情况关闭按钮
                button.onClick.RemoveAllListeners();
                func?.Invoke();
                if (closeAfterTrigger)
                    fader.FadeOut();
            });
        }
        /// <summary>
        /// 主动关闭按钮的方法
        /// </summary>
        public void CloseFuncButton()
        {
            InitialComponents();
            fader.FadeOut();
        }
        /// <summary>
        /// 初始化组件的方法
        /// </summary>
        void InitialComponents()
        {
            if (button == null)
                button = GetComponent<Button>();
            if (text == null)
                text = GetComponentInChildren<Text>(true);
            if (fader == null)
                fader = GetComponent<InOutFader>();
        }
    }
}
