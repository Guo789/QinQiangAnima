//用于显示加载进度条的脚本

//By NeilianAryan

using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace ThinkUtils.UI
{
    public class Proceeder : MonoBehaviour
    {
        /// <summary>
        /// 进度条类型
        /// </summary>
        public ProceederType type;
        /// <summary>
        /// 背景图|填充图
        /// </summary>
        public Sprite bg, fill;
        /// <summary>
        /// 线性进度条
        /// </summary>
        public Slider slider;
        /// <summary>
        /// 环形进度条
        /// </summary>
        public Image circle;
        /// <summary>
        /// 读条协程
        /// </summary>
        Coroutine coroutine;
        private void Awake()
        {
            SetSprite();
        }
        /// <summary>
        /// 设置进度条样式
        /// </summary>
        [ContextMenu("设置样式")]
        void SetSprite()
        {
            bool isLinear = type == ProceederType.Linear;
            slider.gameObject.SetActive(isLinear);
            circle.transform.parent.gameObject.SetActive(!isLinear);
            if (isLinear)
            {
                if (bg != null) slider.transform.GetChild(0).GetComponent<Image>().sprite = bg;
                if (fill != null) slider.fillRect.GetComponent<Image>().sprite = fill;
            }
            else
            {
                if (bg != null) circle.transform.parent.GetComponent<Image>().sprite = bg;
                if (fill != null) circle.sprite = fill;
            }
        }
        /// <summary>
        /// 开始加载进度条的方法
        /// </summary>
        /// <param name="done">加载完成的回调</param>
        /// <param name="process">加载中的回调</param>
        /// <param name="spendTime">加载需要时间</param>
        /// <param name="reverse">是否反向递减加载</param>
        public void StartProceed(Action done = null, Action<int> process = null, float spendTime = 3, bool reverse = false)
        {
            if (coroutine != null)
                StopCoroutine(coroutine);
            coroutine = StartCoroutine(SpendTimeToProceed(spendTime, reverse, process, done));
        }
        IEnumerator SpendTimeToProceed(float time, bool reverse, Action<int> process, Action done)
        {
            for (int i = 0; i <= 100; i++)
            {
                slider.value = reverse ? 100 - i : i;
                circle.fillAmount = (reverse ? 100 - i : i) / 100f;
                process?.Invoke((int)slider.value);
                //每百分比需要等待的时间
                yield return new WaitForSecondsRealtime(time / 100);
            }
            done?.Invoke();
        }
    }
    public enum ProceederType
    {
        Linear, Circular
    }
}
