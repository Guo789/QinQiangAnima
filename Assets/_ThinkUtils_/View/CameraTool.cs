//用于控制镜头运动的脚本

//Version:1.1

//By NeilianAryan

//2021_03_02 - 区分环架模式拖移速度和滚动速度

//2021_02_05 - 1.0正式版
//包含切镜、运镜、环架、FPS四个模块，并具有环架平移、FPS跳跃蹲伏、通用角度限制等功能
#region 0.x Log
//2021_02_05 - 修复FPS下跳跃蹲伏不能稳定触发的BUG
//2021_01_25 - 统一速度属性；优化环架模式移动机制
//2021_01_14 - 环架模式增加缩放和移动功能
//2021_01_13 - 相机移动方法兼容FPS模式
//2021_01_11 - 将环架控制内嵌到脚本中；将相机运动刷新方式改为fixed
//2020_12_24 - FPS控制模块增加蹲伏功能
//2020_12_16 - FPS控制模块增加跳跃功能
//2020_12_15 - 优化FPS初始化模式，增加飞行模式
//2020_12_12 - 增加FPS控制模式
//2020_12_10 - 包括切镜、运镜、环架控制模式
#endregion

using System;
using System.Collections;
using UnityEngine;

namespace ThinkUtils
{
    public class CameraTool : Singleton<CameraTool>
    {
        private void Awake()
        {
            //如果未设置targetCamera则默认为MainCamera
            if (targetCamera == null)
                targetCamera = Camera.main.transform;
        }
        /// <summary>
        /// 被控制的相机
        /// </summary>
        public Transform targetCamera;


        /// <summary>
        /// 目标位置
        /// </summary>
        Pose targetPose;
        /// <summary>
        /// 插值度
        /// </summary>
        public float lerpValue = 0.05f;
        /// <summary>
        /// 移动用协程，暂存以在再次调用时终止
        /// </summary>
        Coroutine lerpCoroutine;


        /// <summary>
        /// 环架近点距离|环架远点距离，仅在移动功能关闭时生效
        /// </summary>
        public float nearDistance = 0.1f, farDistance = 1;
        /// <summary>
        /// 是否进入环架模式|环架是否可移动
        /// </summary>
        bool aroundViewMode, aroundMovable;
        /// <summary>
        /// 是否处于环架模式
        /// </summary>
        public bool IsAroundViewMode { get { return aroundViewMode; } }
        /// <summary>
        /// 环架模式的中心点和目标点
        /// </summary>
        Transform centerPoint, targetPoint;


        /// <summary>
        /// 是否进入第一人称移动模式
        /// </summary>
        bool fpsMode;
        /// <summary>
        /// 是否处于第一人称移动模式
        /// </summary>
        public bool IsFPSMode { get { return fpsMode; } }
        /// <summary>
        /// 是否支持跳跃和蹲伏
        /// </summary>
        public bool jumpCrouch;
        /// <summary>
        /// 第一人称模式时相机的父物体
        /// </summary>
        GameObject cameraBody;
        /// <summary>
        /// 身高
        /// </summary>
        float height;
        /// <summary>
        /// 是否进入飞行模式
        /// </summary>
        bool flyMode;


        /// <summary>
        /// 环架模式/fps模式移动速度|旋转速度|环架模式滚轮速度
        /// </summary>
        public float movingSpeed = 0.4f, rotatingSpeed = 4, wheelingSpeed = 2;
        /// <summary>
        /// 环架模式/fps模式最大的俯仰角度
        /// </summary>
        public float maxPitchAngle = 45;
        /// <summary>
        /// 环架模式/fps模式俯仰角记录变量
        /// </summary>
        float pitchAngleRecorder;

        //通过检测刚体速度进行跳跃限制
        bool canJump;
        private void FixedUpdate()
        {
            if (fpsMode && !flyMode)
            {
                canJump = cameraBody.GetComponent<Rigidbody>().velocity == Vector3.zero;
                //重力矫正
                if (!canJump)
                    cameraBody.GetComponent<Rigidbody>().AddForce(Vector3.down * 2000);
            }
        }

