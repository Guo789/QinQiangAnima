//用于UI元素弹入弹出的脚本

//By NeilianAryan

using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace ThinkUtils.UI
{
    public class InOutZoomer : AnimBase
    {
        /// <summary>
        /// 记录初始状态
        /// </summary>
        void Initial()
        {
            if (maskableGraphics == null)
                maskableGraphics = GetComponentsInChildren<MaskableGraphic>();
            if (rectTransforms == null)
            {
                rectTransforms = new RectTransform[1];
                rectTransforms[0] = GetComponent<RectTransform>();
                originScale = rectTransforms[0].localScale;
                targetScale = Vector3.zero;
            }
        }
        /// <summary>
        /// 弹入
        /// </summary>
        /// <param name="done">完成后的回调</param>
        public void ZoomIn(Action done)
        {
            if (operaLock) return;
            if (gameObject.activeInHierarchy) return;
            gameObject.SetActive(true);
            Initial();
            StartCoroutine(Zoomer(targetScale, originScale, done));
        }
        /// <summary>
        /// 弹入
        /// </summary>
        public void ZoomIn()
        {
            ZoomIn(null);
        }
        /// <summary>
        /// 弹出
        /// </summary>
        /// <param name="done">完成后的回调</param>
        public void ZoomOut(Action done)
        {
            if (operaLock) return;
            if (!gameObject.activeInHierarchy) return;
            Initial();
            StartCoroutine(Zoomer(originScale, targetScale, () =>
            {
                gameObject.SetActive(false);
                rectTransforms[0].localScale = originScale;
                done?.Invoke();
            }));
        }
        /// <summary>
        /// 弹出
        /// </summary>
        public void ZoomOut()
        {
            ZoomOut(null);
        }
        IEnumerator Zoomer(Vector3 origin, Vector3 target, Action end)
        {
            operaLock = true;
            SetRaycastTarget(maskableGraphics, false);
            rectTransforms[0].localScale = origin;
            for (int i = 0; i < animTime / Time.fixedDeltaTime; i++)
            {
                yield return new WaitForFixedUpdate();
                rectTransforms[0].localScale += (target - origin) / (animTime / Time.fixedDeltaTime);
            }
            rectTransforms[0].localScale = target;
            SetRaycastTarget(maskableGraphics, true);
            end?.Invoke();
            operaLock = false;
        }
    }
}
