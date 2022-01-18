//用于UI元素淡入淡出的脚本

//By NeilianAryan

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ThinkUtils.UI
{
    public class InOutFader : AnimBase
    {
        void Initial()
        {
            if (maskableGraphics == null)
            {
                maskableGraphics = GetComponentsInChildren<MaskableGraphic>();
                foreach (var item in maskableGraphics)
                {
                    originColors.Add(item.color);
                    targetColors.Add(new Color(item.color.r, item.color.g, item.color.b, 0));
                }
            }
        }
        /// <summary>
        /// 淡入
        /// </summary>
        /// <param name="done">完成后的回调</param>
        public void FadeIn(Action done)
        {
            if (operaLock) return;
            if (gameObject.activeInHierarchy) return;
            gameObject.SetActive(true);
            Initial();
            StartCoroutine(Fader(targetColors, originColors, done));
        }
        /// <summary>
        /// 淡入
        /// </summary>
        public void FadeIn()
        {
            FadeIn(null);
        }
        /// <summary>
        /// 淡出
        /// </summary>
        /// <param name="done">完成后的回调</param>
        public void FadeOut(Action done)
        {
            if (operaLock) return;
            if (!gameObject.activeInHierarchy) return;
            Initial();
            SetRaycastTarget(maskableGraphics, false);
            StartCoroutine(Fader(originColors, targetColors, () => { gameObject.SetActive(false); done?.Invoke(); }));
        }
        /// <summary>
        /// 淡出
        /// </summary>
        public void FadeOut()
        {
            FadeOut(null);
        }
        IEnumerator Fader(List<Color> origins, List<Color> targets, Action end)
        {
            operaLock = true;
            SetRaycastTarget(maskableGraphics, false);
            for (int i = 0; i < maskableGraphics.Length; i++)
            {
                maskableGraphics[i].color = origins[i];
            }
            for (int i = 0; i < animTime / Time.fixedDeltaTime; i++)
            {
                yield return new WaitForFixedUpdate();
                for (int j = 0; j < maskableGraphics.Length; j++)
                {
                    maskableGraphics[j].color += (targets[j] - origins[j]) / (animTime / Time.fixedDeltaTime);
                }
            }
            for (int i = 0; i < maskableGraphics.Length; i++)
            {
                maskableGraphics[i].color = targets[i];
            }
            SetRaycastTarget(maskableGraphics, true);
            end?.Invoke();
            operaLock = false;
        }
    }
}
