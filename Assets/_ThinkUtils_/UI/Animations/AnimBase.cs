//UI元素动画父类，包含公共成员

//Version:1.0

//By NeilianAryan

//2021_05_19 - 1.0正式版
//包含淡入淡出、缩入缩出、移入移出、悬浮变色、悬浮放大、悬浮位移共6种动画
#region 0.x Log
//2021_05_19 - 对开启类动画进行判重处理；修复移入移出动画的origin位置在窗口改变时不更新的问题
//2021_05_07 - 对关闭类动画增加物体判空操作
//2021_04_29 - 修复位移动画关于姿态获取的一个BUG
//2021_02_04 - InOut动画增加raycastTarget锁，动画过程中UI将无法交互
//2020_12_28 - swapper动画调整优化
//2020_12_20 - 完善位移动画为InOut类动画，区分移入移出及逆操作，增加开关物体功能；完善更换样式动画
//2020_12_03 - 修复位移动画可能会引发数组越界的BUG；优化数据结构、注释方式
//2020_11_30 - 增加动画类型枚举；各动画增加回调和操作锁；位移动画增加旋转；增加渐变高亮动画
//2020_10_13 - 代码优化；增加位移动画脚本
//2020_08_23
#endregion

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ThinkUtils.UI
{
    public class AnimBase : MonoBehaviour
    {
        /// <summary>
        /// 所有元素的ReactTransform
        /// </summary>
        protected RectTransform[] rectTransforms;
        /// <summary>
        /// 所有元素的可调色组件（Image、Text等）
        /// </summary>
        protected MaskableGraphic[] maskableGraphics;
        /// <summary>
        /// 原始位置，目标位置
        /// </summary>
        protected Pose originPose, targetPose;
        /// <summary>
        /// 原始缩放，目标缩放
        /// </summary>
        protected Vector3 originScale, targetScale;
        /// <summary>
        /// 原始大小，目标大小
        /// </summary>
        protected Vector2 originSize, targetSize;
        /// <summary>
        /// 原始色彩，目标色彩
        /// </summary>
        protected List<Color> originColors = new List<Color>(), targetColors = new List<Color>();
        /// <summary>
        /// 动画时长
        /// </summary>
        public float animTime = 0.1f;
        /// <summary>
        /// 操作锁
        /// </summary>
        protected bool operaLock;
        /// <summary>
        /// 设置UI可交互性
        /// </summary>
        /// <param name="graphics">UI</param>
        /// <param name="enable">是否可交互</param>
        protected void SetRaycastTarget(MaskableGraphic[] graphics, bool enable)
        {
            foreach (var item in graphics)
            {
                item.raycastTarget = enable;
            }
        }
    }
    /// <summary>
    /// 动画类型
    /// </summary>
    public enum AnimType
    {
        //无动画
        None,
        //缩放
        Zoom,
        //渐变
        Fade,
        //移动
        Transform,
    }
}
