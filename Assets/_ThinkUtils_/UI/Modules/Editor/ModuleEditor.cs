using UnityEditor;

namespace ThinkUtils.UI
{
    public class ModuleEditor : BaseEditor
    {
        [MenuItem("GameObject/ThinkUtils/UI模块/通用导航栏", false, -1)]
        static void GenerateSimpleNav()
        {
            GenerateUIModules("SimpleNav");
        }
        [MenuItem("GameObject/ThinkUtils/UI模块/可折叠导航栏", false, -1)]
        static void GenerateFoldableNav()
        {
            GenerateUIModules("FoldableNav");
        }
        [MenuItem("GameObject/ThinkUtils/UI模块/背包", false, -1)]
        static void GenerateBag()
        {
            GenerateUIModules("Bag");
        }
        [MenuItem("GameObject/ThinkUtils/UI模块/悬浮窗", false, -1)]
        static void GenerateHoverWindow()
        {
            GenerateUIModules("HoverWindow");
        }
        [MenuItem("GameObject/ThinkUtils/UI模块/消息弹窗", false, -1)]
        static void GenerateMessenger()
        {
            GenerateUIModules("Messenger");
        }
        [MenuItem("GameObject/ThinkUtils/UI模块/进度条", false, -1)]
        static void GenerateProceeder()
        {
            GenerateUIModules("Proceeder");
        }
        [MenuItem("GameObject/ThinkUtils/UI模块/场景加载器", false, -1)]
        static void GenerateSceneLoader()
        {
            GenerateUIModules("SceneLoader");
        }
        [MenuItem("GameObject/ThinkUtils/UI模块/翻页器", false, -1)]
        static void GenerateFliper()
        {
            GenerateUIModules("Fliper");
        }
        [MenuItem("GameObject/ThinkUtils/UI模块/自定义按钮", false, -1)]
        static void GenerateFuncButton()
        {
            GenerateUIModules("FuncButton");
        }
        [MenuItem("GameObject/ThinkUtils/UI模块/屏幕控制台", false, -1)]
        static void GenerateScreenConsole()
        {
            GenerateUIModules("ScreenConsole");
        }
        static void GenerateUIModules(string name)
        {
            GenerateMethod("UI/Modules/Resources/" + name);
        }
    }
}
