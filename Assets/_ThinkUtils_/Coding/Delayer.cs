//用于延迟执行逻辑的脚本

//Version:0.1

//By NeilianAryan

//2021_06_01

using System;
using System.Collections;
using UnityEngine;

namespace ThinkUtils
{
    public class Delayer : Singleton<Delayer>
    {
        /// <summary>
        /// 延迟指定秒数后执行
        /// </summary>
        /// <param name="seconds">秒数</param>
        /// <param name="doSth">执行内容</param>
        public void DelaySeconds(float seconds, Action doSth)
        {
            StartCoroutine(DelayExcuter(true, seconds, doSth));
        }
        /// <summary>
        /// 延迟指定帧数后执行
        /// </summary>
        /// <param name="frames">帧数</param>
        /// <param name="doSth">执行内容</param>
        public void DelayFrames(int frames, Action doSth)
        {
            StartCoroutine(DelayExcuter(false, frames, doSth));
        }
        IEnumerator DelayExcuter(bool waitForSeconds, float parameter, Action doSth)
        {
            if (waitForSeconds)
                yield return new WaitForSeconds(parameter);
            else
                for (int i = 0; i < parameter; i++)
                    yield return new WaitForEndOfFrame();
            doSth?.Invoke();
        }
    }
}
