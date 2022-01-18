//用于UI元素移入移出的脚本

using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace ThinkUtils.UI
{
    public class InOutTransformer : AnimBase
    {
        [Tooltip("UI呈现时的位置，请使用单独的空物体，保证窗口尺寸变化时依旧位移适配")]
        public Transform originPos;
        [Tooltip("移入时的起始位置/移出时的结束位置")]
        public Transform targetPos;
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
            }
            originPose = new Pose(originPos.position, originPos.rotation);
            if (targetPos != null)
                targetPose = new Pose(targetPos.position, targetPos.rotation);
        }
        /// <summary>
        /// 移入
        /// </summary>
        /// <param name="done">完成后的回调</param>
        public void TransformIn(Action done, Transform customPos = null)
        {
            if (operaLock) return;
            if (gameObject.activeInHierarchy) return;
            gameObject.SetActive(true);
            Initial();
            Pose targetPose = customPos == null ? this.targetPose : new Pose(customPos.position, customPos.rotation);
            StartCoroutine(Transformer(targetPose, originPose, done));
        }
        /// <summary>
        /// 移入
        /// </summary>
        public void TransformIn()
        {
            TransformIn(null);
        }
        /// <summary>
        /// 移出
        /// </summary>
        /// <param name="done">完成后的回调</param>
        public void TransformOut(Action done, Transform customPos = null)
        {
            if (operaLock) return;
            if (!gameObject.activeInHierarchy) return;
            Initial();
            Pose targetPose = customPos == null ? this.targetPose : new Pose(customPos.position, customPos.rotation);
            StartCoroutine(Transformer(originPose, targetPose, () => { gameObject.SetActive(false); done?.Invoke(); }));
        }
        /// <summary>
        /// 移出
        /// </summary>
        public void TransformOut()
        {
            TransformOut(null);
        }
        IEnumerator Transformer(Pose origin, Pose target, Action end)
        {
            operaLock = true;
            SetRaycastTarget(maskableGraphics, false);
            rectTransforms[0].position = origin.position;
            rectTransforms[0].rotation = origin.rotation;
            for (int i = 0; i < animTime / Time.fixedDeltaTime; i++)
            {
                yield return new WaitForFixedUpdate();
                rectTransforms[0].position += (target.position - origin.position) / (animTime / Time.fixedDeltaTime);
                rectTransforms[0].eulerAngles += (target.rotation.eulerAngles - origin.rotation.eulerAngles) / (animTime / Time.fixedDeltaTime);
            }
            rectTransforms[0].position = target.position;
            rectTransforms[0].rotation = target.rotation;
            SetRaycastTarget(maskableGraphics, true);
            end?.Invoke();
            operaLock = false;
        }
    }
}
