//用于插值移动模型的脚本

//Version:0.1

//By Erdan

//2021_05_25

using System;
using System.Collections;
using UnityEngine;

namespace ThinkUtils
{
    public class ModelMover : Singleton<ModelMover>
    {
        IEnumerator lerpMove;
        /// <summary>
        /// 插值移动模型
        /// </summary>
        /// <param name="obj">移动的模型</param>
        /// <param name="startPos">起始点</param>
        /// <param name="endPos">终点</param>
        /// <param name="moveTime">时间</param>
        /// <param name="done">完成回调</param>
        public void Move(GameObject obj, Transform startPos, Transform endPos, float moveTime = 1f, Action done = null)
        {
            lerpMove = MoveCoroutine(obj, startPos, endPos, moveTime, done);
            StartCoroutine(lerpMove);
        }
        IEnumerator MoveCoroutine(GameObject obj, Transform startPos, Transform endPos, float moveTime = 1f, Action action = null)
        {
            float time = 0;
            while (Vector3.Distance(obj.transform.position, endPos.position) > 0.001)
            {
                time += Time.fixedDeltaTime;
                obj.transform.position = Vector3.Lerp(startPos.position, endPos.position, time / moveTime);
                yield return new WaitForFixedUpdate();
                if (time >= moveTime)
                {
                    break;
                }
            }
            obj.transform.position = endPos.position;
            action?.Invoke();
        }
    }
}
