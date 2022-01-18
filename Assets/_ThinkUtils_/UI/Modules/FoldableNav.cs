//用于生成可折叠导航栏的脚本

//By NeilianAryan

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ThinkUtils.UI
{
    public class FoldableNav : ModuleBase
    {
        /// <summary>
        /// 是否支持自动折叠
        /// </summary>
        public bool autoFold = true;
        /// <summary>
        /// 是否支持搜索
        /// </summary>
        public bool searchable = true;
        /// <summary>
        /// 背景样式
        /// </summary>
        public Sprite backgroundSprite;
        /// <summary>
        /// 样式列表，元素下标代表深度
        /// </summary>
        public FoldableNavStyle[] styles;
        /// <summary>
        /// 按钮树
        /// </summary>
        public List<FoldableNavButton> buttons = new List<FoldableNavButton>();
        /// <summary>
        /// 页面的RectTransform组件
        /// </summary>
        RectTransform rect;
        /// <summary>
        /// 搜索框
        /// </summary>
        InputField searchField;
        [ContextMenu("生成UI")]
        private void Awake()
        {
            //获得按钮原件
            originButton = transform.GetChild(0).GetChild(0).gameObject;
            //获得各组件
            rect = GetComponent<RectTransform>();
            searchField = transform.GetChild(1).GetComponent<InputField>();
            InitializePage();
            DestroyButtons();
            //生成按钮
            InstantiateButtons(buttons, "/");
        }
        /// <summary>
        /// 初始化页面的方法
        /// </summary>
        void InitializePage()
        {
            GetComponent<Image>().sprite = backgroundSprite;
            searchField.gameObject.SetActive(searchable);
            searchField.onEndEdit.AddListener(str => Search(str));
        }
        /// <summary>
        /// 生成按钮时记录深度的临时变量
        /// </summary>
        int depthRecorder = 0;
        /// <summary>
        /// 存储所有按钮的列表
        /// </summary>
        List<FoldableNavButton> allButtons = new List<FoldableNavButton>();
        /// <summary>
        /// 生成按钮树的方法
        /// </summary>
        /// <param name="buttons">按钮组</param>
        /// <param name="path">按钮路径</param>
        void InstantiateButtons(List<FoldableNavButton> buttons, string path)
        {
            for (int i = 0; i < buttons.Count; i++)
            {
                //生成按钮
                GameObject newButton = BaseGenerateButtons(transform.GetChild(0), path + buttons[i].label, buttons[i].label);
                //获取按钮Button组件
                buttons[i].button = newButton.GetComponent<Button>();
                //修改按钮样式
                if (styles.Length > depthRecorder)
                    buttons[i].button.GetComponent<RectTransform>().sizeDelta = styles[depthRecorder].size;
                else
                    buttons[i].button.GetComponent<RectTransform>().sizeDelta = buttons[i].button.GetComponent<RectTransform>().sizeDelta - Vector2.right * depthRecorder * 10;
                SetButtonState(buttons[i], false, depthRecorder);
                //为按钮绑定方法
                int k = i, l = depthRecorder;
                buttons[k].button.onClick.AddListener(() =>
                {
                    //如果可操作，则执行相应方法
                    if (operable)
                    {
                        buttons[k].action?.Invoke();
                        Unfold(k, buttons, l);
                        onClick?.Invoke(path + buttons[k].label, buttons[k]);
                    }
                });
                //存储按钮
                allButtons.Add(buttons[i]);
                //如果有子节点，则进行递归，增加并传递按钮路径
                if (buttons[i].childNode.Count > 0)
                {
                    depthRecorder++;
                    InstantiateButtons(buttons[i].childNode, path + buttons[i].label + "/");
                    depthRecorder--;
                }
            }
            //生成完成后初始化折叠状态
            Unfold(-1, buttons, 0);
        }
        /// <summary>
        /// 清除折叠栏项目的方法
        /// </summary>
        void DestroyButtons()
        {
            int childCount = transform.GetChild(0).childCount - 1;
            for (int i = 0; i < childCount; i++)
            {
                DestroyImmediate(transform.GetChild(0).GetChild(1).gameObject);
            }
        }
        /// <summary>
        /// 新增按钮
        /// </summary>
        /// <param name="depthPath">按钮深度路径信息</param>
        /// <param name="label">按钮标签</param>
        /// <param name="startFromZero">路径是否从0开始</param>
        public void AddButton(int[] depthPath, string label, bool startFromZero = true)
        {
            //记录当前层级的按钮组
            List<FoldableNavButton> indexButton = buttons;
            //对深度路径进行遍历
            for (int i = 0; i < depthPath.Length; i++)
            {
                //当前路径的深度编号
                int targetIndex = depthPath[i] - (startFromZero ? 0 : 1);
                //如果深度编号小于等于按钮组数量且深度编号不为负数，则进入下一层路径
                if (indexButton.Count > targetIndex && targetIndex >= 0)
                    indexButton = indexButton[targetIndex].childNode;
                //否则判断是否到达底部，到达则创建按钮
                else if (i == depthPath.Length - 1)
                {
                    FoldableNavButton newButton = new FoldableNavButton();
                    newButton.label = label;
                    indexButton.Add(newButton);
                    DestroyButtons();
                    InstantiateButtons(buttons, "/");
                }
                //否则提示路径不合法
                else
                    Debug.LogError("要添加的项目父物体路径越界！");
            }
        }
        /// <summary>
        /// 折叠或展开按钮的方法
        /// </summary>
        /// <param name="index">要折叠或展开的按钮，若为-1则折叠所有按钮</param>
        /// <param name="buttons">按钮所在组</param>
        /// <param name="depth">样式深度</param>
        void Unfold(int index, List<FoldableNavButton> buttons, int depth)
        {
            //记录按钮初始状态
            bool isSelected = false;
            if (index != -1)
                isSelected = buttons[index].isSelected;
            //如果支持自动折叠或index为-1，通过遍历递归，关闭按钮所在组所有的子节点
            if (autoFold || index == -1)
            {
                foreach (var item in buttons)
                {
                    SetButtonState(item, false, depth);
                    if (item.childNode.Count > 0)
                    {
                        for (int i = 0; i < item.childNode.Count; i++)
                            item.childNode[i].button.gameObject.SetActive(false);
                        Unfold(-1, item.childNode, depth + 1);
                    }
                }
            }
            if (index == -1)
                return;
            //如果不支持自动折叠，每次点击取消所有叶子按钮高亮
            if (!autoFold)
                NormalizeAll(0, this.buttons);
            //如果按钮未选中，或不支持取消高亮且为叶子节点，则高亮，否则取消高亮
            SetButtonState(buttons[index], !isSelected || (!normalizable && buttons[index].childNode.Count == 0), depth);
            //如果有子节点，根据按钮选中状态展开或折叠其子节点
            if (buttons[index].childNode.Count > 0)
            {
                if (buttons[index].isSelected)
                    foreach (var item in buttons[index].childNode)
                        item.button.gameObject.SetActive(true);
                else
                    Unfold(-1, buttons, depth);
            }
        }
        /// <summary>
        /// 取消所有叶子按钮高亮的方法
        /// </summary>
        /// <param name="depth">当前深度</param>
        /// <param name="buttons">按钮所在组</param>
        void NormalizeAll(int depth, List<FoldableNavButton> buttons)
        {
            foreach (var item in buttons)
            {
                //取消按钮高亮
                if (item.childNode.Count == 0)
                    SetButtonState(item, false, depth);
                else
                    NormalizeAll(depth + 1, item.childNode);
            }
        }
        /// <summary>
        /// 收起所有按钮
        /// </summary>
        public void UnfoldAll()
        {
            Unfold(-1, buttons, 0);
            NormalizeAll(0, buttons);
        }
        /// <summary>
        /// 设置按钮的选中状态
        /// </summary>
        /// <param name="button">按钮实例</param>
        /// <param name="active">状态</param>
        /// <param name="depth">样式深度</param>
        public void SetButtonState(FoldableNavButton button, bool active, int depth)
        {
            if (active)
            {
                if (styles.Length > depth)
                {
                    button.button.image.sprite = styles[depth].highlightedSprite;
                    button.button.GetComponentInChildren<Text>().color = styles[depth].highlightedTextColor;
                    if (button.childNode.Count == 0 && styles[depth].leafHighlight != null)
                    {
                        button.button.image.sprite = styles[depth].leafHighlight;
                        button.button.GetComponentInChildren<Text>().color = styles[depth].leafHighlightText;
                    }
                }
                else
                {
                    if (uniformHighlightedSprite != null)
                        button.button.image.sprite = uniformHighlightedSprite;
                    else
                        button.button.image.color = normalTextColor;
                    button.button.GetComponentInChildren<Text>().color = highlightedTextColor;
                }
            }
            else
            {
                if (styles.Length > depth)
                {
                    button.button.image.sprite = styles[depth].normalSprite;
                    button.button.GetComponentInChildren<Text>().color = styles[depth].normalTextColor;
                    if (button.childNode.Count == 0 && styles[depth].leafNormal != null)
                        button.button.image.sprite = styles[depth].leafNormal;
                }
                else
                {
                    if (uniformNormalSprite != null)
                        button.button.image.sprite = uniformNormalSprite;
                    else
                        button.button.image.color = highlightedTextColor;
                    button.button.GetComponentInChildren<Text>().color = normalTextColor;
                }
            }
            button.isSelected = active;
        }
        /// <summary>
        /// 搜索按钮的方法
        /// </summary>
        /// <param name="content"></param>
        void Search(string content)
        {
            //首先恢复初始化状态
            foreach (var item in buttons)
                item.button.gameObject.SetActive(true);
            Unfold(-1, buttons, 0);
            //如果搜索内容为空字符串则退出
            if (content == "") return;
            //遍历并打开符合结果的按钮
            foreach (var item in allButtons)
                item.button.gameObject.SetActive(item.label.Contains(content));
        }
    }
    /// <summary>
    /// 可折叠导航按钮类
    /// </summary>
    [Serializable]
    public class FoldableNavButton : ModuleButton
    {
        /// <summary>
        /// 子节点
        /// </summary>
        public List<FoldableNavButton> childNode = new List<FoldableNavButton>();
    }
    /// <summary>
    /// 可折叠导航样式类
    /// </summary>
    [Serializable]
    public class FoldableNavStyle
    {
        /// <summary>
        /// 高亮字色|普通字色|叶子节点高亮字色
        /// </summary>
        public Color highlightedTextColor, normalTextColor, leafHighlightText;
        /// <summary>
        /// 高亮样式|普通样式
        /// </summary>
        public Sprite highlightedSprite, normalSprite;
        /// <summary>
        /// 为叶子节点时的，高亮样式|普通样式
        /// </summary>
        public Sprite leafHighlight, leafNormal;
        /// <summary>
        /// 尺寸
        /// </summary>
        public Vector2 size;
    }
}
