//用于处理背包拖出和拖上检测相关逻辑的扩展脚本

using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace ThinkUtils.UI
{
    public class BagDragAddition : MonoBehaviour
    {
        /// <summary>
        /// 背包拖出物体的类型（模型或UI图标）
        /// </summary>
        [Tooltip("背包拖出物体的类型（模型或UI图标）")]
        public BDAType itemType;
        /// <summary>
        /// 背包拖出物体的类型（模型或UI图标）
        /// </summary>
        [Tooltip("检测拖拽的触发器类型（模型或UI图标）")]
        public BDAType triggerType;
        /// <summary>
        /// 拖动模型时模型与屏幕距离
        /// </summary>
        [Tooltip("拖动模型时模型与屏幕距离")]
        public float depthToScreen = 1;
        /// <summary>
        /// 与本背包内物体可进行交互的触发器
        /// </summary>
        [Tooltip("与本背包内物体可进行交互的模型触发器")]
        public ModelTrigger[] modelTriggers;
        /// <summary>
        /// 与本背包内物体可进行交互的UI触发器
        /// </summary>
        [Tooltip("与本背包内物体可进行交互的UI触发器")]
        public UITrigger[] uiTriggers;
        /// <summary>
        /// 背包物体拖放到Trigger上触发的事件
        /// </summary>
        public Action<BagItemButton, GameObject> detectedTrigger;
        Bag bag;
        private void Start()
        {
            //获取背包，对拖拽事件进行业务绑定
            bag = GetComponent<Bag>();
            bag.onBeginDrag = (name, button) =>
            {
                //如果拖动模型，则生成背包项目关联的模型
                if (itemType == BDAType.Model)
                {
                    GameObject newModel = Instantiate(button.model, button.model.transform.parent);
                    newModel.SetActive(true);
                    CursorAttacher.Instance.AttachModel(newModel, depthToScreen);
                }
                //否则生成背包项目的icon
                else
                {
                    GameObject newIcon = Instantiate(button.button.gameObject, bag.transform.parent);
                    Destroy(newIcon.GetComponent<Button>());
                    Destroy(newIcon.GetComponent<EventTrigger>());
                    newIcon.GetComponent<Image>().raycastTarget = false;
                    newIcon.GetComponent<RectTransform>().sizeDelta = button.button.GetComponent<RectTransform>().sizeDelta;
                    CursorAttacher.Instance.AttachUI(newIcon.GetComponent<RectTransform>());
                }
            };
            //结束拖拽，对待检测触发器进行检测，如果鼠标悬浮在其中之一上，则发布事件
            bag.onEndDrag = (name, button) =>
            {
                if (triggerType == BDAType.Model)
                    foreach (var item in modelTriggers)
                    {
                        if (item.IsHover)
                            detectedTrigger?.Invoke(button, item.gameObject);
                    }
                else
                    foreach (var item in uiTriggers)
                    {
                        if (item.IsHover)
                            detectedTrigger?.Invoke(button, item.gameObject);
                    }
                //根据拖动物体类型进行对应销毁
                if (itemType == BDAType.Model)
                {
                    CursorAttacher.Instance.DetachModel(null);
                    // Destroy(CursorAttacher.Instance.attachedModel);
                }
                else
                    Destroy(CursorAttacher.Instance.attachedUI.gameObject);
            };
        }
        private void Update()
        {
            //如果点击鼠标右键或中键，直接销毁拖拽物体（因为结束拖拽事件将无法触发，导致拖拽物滞留在光标上）
            if (CursorAttacher.Instance != null)
                if (Input.GetMouseButtonDown(1) || Input.GetMouseButtonDown(2))
                {
                    if (CursorAttacher.Instance.attachedModel != null) Destroy(CursorAttacher.Instance.attachedModel);
                    if (CursorAttacher.Instance.attachedUI != null) Destroy(CursorAttacher.Instance.attachedUI);
                }
        }
    }
    public enum BDAType
    {
        Model, UI
    }
}
