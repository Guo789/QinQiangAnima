//用于处理UI翻页逻辑的脚本


using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace ThinkUtils.UI
{
    public class Fliper : MonoBehaviour
    {
        /// <summary>
        /// 起始页
        /// </summary>
        public InOutTransformer firstPage;
        public Transform inPos, outPos;
        /// <summary>
        /// 当前页面
        /// </summary>
        public GameObject CurrentPage { get { return pageStack.Peek().gameObject; } }
        EventSystem eventSystem;
        /// <summary>
        /// 页面栈
        /// </summary>
        Stack<InOutTransformer> pageStack = new Stack<InOutTransformer>();
        private void Start()
        {
            pageStack.Push(firstPage);
        }
        /// <summary>
        /// 前往下一页
        /// </summary>
        /// <param name="next">下一页</param>
        public void Flip(InOutTransformer next)
        {
            if (eventSystem == null)
                eventSystem = FindObjectOfType<EventSystem>();
            if (eventSystem != null)
                eventSystem.enabled = false;
            pageStack.Peek().TransformOut(null, outPos);
            next.TransformIn(() => { if (eventSystem != null) eventSystem.enabled = true; }, inPos);
            pageStack.Push(next);
        }
        /// <summary>
        /// 返回上一页
        /// </summary>
        public void Back()
        {
            //如果已经到达起始页则不再允许返回
            if (pageStack.Count == 1) return;
            if (eventSystem == null)
                eventSystem = FindObjectOfType<EventSystem>();
            if (eventSystem != null)
                eventSystem.enabled = false;
            pageStack.Pop().TransformOut(null, inPos);
            pageStack.Peek().TransformIn(() => { if (eventSystem != null) eventSystem.enabled = true; }, outPos);
        }
        /// <summary>
        /// 返回首页
        /// </summary>
        public void BackHome()
        {
            pageStack.Peek().gameObject.SetActive(false);
            firstPage.TransformIn();
            pageStack.Clear();
            pageStack.Push(firstPage);
        }
        /// <summary>
        /// 显示或隐藏当前页
        /// </summary>
        /// <param name="hide">是否隐藏</param>
        public void HideCurrent(bool hide)
        {
            if (eventSystem == null)
                eventSystem = FindObjectOfType<EventSystem>();
            if (eventSystem != null)
                eventSystem.enabled = false;
            if (hide)
                pageStack.Peek().TransformOut(() => { if (eventSystem != null) eventSystem.enabled = true; }, outPos);
            else
                pageStack.Peek().TransformIn(() => { if (eventSystem != null) eventSystem.enabled = true; }, outPos);
        }
    }
}
