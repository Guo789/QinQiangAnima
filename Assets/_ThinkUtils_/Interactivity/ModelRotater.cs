//用于旋转模型的脚本

//Version:0.2

//By 石中华

//2121_05_26 - 修改旋转实现方式，修复BUG
//2021_05_25

using UnityEngine;
using ThinkUtils;
public class ModelRotater : Singleton<ModelRotater>
{
    private Transform model;
    private bool enable;
    public float rotateSpeed;
    private void Update()
    {
        if (enable && Input.GetMouseButton(1))
        {
            model.Rotate(Camera.main.transform.up, -Input.GetAxis("Mouse X") * rotateSpeed, Space.World);
            model.Rotate(Camera.main.transform.right, Input.GetAxis("Mouse Y") * rotateSpeed, Space.World);
        }
    }
    /// <summary>
    /// 开启旋转模式
    /// </summary>
    /// <param name="model">待旋转模型</param>
    public void StartRotate(Transform model)
    {
        this.model = model;
        enable = true;
    }
    /// <summary>
    /// 停止旋转模式
    /// </summary>
    /// <param name="originEuler">模型旋转重置值</param>
    public void StopRotate(Vector3 originEuler)
    {
        if (model == null) return;
        model.eulerAngles = originEuler;
        enable = false;
        model = null;
    }
}