        /// <summary>
        /// 立即设置相机姿态
        /// </summary>
        /// <param name="pose">姿态</param>
        public void SetPose(Transform pose)
        {
            SetPose(new Pose(pose.position, pose.rotation));
        }
        /// <summary>
        /// 立即设置相机姿态
        /// </summary>
        /// <param name="pose">姿态</param>
        public void SetPose(Pose pose)
        {
            if (lerpCoroutine != null)
                StopCoroutine(lerpCoroutine);
            if (IsFPSMode)
            {
                cameraBody.transform.position = pose.position;
                cameraBody.transform.eulerAngles = Vector3.up * pose.rotation.eulerAngles.y;
            }
            else
            {
                targetCamera.position = pose.position;
                targetCamera.rotation = pose.rotation;
            }
        }
        /// <summary>
        /// 运镜设置相机姿态
        /// </summary>
        /// <param name="pose">姿态</param>
        /// <param name="done">回调函数</param>
        public void SetPose(Transform pose, Action done)
        {
            SetPose(new Pose(pose.position, pose.rotation), done);
        }
        /// <summary>
        /// 运镜设置相机姿态
        /// </summary>
        /// <param name="pose">姿态</param>
        /// <param name="done">回调函数</param>
        public void SetPose(Pose pose, Action done)
        {
            targetPose = pose;
            if (lerpCoroutine != null)
                StopCoroutine(lerpCoroutine);
            lerpCoroutine = StartCoroutine(LerpMove(done));
        }
        /// <summary>
        /// 相机插值移动到目标点
        /// </summary>
        /// <param name="pose">目标点</param>
        /// <param name="done">回调函数</param>
        IEnumerator LerpMove(Action done)
        {
            while (Vector3.Distance(targetCamera.position, targetPose.position) > 0.001f)
            {
                if (IsFPSMode)
                {
                    cameraBody.transform.position = Vector3.Lerp(cameraBody.transform.position, targetPose.position, lerpValue);
                    cameraBody.transform.eulerAngles = Vector3.Lerp(cameraBody.transform.eulerAngles, Vector3.up * targetPose.rotation.eulerAngles.y, lerpValue);
                }
                else
                {
                    targetCamera.position = Vector3.Lerp(targetCamera.position, targetPose.position, lerpValue);
                    targetCamera.rotation = Quaternion.Lerp(targetCamera.rotation, targetPose.rotation, lerpValue);
                }
                yield return new WaitForFixedUpdate();
            }
            if (IsFPSMode)
            {
                cameraBody.transform.position = targetPose.position;
                cameraBody.transform.eulerAngles = Vector3.up * targetPose.rotation.eulerAngles.y;
            }
            else
            {
                targetCamera.position = targetPose.position;
                targetCamera.rotation = targetPose.rotation;
            }
            done?.Invoke();
        }

        /// <summary>
        /// 开启相机环架模式
        /// </summary>
        /// <param name="centerPoint">注视中心点</param>
        /// <param name="targetPoint">相机lerp追踪点</param>
        /// <param name="maxAngle">最大俯仰角</param>
        public void StartAroundView(Transform centerPoint, Transform targetPoint, float maxAngle = 45, bool aroundMovable = false)
        {
            if (aroundViewMode) return;
            this.centerPoint = centerPoint;
            this.targetPoint = targetPoint;
            maxPitchAngle = maxAngle;
            this.aroundMovable = aroundMovable;
            StartCoroutine(AroundView());
        }
        /// <summary>
        /// 关闭相机环架模式
        /// </summary>
        public void StopAroundView()
        {
            aroundViewMode = false;
            pitchAngleRecorder = 0;
        }
        /// <summary>
        /// 相机环架移动
        /// </summary>
        IEnumerator AroundView()
        {
            aroundViewMode = true;
            while (aroundViewMode)
            {
                //按住鼠标右键旋转
                if (Input.GetMouseButton(1))
                {
                    Vector3 delta = new Vector3(-Input.GetAxis("Mouse Y"), Input.GetAxis("Mouse X")) * rotatingSpeed;
                    targetPoint.RotateAround(centerPoint.position, centerPoint.up, delta.y);
                    targetPoint.LookAt(centerPoint);
                    if (!isOverAngle(delta.x))
                    {
                        pitchAngleRecorder += delta.x;
                        targetPoint.RotateAround(centerPoint.position, targetPoint.right, delta.x);
                    }
                }
                //按住鼠标滚轮平移
                if (Input.GetMouseButton(2) && aroundMovable)
                {
                    centerPoint.position -= targetCamera.right * Input.GetAxis("Mouse X") * movingSpeed;
                    centerPoint.position -= targetCamera.up * Input.GetAxis("Mouse Y") * movingSpeed;
                    targetPoint.position -= targetCamera.right * Input.GetAxis("Mouse X") * movingSpeed;
                    targetPoint.position -= targetCamera.up * Input.GetAxis("Mouse Y") * movingSpeed;
                }
                //滚动鼠标滚轮推拉相机或纵深移动
                float scrollWheel = Input.GetAxis("Mouse ScrollWheel");
                float CTDistance = Vector3.Distance(centerPoint.position, targetPoint.position);
                if (aroundMovable)
                    centerPoint.transform.position += targetPoint.forward * scrollWheel * wheelingSpeed;
                if (aroundMovable || scrollWheel > 0 && CTDistance > nearDistance || scrollWheel < 0 && CTDistance < farDistance)
                    targetPoint.Translate(Vector3.forward * scrollWheel * wheelingSpeed, Space.Self);
                //相机追踪目标点
                targetCamera.position = Vector3.Slerp(targetCamera.position, targetPoint.position, lerpValue * 4);
                targetCamera.rotation = Quaternion.Slerp(targetCamera.rotation, targetPoint.rotation, lerpValue * 4);
                yield return new WaitForFixedUpdate();
            }
        }

