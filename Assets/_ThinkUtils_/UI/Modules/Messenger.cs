//用于生成消息弹框的脚本

//By NeilianAryan

using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace ThinkUtils.UI
{
    public class Messenger : MonoBehaviour
    {
        /// <summary>
        /// 是否具有标题
        /// </summary>
        public bool hasTitle;
        /// <summary>
        /// 是否可关闭
        /// </summary>
        public bool closable;
        /// <summary>
        /// 动画类型
        /// </summary>
        public AnimType animType;
        public MessengerPage messenger;
        Coroutine inAnim;
        /// <summary>
        /// 发送一个持续存在的消息
        /// </summary>
        /// <param name="message">消息内容</param>
        /// <param name="title">消息标题</param>
        public void Send(string message, string title = null)
        {
            Send(message, -1, title);
        }
        /// <summary>
        /// 发送一个存在一段时间的消息
        /// </summary>
        /// <param name="message">消息内容</param>
        /// <param name="title">消息标题</param>
        /// <param name="persistence">持续时间</param>
        public void Send(string message, float persistence, string title = null)
        {
            //如果有标题则打开标题，反之关闭
            messenger.title.gameObject.SetActive(hasTitle);
            //如果可关闭，则显示关闭按钮
            messenger.closeBtn.gameObject.SetActive(closable);
            //显示消息标题和内容
            messenger.title.text = title;
            messenger.content.text = message;
            if (inAnim != null)
                StopCoroutine(inAnim);
            inAnim = StartCoroutine(MessageIn(persistence));
        }
        /// <summary>
        /// 显示消息
        /// </summary>
        /// <param name="persistence">持续时间</param>
        IEnumerator MessageIn(float persistence)
        {
            //判断动画类型并播放
            switch (animType)
            {
                case AnimType.None: messenger.page.SetActive(true); break;
                case AnimType.Zoom: messenger.zoomer.ZoomIn(); break;
                case AnimType.Fade: messenger.fader.FadeIn(); break;
                case AnimType.Transform: messenger.transformer.TransformIn(); break;
            }
            //如果持续时间不为-1则等待时间后关闭消息，否则不关闭
            if (persistence != -1)
            {
                yield return new WaitForSecondsRealtime(persistence);
                CloseMessage();
            }
        }
        /// <summary>
        /// 关闭消息
        /// </summary>
        public void CloseMessage()
        {
            if (messenger.page.activeInHierarchy)
                switch (animType)
                {
                    case AnimType.None: messenger.page.SetActive(false); break;
                    case AnimType.Zoom: messenger.zoomer.ZoomOut(); break;
                    case AnimType.Fade: messenger.fader.FadeOut(); break;
                    case AnimType.Transform: messenger.transformer.TransformOut(); break;
                }
        }
    }
    [Serializable]
    public class MessengerPage
    {
        public GameObject page;
        public Text title, content;
        public Button closeBtn;
        public InOutZoomer zoomer;
        public InOutFader fader;
        public InOutTransformer transformer;
    }
}
