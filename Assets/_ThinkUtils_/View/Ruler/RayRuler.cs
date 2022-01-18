//用于生成测距标尺的脚本

//Version:0.4

//By NeilianAryan

//2021_03_26 - 增加调试模式用控制以快速展示功能
//2021_03_15 - 对绝对线框进行标签去重；增加水平对角线
//2021_02_28 - 优化代码，增加绝对线框模式，增加并区分UI标签与世界标签，完善面板可配置项，自动处理渲染相机逻辑
//2021_01_08

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ThinkUtils
{
    public class RayRuler : Singleton<RayRuler>
    {
        /// <summary>
        /// 渲染用相机
        /// </summary>
        public Camera viewCamera;
        /// <summary>
        /// 光标尺寸|线条尺寸|世界标签尺寸
        /// </summary>
        public float cursorSize = 0.01f, lineSize = 0.007f, labelSizeInWorld = 0.2f;
        /// <summary>
        /// 标尺颜色|标签背景色|标签字色
        /// </summary>
        public Color rulerColor, labelBG, labelText;
        /// <summary>
        /// 是否使用世界标签｜绝对线框模式
        /// </summary>
        public bool useWorldLabel, absFrameMode;
        /// <summary>
        /// 标尺原件|标签原件
        /// </summary>
        public GameObject rulerOrigin, labelOrigin;
        /// <summary>
        /// 标尺使用的材质
        /// </summary>
        public Material rulerMat;
        /// <summary>
        /// 渲染线条的相机|渲染世界标签的相机
        /// </summary>
        public Transform lineCamera, worldLabelCamera;
        /// <summary>
        /// 所有标尺
        /// </summary>
        /// <typeparam name="Ruler">标尺类</typeparam>
        /// <returns></returns>
        List<Ruler> allRuler = new List<Ruler>();
        /// <summary>
        /// 当前标尺
        /// </summary>
        Ruler currentRuler;
        /// <summary>
        /// 检测射线
        /// </summary>
        Ray ray;
        /// <summary>
        /// 射线结果
        /// </summary>
        RaycastHit hit;
        /// <summary>
        /// 是否处于测量模式|是否锚定一个点
        /// </summary>
        bool ruleMode, isPointed;
        /// <summary>
        /// 是否使用测试用控制方式
        /// </summary>
        public bool debugControl;
        private void Awake()
        {
            //如果未设置viewCamera则默认为MainCamera，并添加渲染线条和标签的相机
            if (viewCamera == null)
                viewCamera = Camera.main;
            InitialRenderCamera(lineCamera);
            InitialRenderCamera(worldLabelCamera);
            //初始化标尺属性
            rulerOrigin.GetComponent<LineRenderer>().startWidth = lineSize;
            rulerOrigin.GetComponent<LineRenderer>().endWidth = lineSize;
            rulerOrigin.transform.GetChild(1).localScale = Vector3.one * labelSizeInWorld;

            rulerMat.color = rulerColor;
            rulerOrigin.GetComponentInChildren<Image>(true).color = labelBG;
            rulerOrigin.GetComponentInChildren<Text>(true).color = labelText;
            labelOrigin.GetComponentInChildren<Image>(true).color = labelBG;
            labelOrigin.GetComponentInChildren<Text>(true).color = labelText;
        }
        private void Update()
        {
            if (debugControl)
            {
                UpdateRule(viewCamera.ScreenPointToRay(Input.mousePosition));
                if (Input.GetKeyDown(KeyCode.R))
                    Rule(!ruleMode);
                if (Input.GetMouseButtonDown(0))
                    Point();
            }
        }
        /// <summary>
        /// 初始化渲染相机的方法
        /// </summary>
        /// <param name="camera"></param>
        void InitialRenderCamera(Transform camera)
        {
            camera.SetParent(viewCamera.transform);
            camera.localPosition = camera.localEulerAngles = Vector3.zero;
            camera.localScale = Vector3.one;
            camera.gameObject.SetActive(true);
        }
        /// <summary>
        /// 进入/退出测量模式的方法
        /// </summary>
        /// <param name="ifRule">是否进入测量模式</param>
        public void Rule(bool ifRule)
        {
            //若已经进入或退出，则返回
            if (ifRule == ruleMode) return;
            //更新模式
            ruleMode = ifRule;
            //若退出测量模式，则删除所有标尺
            if (!ifRule)
            {
                foreach (var item in allRuler)
                {
                    Destroy(item.line);
                    Destroy(item.label);
                }
                allRuler.Clear();
                return;
            }
            //若未锚定点，则生成一个待测标尺
            if (!isPointed)
            {
                Ruler newRuler = new Ruler(Instantiate(rulerOrigin), Instantiate(labelOrigin, labelOrigin.transform.parent));
                newRuler.line.transform.localScale = Vector3.one * cursorSize;
                allRuler.Add(newRuler);
                currentRuler = newRuler;
            }
        }
        /// <summary>
        /// 手动锚定标尺点的方法
        /// </summary>
        public void Point()
        {
            //若不在测量模式或射线未检测到点则返回
            if (!ruleMode || !Physics.Raycast(ray, out hit)) return;
            //实用锚定方法
            Anchor(true, hit.point, hit.transform, true);
            //如果为绝对坐标线框模式，则在闭合后生成线框
            if (!isPointed && absFrameMode)
                GenerateAbsFrame();
        }
        /// <summary>
        /// 更新标尺的方法，须实时刷新
        /// </summary>
        /// <param name="ray">提供给标尺用于锚定锚点的射线</param>
        public void UpdateRule(Ray ray)
        {
            //若不在测量模式则返回
            if (!ruleMode) return;
            //更新射线相关信息
            this.ray = ray;
            bool ifRaycast = Physics.Raycast(ray, out hit);
            Anchor(ifRaycast, hit.point, hit.transform, false);
            //持续更新线条位置，与标尺保持一致，并令信息面板持续看向相机
            foreach (var item in allRuler)
            {
                Vector3 startPos = item.line.transform.position;
                Vector3 endPos = item.endPoint.position;
                Vector3 middlePos = Geometry.GetCenterPoint(startPos, endPos);
                item.line.GetComponent<LineRenderer>().SetPosition(0, startPos);
                item.line.GetComponent<LineRenderer>().SetPosition(1, endPos);
                if (useWorldLabel)
                {
                    item.worldLabel.transform.position = middlePos;
                    item.worldLabel.transform.LookAt(viewCamera.transform);
                }
                else
                    item.label.transform.position = viewCamera.WorldToScreenPoint(middlePos);
            }
        }
        /// <summary>
        /// 生成一条标尺线的方法
        /// </summary>
        /// <param name="start">起始点</param>
        /// <param name="end">结束点</param>
        /// <param name="lineParent">标尺线的父物体</param>
        public void DrawALine(Vector3 start, Vector3 end, Transform lineParent, bool showLabel = true)
        {
            //如果不在测量模式或已经锚定一个点，则禁止画线方法
            if (!ruleMode || isPointed) return;
            Anchor(true, start, lineParent, true, showLabel);
            Anchor(true, end, lineParent, true, showLabel);
        }
        /// <summary>
        /// 内部用锚定工具方法
        /// </summary>
        /// <param name="existPoint">是否存在目标点，用于射线未hit时跳过后续代码</param>
        /// <param name="targetPosition">目标点位置</param>
        /// <param name="targetTransform">目标点Transform</param>
        /// <param name="ifAnchor">是否锚定</param>
        void Anchor(bool existPoint, Vector3 targetPosition, Transform targetTransform, bool ifAnchor, bool showLabel = true)
        {
            //如果未锚定一个点，则首先设置第一个点的位置和标尺的父物体，并在确定锚定时锚定
            if (!isPointed)
            {
                currentRuler.line.SetActive(existPoint);
                if (!existPoint) return;
                currentRuler.line.transform.position = targetPosition;
                currentRuler.line.transform.SetParent(targetTransform);
                currentRuler.line.GetComponent<LineRenderer>().SetPosition(0, targetPosition);
                if (ifAnchor)
                    isPointed = true;
            }
            //如果已经锚定第一个点，则设置第二个点及标尺线的位置，并在确定锚定时闭合标尺，生成新的待测标尺
            else
            {
                currentRuler.endPoint.gameObject.SetActive(existPoint);
                currentRuler.line.GetComponent<LineRenderer>().enabled = existPoint;
                if (useWorldLabel)
                    currentRuler.worldLabel.SetActive(existPoint && showLabel);
                else
                    currentRuler.label.SetActive(existPoint && showLabel);
                if (!existPoint) return;
                currentRuler.endPoint.position = targetPosition;
                currentRuler.line.GetComponent<LineRenderer>().SetPosition(1, targetPosition);
                string rulerValue = Vector3.Distance(currentRuler.line.transform.position, targetPosition).ToString("0.00") + "m";
                currentRuler.worldLabel.GetComponentInChildren<Text>(true).text = rulerValue;
                currentRuler.label.GetComponentInChildren<Text>(true).text = rulerValue;
                if (ifAnchor)
                {
                    isPointed = false;
                    ruleMode = false;
                    Rule(true);
                }
            }
        }
        /// <summary>
        /// 生成绝对坐标线框的方法
        /// </summary>
        void GenerateAbsFrame()
        {
            Ruler lastRuler = allRuler[allRuler.Count - 2];
            Transform lastResult = lastRuler.line.transform;
            Vector3 lastStartPosition = lastResult.position;
            Vector3 lastEndPostion = lastRuler.endPoint.position;
            //从标尺起始点延伸的三条线
            DrawALine(lastStartPosition, new Vector3(lastEndPostion.x, lastStartPosition.y, lastStartPosition.z), lastResult);
            DrawALine(lastStartPosition, new Vector3(lastStartPosition.x, lastEndPostion.y, lastStartPosition.z), lastResult);
            DrawALine(lastStartPosition, new Vector3(lastStartPosition.x, lastStartPosition.y, lastEndPostion.z), lastResult);
            //从标尺结束点延伸的三条线
            DrawALine(lastEndPostion, new Vector3(lastStartPosition.x, lastEndPostion.y, lastEndPostion.z), lastResult, false);
            DrawALine(lastEndPostion, new Vector3(lastEndPostion.x, lastStartPosition.y, lastEndPostion.z), lastResult, false);
            DrawALine(lastEndPostion, new Vector3(lastEndPostion.x, lastEndPostion.y, lastStartPosition.z), lastResult, false);
            //各延伸线之间的连线
            DrawALine(new Vector3(lastEndPostion.x, lastStartPosition.y, lastStartPosition.z), new Vector3(lastEndPostion.x, lastStartPosition.y, lastEndPostion.z), lastResult, false);
            DrawALine(new Vector3(lastEndPostion.x, lastStartPosition.y, lastStartPosition.z), new Vector3(lastEndPostion.x, lastEndPostion.y, lastStartPosition.z), lastResult, false);

            DrawALine(new Vector3(lastStartPosition.x, lastEndPostion.y, lastStartPosition.z), new Vector3(lastStartPosition.x, lastEndPostion.y, lastEndPostion.z), lastResult, false);
            DrawALine(new Vector3(lastStartPosition.x, lastEndPostion.y, lastStartPosition.z), new Vector3(lastEndPostion.x, lastEndPostion.y, lastStartPosition.z), lastResult, false);

            DrawALine(new Vector3(lastStartPosition.x, lastStartPosition.y, lastEndPostion.z), new Vector3(lastStartPosition.x, lastEndPostion.y, lastEndPostion.z), lastResult, false);
            DrawALine(new Vector3(lastStartPosition.x, lastStartPosition.y, lastEndPostion.z), new Vector3(lastEndPostion.x, lastStartPosition.y, lastEndPostion.z), lastResult, false);
            //起点到终点的水平投影
            DrawALine(lastStartPosition, new Vector3(lastEndPostion.x, lastStartPosition.y, lastEndPostion.z), lastResult);
        }
    }
    /// <summary>
    /// 标尺类
    /// </summary>
    class Ruler
    {
        /// <summary>
        /// 标尺线
        /// </summary>
        public GameObject line;
        /// <summary>
        /// 标尺闭合点
        /// </summary>
        public Transform endPoint;
        /// <summary>
        /// 标尺标签
        /// </summary>
        public GameObject label;
        /// <summary>
        /// 世界标尺标签
        /// </summary>
        public GameObject worldLabel;
        public Ruler(GameObject line, GameObject label)
        {
            this.line = line;
            endPoint = line.transform.GetChild(0);
            this.label = label;
            worldLabel = line.transform.GetChild(1).gameObject;
        }
    }
}