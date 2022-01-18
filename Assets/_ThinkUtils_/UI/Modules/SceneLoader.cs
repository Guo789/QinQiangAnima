//加载场景的脚本，自动处理了进度条逻辑，并可配置样式

//By NeilianAryan

using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace ThinkUtils.UI
{
    public class SceneLoader : Singleton<SceneLoader>
    {
        /// <summary>
        /// 是否展示提示信息
        /// </summary>
        public bool showTip = true;
        /// <summary>
        /// 动画类型
        /// </summary>
        public AnimType animType;
        /// <summary>
        /// 预设样式数组
        /// </summary>
        public SceneLoaderStyle[] styles;
        /// <summary>
        /// 页面
        /// </summary>
        Image page;
        /// <summary>
        /// 进度条
        /// </summary>
        Slider slider;
        /// <summary>
        /// 进度信息|提示信息
        /// </summary>
        Text progressText, tipText;
        // 三种动画类型
        InOutZoomer animZoomer;
        InOutFader animFader;
        InOutTransformer animTransformer;
        /// <summary>
        /// 异步加载场景操作
        /// </summary>
        AsyncOperation operation;
        /// <summary>
        /// 是否开始加载场景
        /// </summary>
        bool isLoadScene;
        /// <summary>
        /// 显示进度用变量
        /// </summary>
        int displayProgress = 0;
        /// <summary>
        /// 计时器
        /// </summary>
        float timer;
        /// <summary>
        /// 加载完成后的回调
        /// </summary>
        Action done;
        private void Awake()
        {
            //获取页面、进度条和信息文本，以及动画组件
            page = transform.GetChild(0).GetComponent<Image>();
            slider = page.transform.GetChild(0).GetComponent<Slider>();
            progressText = page.transform.GetChild(1).GetComponent<Text>();
            tipText = page.transform.GetChild(2).GetComponent<Text>();
            animZoomer = page.GetComponent<InOutZoomer>();
            animFader = page.GetComponent<InOutFader>();
            animTransformer = page.GetComponent<InOutTransformer>();
        }
        /// <summary>
        /// 加载场景（编辑器用）
        /// </summary>
        /// <param name="sceneName">场景名</param>
        public void LoadScene(string sceneName)
        {
            LoadScene(sceneName, "", null);
        }
        /// <summary>
        /// 加载场景
        /// </summary>
        /// <param name="sceneName">场景名</param>
        /// <param name="tip">提示信息</param>
        /// <param name="done">加载完成后的回调</param>
        public void LoadScene(string sceneName, string tip = "", Action done = null)
        {
            //如果显示提示信息则加载提示信息，否则为空字符串
            tipText.text = showTip ? tip : "";
            switch (animType)
            {
                case AnimType.None: page.gameObject.SetActive(true); StartCoroutine(Loader(sceneName)); break;
                case AnimType.Zoom: animZoomer.ZoomIn(() => StartCoroutine(Loader(sceneName))); break;
                case AnimType.Fade: animFader.FadeIn(() => StartCoroutine(Loader(sceneName))); break;
                case AnimType.Transform: animTransformer.TransformIn(() => StartCoroutine(Loader(sceneName))); break;
            }
            this.done = done;
        }
        /// <summary>
        /// 加载场景并指定样式
        /// </summary>
        /// <param name="sceneName">场景名</param>
        /// <param name="styleIndex">样式编号</param>
        /// <param name="tip">提示信息</param>
        /// <param name="done">加载完成后的回调</param>
        public void LoadScene(string sceneName, int styleIndex, string tip = "", Action done = null)
        {
            page.sprite = styles[styleIndex].page;
            slider.transform.GetChild(0).GetComponent<Image>().sprite = styles[styleIndex].sliderBack;
            slider.fillRect.GetComponent<Image>().sprite = styles[styleIndex].sliderFill;
            LoadScene(sceneName, tip, done);
        }
        /// <summary>
        /// 异步加载场景
        /// </summary>
        /// <param name="name">场景名</param>
        /// <param name="done">加载完成后的回调</param>
        /// <returns></returns>
        IEnumerator Loader(string name)
        {
            operation = SceneManager.LoadSceneAsync(name);
            //关闭自动进入场景并重置进度
            operation.allowSceneActivation = false;
            isLoadScene = true;
            displayProgress = 0;
            yield return operation;
        }
        private void Update()
        {
            if (isLoadScene)
            {
                timer += Time.deltaTime;
                //SceneManager加载到90后会直接完成，因此分段判断，90以下时进度变量追逐实际进度，90以上则逐个向100靠拢
                if (operation.progress < 0.9f)
                {
                    if (displayProgress < operation.progress * 100)
                    {
                        displayProgress++;
                        slider.value = displayProgress;
                        progressText.text = displayProgress + "%";
                    }
                }
                else if (displayProgress < 100)
                {
                    displayProgress++;
                    slider.value = displayProgress;
                    progressText.text = displayProgress + "%";
                }
                else if (timer > 3)
                {
                    timer = 0;
                    isLoadScene = false;
                    operation.allowSceneActivation = true;
                    done?.Invoke();
                    switch (animType)
                    {
                        case AnimType.None: page.gameObject.SetActive(false); break;
                        case AnimType.Zoom: animZoomer.ZoomOut(); break;
                        case AnimType.Fade: animFader.FadeOut(); break;
                        case AnimType.Transform: animTransformer.TransformOut(); break;
                    }
                }
            }
        }
    }
    /// <summary>
    /// 用于保存加载页样式的类
    /// </summary>
    [Serializable]
    public class SceneLoaderStyle
    {
        /// <summary>
        /// 样式名称，方便区分辨认
        /// </summary>
        public string styleName;
        /// <summary>
        /// 页面|进度条背景|进度条填充
        /// </summary>
        public Sprite page, sliderBack, sliderFill;
    }
}
