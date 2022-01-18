//UI模块父类脚本，包含公共成员和公共类

//Version:2.9

//By NeilianAryan

//2021_05_25 - 背包增加multiple字段支持
//2021_05_21 - 新增自定义功能按钮(FuncButton)
//2021_05_19 - 可折叠导航增加收起所有方法；背包扩展模块增加Trigger监测和Model打开功能；翻页器适配新的位移动画
//2021_05_13 - 可折叠导航增加根据路径添加按钮功能
//2021_05_11 - 修复按钮样式优先级错误的BUG
//2021_04_14 - 场景加载器增加编辑器用方法
//2021_03_23 - 完善编辑器生成功能；进度条增加环形样式和样式设置功能
//2021_03_15 - 修复Messenger连续发送消息计时器不重置的BUG；修复加载条连续调用进度叠加的BUG；增加编辑器生成常用模块功能
//2021_03_08 - 修复Bag拖动结束事件报空BUG；修复Messenger关闭页面协程报错BUG

//2021_03_04 - 2.0正式版
//包含以下模块
//SimpleNav(通用导航)、FoldableNav(可折叠导航)、Bag(背包)
//SceneLoader(场景加载器)、Messenger(消息弹框)、HoverWindow(浮窗)
//Fliper(翻页器)、Proceeder(加载条)
#region  1.x Log
//2021_03_04 - SimpleNav、FoldableNav、Bag增加非运行时UI生成
//2021_02_26 - 修复翻页器显示/隐藏进程中未禁用EventSystem引发的BUG
//2021_02_03 - 修复SimpleNav与HighlightSwapper组合使用时出现的文字变透明BUG；修复Messenger弹窗发送过快旧消息滞留BUG
//2021_01_24 - 新增加载条(Proceeder)
//2021_01_05 - 修复场景加载器再次使用进度条未重置BUG；修复翻页器eventSystem为空阻碍后续进程BUG
//2021_01_04 - 背包增加拖拽事件
//2020_12_28 - 翻页器增加显示/隐藏当前页功能
//2020_12_24 - 背包增加添加/删除项目功能
//2020_12_20 - 新增UI翻页逻辑处理脚本(Fliper)
//2020_12_15 - 场景加载器中进度条设置与异步加载分离（为解决webGL端异步加载卡死BUG）
#endregion
//2020_12_11 - 1.0正式版
//包含SimpleNav(通用导航)、FoldableNav(可折叠导航)、Bag(背包)、SceneLoader(场景加载器)、Messenger(消息弹框)、HoverWindow(浮窗)
#region 0.x Log
//2020_12_11 - 折叠栏增加搜索框功能；浮窗增加点击事件
//2020_12_10 - 背包增加通用项目底图配置、网格样式&边界配置，修复页面尺寸计算BUG；折叠栏适配uniform样式；新增浮窗
//2020_12_09 - 修复优化通用导航栏uniform样式使用问题；父类取消defaultColor，增加字色选项
//2020_12_07 - 通用导航栏增加悬浮高亮选项，调整uniformSprite优先级；修复折叠导航栏初始化字色BUG、层级样式BUG，增加autoFold可选项，叶子节点样式支持单独配置
//2020_12_04 - 消息弹框增加是否堆叠可选项和关闭最近一条消息的方法；修复通用导航栏uniformStyle无法生效的问题
//2020_12_03 - 通用导航增加更换指定按钮样式方法；折叠导航栏深度样式类增加字色选项
//2020_11_30 - 新增消息弹框
//2020_11_27 - 按钮事件完善；提取公共内容到父类；新增背包模块；代码优化
//2020_11_26 - 顶部导航栏修改为通用导航栏；父类增加normalizable、operable字段；通用导航栏增加multiple字段
//2020_11_25 - 建立工具脚本
#endregion

using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace ThinkUtils.UI
{
    public class ModuleBase : MonoBehaviour
    {
        /// <summary>
        /// 高亮字色|普通字色
        /// </summary>
        public Color highlightedTextColor = Color.white, normalTextColor = Color.black;
        /// <summary>
        /// 统一高亮样式|统一普通样式
        /// </summary>
        public Sprite uniformHighlightedSprite, uniformNormalSprite;
        /// <summary>
        /// 是否可操作
        /// </summary>
        public bool operable = true;
        /// <summary>
        /// 是否支持高亮取消
        /// </summary>
        public bool normalizable = true;
        /// <summary>
        /// 点击按钮的事件（供代码监听）
        /// </summary>
        public Action<string, ModuleButton> onClick;
        /// <summary>
        /// 生成按钮的原件
        /// </summary>
        protected GameObject originButton;
        /// <summary>
        /// 生成按钮的基本方法
        /// </summary>
        /// <param name="parent">按钮父物体</param>
        /// <param name="name">按钮物体名称</param>
        /// <param name="label">按钮上显示的名称</param>
        /// <returns>生成的按钮</returns>
        protected GameObject BaseGenerateButtons(Transform parent, string name, string label)
        {
            //生成按钮并打开
            GameObject newButton = (GameObject)Instantiate(originButton, parent);
            newButton.SetActive(true);
            newButton.name = name;
            //修改按钮标签文字，若无标签（文字做在图上）则跳过
            if (newButton.GetComponentInChildren<Text>(true) != null)
                newButton.GetComponentInChildren<Text>(true).text = label;
            return newButton;
        }
        /// <summary>
        /// 设置按钮的选中状态
        /// </summary>
        /// <param name="button">按钮实例</param>
        /// <param name="active">状态</param>
        /// <param name="defaultSprite">ModuleButton内无样式时的备用样式</param>
        /// <param name="preferSprite">优先于ModuleButton内置样式的指定样式</param>
        public void SetButtonState(ModuleButton button, bool active, Sprite defaultSprite = null, Sprite preferSprite = null)
        {
            //如果有默认样式，则先使用默认样式
            if (defaultSprite != null)
                button.button.image.sprite = defaultSprite;
            //如果有优先指定样式，则使用优先指定样式
            if (preferSprite != null)
                button.button.image.sprite = preferSprite;
            else
            {
                //否则根据状态，如果有内置样式则使用，否则如果没有默认样式则使用纯色
                if (active)
                {
                    if (button.highlightedSprite != null)
                        button.button.image.sprite = button.highlightedSprite;
                    else if (defaultSprite == null)
                        button.button.image.color = normalTextColor;//取反色
                }
                else
                {
                    if (button.normalSprite != null)
                        button.button.image.sprite = button.normalSprite;
                    else if (defaultSprite == null)
                        button.button.image.color = highlightedTextColor;//取反色
                }
            }
            //统一设置字色
            if (button.button.GetComponentInChildren<Text>() != null)
                button.button.GetComponentInChildren<Text>().color = active ? highlightedTextColor : normalTextColor;
            button.isSelected = active;
        }
    }
    /// <summary>
    /// 基本按钮类
    /// </summary>
    [Serializable]
    public class ModuleButton
    {
        /// <summary>
        /// 按钮标签
        /// </summary>
        public string label;
        /// <summary>
        /// 高亮样式|普通样式
        /// </summary>
        public Sprite highlightedSprite, normalSprite;
        /// <summary>
        /// 按钮行为
        /// </summary>
        public UnityEvent action;
        /// <summary>
        /// 是否选中
        /// </summary>
        [HideInInspector]
        public bool isSelected;
        /// <summary>
        /// 按钮Button组件
        /// </summary>
        [HideInInspector]
        public Button button;
    }
}
