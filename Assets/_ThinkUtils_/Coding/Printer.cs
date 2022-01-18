//处理控制台/屏幕Log信息打印的脚本

//Version:0.3

//By NeilianAryan

//2021_06_01 - 增加清除控制台方法；修改类名
//2021_05_25 - 增加普通Log支持；针对WebGL环境进行过滤
//2020_08_14

using UnityEngine;
using UnityEngine.UI;

namespace ThinkUtils
{
    public class Printer : Singleton<Printer>
    {
        /// <summary>
        /// 打印一段信息
        /// </summary>
        /// <param name="content">信息</param>
        public static void Log(object content)
        {
#if UNITY_EDITOR
            Debug.Log(content);
#endif
        }
        /// <summary>
        /// 打印一段自定义颜色的信息
        /// </summary>
        /// <param name="content">信息</param>
        /// <param name="color">颜色</param>
        public static void Log(object content, Color color)
        {
#if UNITY_EDITOR
            string code = ColorUtility.ToHtmlStringRGB(color);
            Debug.Log($"<color=#{code}>" + content + "</color>");
#endif
        }
        /// <summary>
        /// 打印一段警告信息
        /// </summary>
        /// <param name="content">信息</param>
        public static void LogWarning(object content)
        {
#if UNITY_EDITOR
            Debug.LogWarning(content);
#endif
        }
        /// <summary>
        /// 打印一段错误信息
        /// </summary>
        /// <param name="content">信息</param>
        public static void LogError(object content)
        {
#if UNITY_EDITOR
            Debug.LogError(content);
#endif
        }

        /// <summary>
        /// 负责显示打印信息的Text
        /// </summary>
        Text info;
        /// <summary>
        /// 初始化组件
        /// </summary>
        void InitialScreen()
        {
            if (info != null) return;
            info = GetComponent<Text>();
            info.text = "";
        }
        /// <summary>
        /// 将信息打印屏幕上
        /// </summary>
        /// <param name="content">信息内容</param>
        public void LogOnScreen(object content)
        {
            InitialScreen();
            info.text += content + "\n";
        }
        /// <summary>
        /// 清除屏幕上的打印信息
        /// </summary>
        public void ClearScreen()
        {
            InitialScreen();
            info.text = "";
        }
    }
}