        /// <summary>
        /// 开启第一人称移动模式
        /// </summary>
        /// <param name="movingSpeed">移动速度</param>
        /// <param name="canFly">是否可飞行</param>
        /// <param name="height">身高</param>
        /// <param name="radius">半径</param>
        public void StartFPSMove(bool flyMode = false, float height = 1.74f, float radius = 0.5f)
        {
            if (fpsMode) return;
            fpsMode = true;
            //创建空物体，并设置初始姿态
            cameraBody = new GameObject("CameraBody");
            cameraBody.transform.position = targetCamera.position;
            cameraBody.transform.eulerAngles = Vector3.up * targetCamera.eulerAngles.y;
            //添加并设置刚体
            Rigidbody rigidbody = cameraBody.AddComponent<Rigidbody>();
            rigidbody.mass = 55;
            rigidbody.drag = 10;
            rigidbody.useGravity = !flyMode;
            rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
            //添加并设置碰撞器
            BoxCollider collider = cameraBody.AddComponent<BoxCollider>();
            collider.size = new Vector3(radius, height, radius);
            //将相机放置于空物体下，并设置初始姿态
            targetCamera.SetParent(cameraBody.transform);
            targetCamera.localPosition = Vector3.up * height / 2;
            targetCamera.localEulerAngles = Vector3.right * targetCamera.eulerAngles.x;
            this.height = height;
            //设置飞行模式
            this.flyMode = flyMode;
            StartCoroutine(FPSMove());
        }
        /// <summary>
        /// 关闭第一人称移动模式
        /// </summary>
        public void StopFPSMove()
        {
            fpsMode = false;
            targetCamera.parent = null;
            Destroy(cameraBody);
            pitchAngleRecorder = 0;
        }
        /// <summary>
        /// 切换飞行模式
        /// </summary>
        /// <param name="flyMode"></param>
        public void Fly(bool flyMode)
        {
            if (!fpsMode) return;
            this.flyMode = flyMode;
            cameraBody.GetComponent<Rigidbody>().useGravity = !flyMode;
        }
        /// <summary>
        /// 第一人称控制
        /// </summary>
        /// <returns></returns>
        IEnumerator FPSMove()
        {
            while (fpsMode)
            {
                yield return new WaitForFixedUpdate();
                //设置相机移动
                float xSpeed = Input.GetAxis("Horizontal") * Time.fixedDeltaTime * movingSpeed;
                float zSpeed = Input.GetAxis("Vertical") * Time.fixedDeltaTime * movingSpeed;
                if (flyMode)
                    cameraBody.transform.Translate(xSpeed, 0, zSpeed, targetCamera.transform);
                else
                    cameraBody.transform.Translate(xSpeed, 0, zSpeed, Space.Self);
                //飞行模式下的升降以及行走模式下的跳跃蹲伏
                if (flyMode)
                {
                    if (Input.GetKey(KeyCode.E))
                        cameraBody.transform.Translate(0, Time.fixedDeltaTime * movingSpeed, 0, Space.Self);
                    if (Input.GetKey(KeyCode.Q))
                        cameraBody.transform.Translate(0, -Time.fixedDeltaTime * movingSpeed, 0, Space.Self);
                }
                else
                {
                    if (jumpCrouch)
                    {
                        if (Input.GetKey(KeyCode.Space) && canJump)
                            cameraBody.GetComponent<Rigidbody>().AddForce(Vector3.up * 30000);
                        if (Input.GetKey(KeyCode.LeftControl))
                            targetCamera.localPosition = Vector3.zero;
                        else
                            targetCamera.localPosition = Vector3.up * height / 2;
                    }
                }
                //设置相机旋转
                float xRotate = -Input.GetAxis("Mouse Y") * rotatingSpeed;
                float yRotate = Input.GetAxis("Mouse X") * rotatingSpeed;
                if (Input.GetMouseButton(1))
                {
                    if (!isOverAngle(xRotate))
                    {
                        pitchAngleRecorder += xRotate;
                        targetCamera.Rotate(Vector3.right * xRotate);
                    }
                    cameraBody.transform.Rotate(Vector3.up * yRotate);
                }
            }
        }
        /// <summary>
        /// 判断记录俯仰角是否越界
        /// </summary>
        /// <param name="delta">当前角度变化量</param>
        /// <returns>是否越界</returns>
        bool isOverAngle(float delta)
        {
            bool biggerThanMax = delta > 0 && pitchAngleRecorder >= maxPitchAngle;
            bool lessThanMin = delta < 0 && pitchAngleRecorder <= -maxPitchAngle;
            return biggerThanMax || lessThanMin;
        }
    }
}
