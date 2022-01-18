//用于鼠标悬停时放大UI元素的脚本


using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

namespace ThinkUtils.UI
{
    public class HighlightZoomer : AnimBase, IPointerEnterHandler, IPointerExitHandler
    {
        /// <summary>
        /// 放大倍数，默认为1.1倍
        /// </summary>
        public float delta = 1.1f;
        private void OnEnable()
        {
            //开启时记录初始状态
            rectTransforms = new RectTransform[1];
            rectTransforms[0] = GetComponent<RectTransform>();
            originScale = rectTransforms[0].localScale;
            targetScale = rectTransforms[0].localScale * delta;
        }
        private void OnDisable()
        {
            //关闭时恢复初始状态
            rectTransforms[0].localScale = originScale;
        }
        public void OnPointerEnter(PointerEventData e)
        {
            //鼠标悬浮，放大UI
            StartCoroutine(Zoomer(originScale, targetScale));
        }
        public void OnPointerExit(PointerEventData e)
        {
            //鼠标移出，缩小UI
            StartCoroutine(Zoomer(targetScale, originScale));
        }
        IEnumerator Zoomer(Vector3 origin, Vector3 target)
        {
            rectTransforms[0].localScale = origin;
            for (int i = 0; i < animTime / Time.fixedDeltaTime; i++)
            {
                yield return new WaitForFixedUpdate();
                rectTransforms[0].localScale += (target - origin) / (animTime / Time.fixedDeltaTime);
            }
            rectTransforms[0].localScale = target;
        }
    }
}
