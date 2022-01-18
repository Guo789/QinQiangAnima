//用于管理UI中多语言的文本替换脚本

//Version:0.4

//By NeilianAryan

//2021_06_10 - 调整打印信息并提供设置指定文本块语言的方法
//2021_06_01 - 调整语言切换时机，从开始时主动改为调用时被动
//2021_05_21 - 增加配置内容匹配检测
//2021_04_13

using System;
using UnityEngine;
using UnityEngine.UI;

namespace ThinkUtils.UI
{
    public class UILanguageManager : Singleton<UILanguageManager>
    {
        /// <summary>
        /// 当前语言
        /// </summary>
        public Language language;
        /// <summary>
        /// 是否启用多语言
        /// </summary>
        public bool useMultilanguage;
        /// <summary>
        /// 分割中英的字符
        /// </summary>
        [Tooltip("分割中英的字符")]
        public char splittingChar = '$';
        /// <summary>
        /// 换行的字符(仅用于动态文本)
        /// </summary>
        [Tooltip("用于动态文本的自定义换行符")]
        public char wrappingChar = '&';
        /// <summary>
        /// 受管理的静态文本块
        /// </summary>
        [Tooltip("受管理的静态文本块")]
        public LanguageText[] allText;
        /// <summary>
        /// 设置语言
        /// </summary>
        /// <param name="language">语言编号</param>
        public void SetLanguage(int language)
        {
            SetLanguage((Language)language);
        }
        /// <summary>
        /// 设置语言
        /// </summary>
        /// <param name="language">语言</param>
        public void SetLanguage(Language language)
        {
            this.language = language;
            for (int i = 0; i < allText.Length; i++)
            {
                SetSpecificLanguage(i);
            }
        }
        /// <summary>
        /// 设置指定文本块的语言
        /// </summary>
        /// <param name="index">文本块编号</param>
        public void SetSpecificLanguage(int index)
        {
            SetSpecificLanguage(allText[index]);
        }
        /// <summary>
        /// 设置指定文本块的语言
        /// </summary>
        /// <param name="text">文本块</param>
        public void SetSpecificLanguage(LanguageText text)
        {
            string[] allLines = text.textAsset.text.Split('\n');
            Text[] allTextComponents = text.textParent.GetComponentsInChildren<Text>(true);
            if (allLines.Length != allTextComponents.Length)
            {
                Printer.Log(text.textAsset.name + "内容行数(" + allLines.Length + ")与Text数目(" + allTextComponents.Length + ")不符，请检查配置txt", Color.red);
                return;
            }
            for (int i = 0; i < allTextComponents.Length; i++)
            {
                string[] allRes = allLines[i].Split(splittingChar);
                allTextComponents[i].text = allRes[(int)language];
            }
        }
        /// <summary>
        /// 获得指定文本中的指定行文本
        /// </summary>
        /// <param name="textAssetName">文本名称</param>
        /// <param name="index">行编号</param>
        /// <param name="result">结果</param>
        public void GetTextContent(string textAssetName, int index, Action<string> result)
        {
            GetTextContent(textAssetName, res =>
            {
                string[] allLines = res.Split(wrappingChar);
                string[] allRes = allLines[index].Split(splittingChar);
                string finalRes = allRes[(int)language].Substring(language == 0 && index != 0 ? 2 : 0);
                result?.Invoke(finalRes);
            });
        }
        /// <summary>
        /// 从文本中请求内容
        /// </summary>
        /// <param name="textAssetName">文本名</param>
        /// <param name="result">结果</param>
        void GetTextContent(string textAssetName, Action<string> result)
        {
            string textRelativePath = Application.streamingAssetsPath + "/ConfigText/" + textAssetName + ".txt";
            WebRequester.Instance.GET(textRelativePath, res =>
            {
                result?.Invoke(res);
            }, null, null);
        }
    }
    public enum Language
    {
        Chinese, English
    }
    [Serializable]
    public class LanguageText
    {
        /// <summary>
        /// 语言配置文本
        /// </summary>
        public TextAsset textAsset;
        /// <summary>
        /// Text文本块父物体
        /// </summary>
        public GameObject textParent;
    }
}
