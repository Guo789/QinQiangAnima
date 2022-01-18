//用于鼠标悬停时渐变UI元素的脚本

//By NeilianAryan

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace ThinkUtils.UI
{
    public class HighlightFader : AnimBase, IPointerEnterHandler, IPointerExitHandler
    {
        /// <summary>
        /// 高亮时透明度，默认为1
        /// </summary>
        public float delta = 1;
        private void OnEnable()
        {
            //开启时记录初始状态
            if (maskableGraphics == null)
            {
                maskableGraphics = GetComponentsInChildren<MaskableGraphic>();
                foreach (var item in maskableGraphics)
                {
                    originColors.Add(item.color);
                    targetColors.Add(new Color(item.color.r, item.color.g, item.color.b, delta));
                }
            }
        }
        private void OnDisable()
        {
            //关闭时恢复初始状态
            for (int i = 0; i < maskableGraphics.Length; i++)
            {
                maskableGraphics[i].color = originColors[i];
            }
        }
        public void OnPointerEnter(PointerEventData e)
        {
            //鼠标悬浮，渐变UI
            StartCoroutine(Fader(originColors, targetColors));
        }
        public void OnPointerExit(PointerEventData e)
        {
            //鼠标移出，恢复UI
            StartCoroutine(Fader(targetColors, originColors));
        }
        IEnumerator Fader(List<Color> origins, List<Color> targets)
        {
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
        }
    }
}
