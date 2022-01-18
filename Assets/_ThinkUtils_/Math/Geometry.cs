//用于处理与几何算法相关的脚本

//Version:0.2

//By NeilianAryan

//2021_03_01 - 增加计算两点连线中点的方法
//2021_01_24

using System.Collections.Generic;
using UnityEngine;

namespace ThinkUtils
{
    public class Geometry : MonoBehaviour
    {
        /// <summary>
        /// 计算两点连线中点位置的方法
        /// </summary>
        /// <param name="dot1">点1</param>
        /// <param name="dot2">点2</param>
        /// <returns>中点位置</returns>
        public static Vector3 GetCenterPoint(Vector3 dot1, Vector3 dot2)
        {
            return new Vector3((dot1.x + dot2.x) / 2, (dot1.y + dot2.y) / 2, (dot1.z + dot2.z) / 2);
        }
        /// <summary>
        /// 计算折射向量的方法
        /// </summary>
        /// <param name="I">入射光线</param>
        /// <param name="N">法线</param>
        /// <param name="r1">入射介质折射率</param>
        /// <param name="r2">折射介质折射率</param>
        /// <returns>折射向量</returns>
        public static Vector3 Refract(Vector3 I, Vector3 N, float r1, float r2)
        {
            I = I.normalized;
            N = N.normalized;
            float h = r1 / r2;
            float A = Vector3.Dot(I, N);
            float B = 1.0f - h * h * (1.0f - A * A);
            Vector3 T = h * I - (h * A + Mathf.Sqrt(B)) * N;
            if (B > 0)
                return T;
            else
                return Vector3.zero;
        }
        /// <summary>
        /// 利用3个控制点和LineRenderer组件绘制贝塞尔曲线的方法
        /// </summary>
        /// <param name="line">绘制曲线的LineRenderer</param>
        /// <param name="vertexCount">曲线的顶点总数</param>
        /// <param name="p1">控制点1</param>
        /// <param name="p2">控制点2</param>
        /// <param name="p3">控制点3</param>
        public static void DrawBezierCurve(LineRenderer line, int vertexCount, Transform p1, Transform p2, Transform p3)
        {
            List<Vector3> pointList = new List<Vector3>();
            for (float ratio = 0; ratio <= 1; ratio += 1.0f / vertexCount)
            {
                Vector3 tangentLineVertex1 = Vector3.Lerp(p1.position, p2.position, ratio);
                Vector3 tangentLineVectex2 = Vector3.Lerp(p2.position, p3.position, ratio);
                Vector3 bezierPoint = Vector3.Lerp(tangentLineVertex1, tangentLineVectex2, ratio);
                pointList.Add(bezierPoint);
            }
            line.positionCount = pointList.Count;
            line.SetPositions(pointList.ToArray());
        }
    }
}
