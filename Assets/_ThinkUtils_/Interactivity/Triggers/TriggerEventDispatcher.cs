//用于总管派发交互事件的脚本

//Version:0.1

//By NeilianAryan

//2021_03_26

using System;
using UnityEngine;

namespace ThinkUtils
{
    public class TriggerEventDispatcher : Singleton<TriggerEventDispatcher>
    {
        /// <summary>
        /// 鼠标与模型或UI的交互事件
        /// </summary>
        public Action<GameObject, TriggerType> triggerEvent;
    }
    public enum TriggerType
    {
        Enter, Over, Exit, Down, Up, Click, Drag
    }
}
