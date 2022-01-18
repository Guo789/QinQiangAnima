using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;

namespace ThinkUtils
{
    public class BaseEditor : MonoBehaviour
    {
        /// <summary>
        /// 在Scene中生成预制体的方法
        /// </summary>
        /// <param name="prefab">预制体名称</param>
        /// <param name="isUnique">是否唯一</param>
        public static void GenerateMethod(string prefab, bool isUnique = false)
        {
            string prefabName = Path.GetFileName(prefab);
            if (isUnique)
                foreach (var item in SceneManager.GetActiveScene().GetRootGameObjects())
                {
                    if (item.name == prefabName)
                        DestroyImmediate(item);
                }
            GameObject newObj = Instantiate(AssetDatabase.LoadAssetAtPath<GameObject>("Assets/_ThinkUtils_/" + prefab + ".prefab"));
            newObj.name = prefabName;
            if (Selection.gameObjects.Length == 0)
            {
                newObj.transform.SetSiblingIndex(SceneManager.GetActiveScene().GetRootGameObjects().Length - 1);
                newObj.transform.position = Vector3.zero;
            }
            else
            {
                newObj.transform.SetParent(Selection.gameObjects[0].transform);
                if (newObj.GetComponent<RectTransform>() != null)
                    newObj.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                else
                    newObj.transform.localPosition = Vector3.zero;
            }
            newObj.transform.localScale = Vector3.one;
        }
    }
}
