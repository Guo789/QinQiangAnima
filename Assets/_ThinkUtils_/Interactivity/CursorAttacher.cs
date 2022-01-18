//用于将模型/UI绑定到鼠标光标上以供拖拽的脚本

//Version:0.2

//By NeilianAryan

//2021_01_04 - 增加UI拖拽的支持
//2020_12_24

using UnityEngine;

namespace ThinkUtils
{
    public class CursorAttacher : Singleton<CursorAttacher>
    {
        /// <summary>
        /// 吸附在光标上的模型
        /// </summary>
        public GameObject attachedModel;
        /// <summary>
        /// 距离屏幕的Z轴深度
        /// </summary>
        float attachingDepth;
        /// <summary>
        /// 吸附在光标上的UI
        /// </summary>
        public RectTransform attachedUI;
        /// <summary>
        /// 将模型吸附在光标上的方法
        /// </summary>
        /// <param name="model">模型</param>
        /// <param name="depthToCursor">距离屏幕的Z轴深度</param>
        public void AttachModel(GameObject model, float depthToScreen)
        {
            attachedModel = model;
            attachingDepth = depthToScreen;
        }
        /// <summary>
        /// 解除模型吸附的方法
        /// </summary>
        /// <param name="targetPos">目标位置，解除后模型将会作为目标Transform的子物体</param>
        public void DetachModel(Transform targetPos)
        {
            // attachedModel.transform.parent = targetPos;
            // attachedModel.transform.localPosition = Vector3.zero;
            // attachedModel.transform.localEulerAngles = Vector3.zero;
            attachedModel = null;
        }
        /// <summary>
        /// 将UI吸附在光标上的方法
        /// </summary>
        /// <param name="ui"></param>
        public void AttachUI(RectTransform ui)
        {
            attachedUI = ui;
        }
        /// <summary>
        /// 解除UI吸附的方法
        /// </summary>
        public void DetachUI(Vector3 pos)
        {
            attachedUI.anchoredPosition = pos;
            attachedUI = null;
        }
        private void Update()
        {
            //如果模型不为空，则将模型位置设置为光标的世界位置（深度采用attachingDepth）
            if (attachedModel != null)
            {
                attachedModel.transform.position = Camera.main.ScreenToWorldPoint(Vector3.forward * attachingDepth + Input.mousePosition);
            }
            //如果UI不为空，则将UI位置设置为光标位置
            if (attachedUI != null)
            {
                attachedUI.position = Input.mousePosition;
            }
        }
    }
}
