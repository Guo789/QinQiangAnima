//用于生成背包的脚本(给背包里的按钮添加事件)


using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace ThinkUtils.UI
{
    public class Bag : ModuleBase
    {
        /// <summary>
        /// 是否可以同时高亮多个
        /// </summary>
        public bool multiple = false;
        /// <summary>
        /// 是否具有标题
        /// </summary>
        public bool hasTitle = true;
        /// <summary>
        /// 是否具有项目计数
        /// </summary>
        public bool hasCounter = false;
        /// <summary>
        /// 是否具有分页器
        /// </summary>
        public bool hasPagination = false;
        /// <summary>
        /// 背包标题
        /// </summary>
        public string title;
        /// <summary>
        /// 背包单页尺寸
        /// </summary>
        public Vector2 columnAndRow = Vector2.one * 5;
        /// <summary>
        /// 背包网格边距
        /// </summary>
        public Vector2 gridMargin = Vector2.one * 5;
        /// <summary>
        /// 背包底图
        /// </summary>
        public Sprite pageSprite;
        /// <summary>
        /// 背包网格
        /// </summary>
        public Sprite gridSprite;
        /// <summary>
        /// 背包内项目
        /// </summary>
        public List<BagItemButton> items = new List<BagItemButton>();
        /// <summary>
        /// 放置整个内容的页面
        /// </summary>
        RectTransform page;
        /// <summary>
        /// 显示标题的Text
        /// </summary>
        Text titleText;
        /// <summary>
        /// 放置项目的网格
        /// </summary>
        GridLayoutGroup grid;
        /// <summary>
        /// 显示页码的Text
        /// </summary>
        public Text pageIndicator;
        /// <summary>
        /// 当前页码（从1开始）|总页数
        /// </summary>
        int currentPage = 1, maxPage = 1;
        /// <summary>
        /// 点击按钮的事件（供代码监听）
        /// </summary>
        new public Action<string, BagItemButton> onClick;
        /// <summary>
        /// 拖拽按钮的事件（供代码监听）
        /// </summary>
        public Action<string, BagItemButton> onBeginDrag, onEndDrag;
        [ContextMenu("生成UI")]
        private void Awake()
        {
            //获得按钮原件
            originButton = transform.GetChild(0).gameObject;
            //为各组件赋值
            page = transform.GetChild(1).GetComponent<RectTransform>();
            titleText = page.GetChild(0).GetComponent<Text>();
            grid = page.GetChild(1).GetComponent<GridLayoutGroup>();
            //初始化页面和其中项目，并将初始页码置为1
            InitializePage();
            DestroyItems();
            InstantiateItems();
            Flip(1);
        }
        /// <summary>
        /// 初始化背包页面标题、样式、尺寸的方法
        /// </summary>
        void InitializePage()
        {
            titleText.gameObject.SetActive(hasTitle);
            titleText.text = hasTitle ? title : "";
            page.GetComponent<Image>().sprite = pageSprite;
            grid.GetComponent<Image>().sprite = gridSprite;
            //各轴尺寸为 格子尺寸 x 格子数 + 间距 x (格子数 - 1) + 两边边距
            float gridWidth = grid.cellSize.x * columnAndRow.x + grid.spacing.x * (columnAndRow.x - 1) + grid.padding.right + grid.padding.left;
            float gridHeight = grid.cellSize.y * columnAndRow.y + grid.spacing.y * (columnAndRow.y - 1) + grid.padding.top + grid.padding.bottom;
            //如果具有标题，页面高度为网格高度额外加上标题高度，此外页面宽高还要加上gridMargin
            float pageHeight = (hasTitle ? (gridHeight + titleText.GetComponent<RectTransform>().sizeDelta.y) : gridHeight) + gridMargin.y;
            float pageWidth = gridWidth + gridMargin.x * 2;
            page.sizeDelta = new Vector2(pageWidth, pageHeight);
            grid.GetComponent<RectTransform>().sizeDelta = new Vector2(gridWidth, gridHeight);
            //如果具有分页器则打开，反之关闭
            pageIndicator.transform.parent.gameObject.SetActive(hasPagination);
        }
        /// <summary>
        /// 生成背包项目的方法
        /// </summary>
        void InstantiateItems()
        {
            for (int i = 0; i < items.Count; i++)
            {
                //生成项目
                GameObject newButton = BaseGenerateButtons(grid.transform, items[i].label, items[i].label);
                //获取按钮Button组件
                items[i].button = newButton.GetComponent<Button>();
                //获取计数器
                GameObject counter = items[i].button.transform.GetChild(2).gameObject;
                //如果具有项目计数，则打开计数，反之关闭
                counter.SetActive(hasCounter);
                //设置项目数量
                SetItemCount(items[i], items[i].count);
                //适配计数器位置和尺寸
                counter.GetComponent<RectTransform>().anchoredPosition = new Vector3(-grid.cellSize.x / 10, grid.cellSize.y / 10, 0);
                counter.GetComponent<Text>().fontSize = (int)(grid.cellSize.x / 5);
                //为按钮绑定方法
                int k = i;
                items[k].button.onClick.AddListener(() =>
                {
                    //如果可操作，则执行相应方法
                    if (operable)
                    {
                        items[k].action.Invoke();
                        Highlight(k);
                        onClick?.Invoke(items[k].label, items[k]);
                    }
                });
            }
            Highlight(-1);
        }
        /// <summary>
        /// 编辑器中清除背包项目的方法
        /// </summary>
        void DestroyItems()
        {
            int itemCount = grid.transform.childCount;
            for (int i = 0; i < itemCount; i++)
            {
                DestroyImmediate(grid.transform.GetChild(0).gameObject);
            }
        }
        /// <summary>
        /// 清除背包项目的方法
        /// </summary>
        void ClearItems()
        {
            foreach (var item in items)
            {
                item.button.onClick.RemoveAllListeners();
                Destroy(item.button.gameObject);
            }
        }
        /// <summary>
        /// 新增项目
        /// </summary>
        /// <param name="item">项目</param>
        public void AddItem(BagItemButton item)
        {
            ClearItems();
            items.Add(item);
            InstantiateItems();
            Flip(currentPage);
        }
        /// <summary>
        /// 移除项目
        /// </summary>
        /// <param name="item">项目</param>
        public void RemoveItem(BagItemButton item)
        {
            ClearItems();
            items.Remove(item);
            InstantiateItems();

            if (!Flip(currentPage))
                PreviousPage();
        }
        /// <summary>
        /// 更改按钮样式的方法
        /// </summary>
        /// <param name="index">要高亮的按钮，若为-1则不高亮任何按钮</param>
        void Highlight(int index)
        {
            //记录项目状态
            bool isSelected = false;
            if (index != -1)
                isSelected = items[index].isSelected;
            //如果不支持多选或index为-1，则先取消所有按钮高亮
            if (!multiple || index == -1)
                foreach (var item in items)
                {
                    //为Button替换统一的底图
                    SetButtonState(item, false, null, uniformNormalSprite);
                    //更换icon
                    if (item.normalSprite != null)
                        item.button.transform.GetChild(0).GetComponent<Image>().sprite = item.normalSprite;
                    //更换计数器字色
                    item.button.transform.GetChild(2).GetComponent<Text>().color = normalTextColor;
                }
            if (index == -1)
                return;
            //如果按钮为选中或不支持取消高亮，则高亮，否则取消高亮
            bool state = !isSelected || !normalizable;
            //为Button替换统一的底图
            SetButtonState(items[index], state, null, state ? uniformHighlightedSprite : uniformNormalSprite);
            //更换icon
            if (items[index].highlightedSprite != null && items[index].normalSprite != null)
                items[index].button.transform.GetChild(0).GetComponent<Image>().sprite = state ? items[index].highlightedSprite : items[index].normalSprite;
            //更换计数器字色
            items[index].button.transform.GetChild(2).GetComponent<Text>().color = state ? highlightedTextColor : normalTextColor;
        }
        /// <summary>
        /// 下一页方法
        /// </summary>
        public void NextPage()
        {
            if (currentPage < maxPage)
            {
                currentPage++;
                Flip(currentPage);
            }
        }
        /// <summary>
        /// 上一页方法
        /// </summary>
        public void PreviousPage()
        {
            if (currentPage > 1)
            {
                currentPage--;
                Flip(currentPage);
            }
        }
        /// <summary>
        /// 执行翻页逻辑的方法
        /// </summary>
        /// <param name="targetPage">目标页码</param>
        /// <returns>翻页是否成功</returns>
        bool Flip(int targetPage)
        {
            //计算页面尺寸和最大页数
            int pageSize = (int)(columnAndRow.x * columnAndRow.y);
            maxPage = (int)Mathf.Ceil((float)items.Count / pageSize);
            //如果页码无效则返回false
            if (targetPage < 1 || targetPage > maxPage)
                return false;
            //更新背包上的页码显示
            pageIndicator.text = currentPage + "/" + maxPage;
            //关闭所有项目
            foreach (var item in items)
            {
                item.button.gameObject.SetActive(false);
            }
            //计算当前页面的项目下标范围
            int startCount = (targetPage - 1) * pageSize;
            int endCount = targetPage * pageSize - 1;
            //打开当前页面下的所有项目
            for (int i = startCount; i <= endCount; i++)
            {
                //如果下标超出项目长度（即最后一页未填满），则返回
                if (i >= items.Count)
                    return true;
                items[i].button.gameObject.SetActive(true);
            }
            return true;
        }
        /// <summary>
        /// 修改项目计数的方法
        /// </summary>
        /// <param name="item">项目实例</param>
        /// <param name="count">计数</param>
        public void SetItemCount(BagItemButton item, int count)
        {
            if (count < 0)
            {
                Debug.LogError(item.label + "：无效的背包项目计数" + count);
                return;
            }
            item.count = count;
            item.button.transform.GetChild(2).GetComponent<Text>().text = "x" + count;
        }
        /// <summary>
        /// 根据名称获取背包项目的方法
        /// </summary>
        /// <param name="name">项目名称</param>
        /// <returns>背包项目对象</returns>
        public BagItemButton GetBagItem(string name)
        {
            foreach (var item in items)
            {
                if (name == item.label)
                    return item;
            }
            Debug.LogError("未找到背包项目");
            return null;
        }
        /// <summary>
        /// 供背包项目发布事件使用的开始拖拽方法
        /// </summary>
        /// <param name="d">事件数据</param>
        public void OnBeginDrag(BaseEventData d)
        {
            onBeginDrag?.Invoke(d.selectedObject.name, GetBagItem(d.selectedObject.name));
        }
        /// <summary>
        /// 供背包项目发布事件使用的停止拖拽方法
        /// </summary>
        /// <param name="d">事件数据</param>
        public void OnEndDrag(BaseEventData d)
        {
            if (d.selectedObject != null)
                onEndDrag?.Invoke(d.selectedObject.name, GetBagItem(d.selectedObject.name));
            else
                onEndDrag?.Invoke(null, null);
        }
    }
    /// <summary>
    /// 背包项目类
    /// </summary>
    [Serializable]
    public class BagItemButton : ModuleButton
    {
        /// <summary>
        /// 同种物品的数量
        /// </summary>
        public int count;
        /// <summary>
        /// 依据背包的特性，在通用按钮的基础上增加了模型字段，方便外部调用
        /// 若背包跨场景，则不推荐使用，可能会造成空引用异常
        /// </summary>
        public GameObject model;
    }
}
