//用于发送Web请求及多媒体资源请求并返回结果的脚本

//Version:0.9

//By NeilianAryan

//2021_05_25 - 错误返回信息从error完善为详细信息
//2021_05_21 - 完善本地资源请求的链接前缀
//2021_03_10 - 完善错误回调触发时的协程终止功能
//2021_03_05 - 增加Post、Put接口请求对Json数据的支持
//2021_03_04 - 增加请求错误后的处理回调函数
//2020_12_17 - 增加同步请求本地文本方法
//2020_08_28 - 取消http请求返回代码判定，直接返回请求结果
//2020_08_19 - 取消心跳机制；增加多媒体资源支持
//2020_06_10

using System;
using System.IO;
using System.Text;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace ThinkUtils
{
    public class WebRequester : Singleton<WebRequester>
    {
        //——————————————以同步方式请求本地文本——————————————

        /// <summary>
        /// 同步请求文本信息
        /// </summary>
        /// <param name="path">文本路径</param>
        /// <param name="encoding">编码方式，若不提供则默认使用UTF8</param>
        /// <returns>请求结果</returns>
        public string GetText(string path, Encoding encoding = null)
        {
            if (!File.Exists(path)) Debug.LogError("请求的文件不存在");
            return File.ReadAllText(path, encoding != null ? encoding : Encoding.UTF8);
        }

        //——————————————从网络请求字符串结果——————————————

        /// <summary>
        /// 是否打印返回结果
        /// </summary>
        public bool showResult;
        /// <summary>
        /// 请求方法枚举
        /// </summary>
        enum RequestMethod
        {
            GET,
            POST,
            PUT,
            DELETE,
        }
        IEnumerator APIRequester(string api, RequestMethod method, Action<string> callback = null, Action<string> errorHandler = null, string token = null, WWWForm postData = null, string postDataJson = null)
        {
            //创建请求
            UnityWebRequest webRequest = new UnityWebRequest();
            //判断请求方法
            switch (method)
            {
                case RequestMethod.GET:
                    webRequest = UnityWebRequest.Get(api);
                    break;
                case RequestMethod.POST:
                    webRequest = postData != null ? UnityWebRequest.Post(api, postData) : new UnityWebRequest(api, "POST");
                    break;
                case RequestMethod.PUT:
                    webRequest = postData != null ? UnityWebRequest.Post(api, postData) : new UnityWebRequest(api, "PUT");
                    webRequest.method = "PUT";
                    break;
                case RequestMethod.DELETE:
                    webRequest = UnityWebRequest.Delete(api);
                    break;
            }
            //传递Json时设置ContentType并处理数据
            if (postDataJson != null)
            {
                webRequest.SetRequestHeader("Content-Type", "application/json;charset=utf-8");
                byte[] body = Encoding.UTF8.GetBytes(postDataJson);
                webRequest.uploadHandler = new UploadHandlerRaw(body);
                webRequest.downloadHandler = new DownloadHandlerBuffer();
            }
            //判断是否携带token认证
            if (token != null)
                webRequest.SetRequestHeader("Authorization", token);
            //发送请求
            yield return webRequest.SendWebRequest();
            //处理错误
            if (webRequest.isNetworkError || webRequest.isHttpError)
            {
                if (showResult)
                    Debug.LogError(webRequest.downloadHandler.text);
                errorHandler?.Invoke(webRequest.downloadHandler.text);
                yield break;
            }
            //获取返回结果
            string result = webRequest.downloadHandler.text;
            if (showResult)
                print(result);
            //回调函数
            callback?.Invoke(result);
        }
        /// <summary>
        /// 发送GET请求
        /// </summary>
        /// <param name="API">接口地址</param>
        /// <param name="callback">请求完成后的回调函数，string参数为接口返回内容</param>
        /// <param name="errorHandler">报错时的错误处理回调函数，string参数为报错信息</param>
        /// <param name="token">是否需要token认证，默认为null，即无需认证</param>
        /// <param name="filePrefix">是否携带file前缀，若是则会为地址添加 "file://" 前缀</param>
        public void GET(string API, Action<string> callback = null, Action<string> errorHandler = null, string token = null, bool filePrefix = false)
        {
            StartCoroutine(APIRequester((filePrefix ? "file://" : "") + API, RequestMethod.GET, callback, errorHandler, token));
        }
        /// <summary>
        /// 发送POST请求
        /// </summary>
        /// <param name="API">接口地址</param>
        /// <param name="postData">传给接口的form类型数据</param>
        /// <param name="callback">请求完成后的回调函数，string参数为接口返回内容</param>
        /// <param name="errorHandler">报错时的错误处理回调函数，string参数为报错信息</param>
        /// <param name="token">是否需要token认证，默认为null，即无需认证</param>
        public void POST(string API, WWWForm postData, Action<string> callback = null, Action<string> errorHandler = null, string token = null)
        {
            StartCoroutine(APIRequester(API, RequestMethod.POST, callback, errorHandler, token, postData));
        }
        /// <summary>
        /// 发送POST请求
        /// </summary>
        /// <param name="API">接口地址</param>
        /// <param name="postData">传给接口的Json类型数据</param>
        /// <param name="callback">请求完成后的回调函数，string参数为接口返回内容</param>
        /// <param name="errorHandler">报错时的错误处理回调函数，string参数为报错信息</param>
        /// <param name="token">是否需要token认证，默认为null，即无需认证</param>
        public void POST(string API, string postData, Action<string> callback = null, Action<string> errorHandler = null, string token = null)
        {
            StartCoroutine(APIRequester(API, RequestMethod.POST, callback, errorHandler, token, null, postData));
        }
        /// <summary>
        /// 发送PUT请求
        /// </summary>
        /// <param name="API">接口地址</param>
        /// <param name="putData">传给接口的form类型数据</param>
        /// <param name="callback">请求完成后的回调函数，string参数为接口返回内容</param>
        /// <param name="errorHandler">报错时的错误处理回调函数，string参数为报错信息</param>
        /// <param name="token">是否需要token认证，默认为null，即无需认证</param>
        public void PUT(string API, WWWForm putData, Action<string> callback = null, Action<string> errorHandler = null, string token = null)
        {
            StartCoroutine(APIRequester(API, RequestMethod.PUT, callback, errorHandler, token, putData));
        }
        /// <summary>
        /// 发送PUT请求
        /// </summary>
        /// <param name="API">接口地址</param>
        /// <param name="putData">传给接口的Json类型数据</param>
        /// <param name="callback">请求完成后的回调函数，string参数为接口返回内容</param>
        /// <param name="errorHandler">报错时的错误处理回调函数，string参数为报错信息</param>
        /// <param name="token">是否需要token认证，默认为null，即无需认证</param>
        public void PUT(string API, string putData, Action<string> callback = null, Action<string> errorHandler = null, string token = null)
        {
            StartCoroutine(APIRequester(API, RequestMethod.PUT, callback, errorHandler, token, null, putData));
        }
        /// <summary>
        /// 发送DELETE请求
        /// </summary>
        /// <param name="API">接口地址</param>
        /// <param name="callback">请求完成后的回调函数，string参数为接口返回内容</param>
        /// <param name="errorHandler">报错时的错误处理回调函数，string参数为报错信息</param>
        /// <param name="token">是否需要token认证，默认为null，即无需认证</param>
        public void DELETE(string API, Action<string> callback = null, Action<string> errorHandler = null, string token = null)
        {
            StartCoroutine(APIRequester(API, RequestMethod.DELETE, callback, errorHandler, token));
        }

        //——————————————从网络或本地请求音频/图片多媒体资源——————————————

        /// <summary>
        /// 请求数据类型枚举
        /// </summary>
        enum ResultType
        {
            AudioClip,
            Texture2D
        }
        Coroutine multimediaRequester;
        IEnumerator MultimediaRequester(ResultType resultType, string path, AudioType audioType, Action<AudioClip> audioCallback = null, Action<Texture2D> textureCallback = null, Action<string> errorHandler = null, string token = null)
        {
            //创建方法
            UnityWebRequest webRequest = new UnityWebRequest();
            //判断请求数据类型
            switch (resultType)
            {
                case ResultType.AudioClip:
                    webRequest = UnityWebRequestMultimedia.GetAudioClip(path, audioType);
                    break;
                case ResultType.Texture2D:
                    webRequest = UnityWebRequestTexture.GetTexture(path);
                    break;
            }
            //判断是否携带token认证
            if (token != null)
                webRequest.SetRequestHeader("Authorization", token);
            //发送请求
            yield return webRequest.SendWebRequest();
            //处理错误
            if (webRequest.isNetworkError || webRequest.isHttpError)
            {
                if (showResult)
                    Debug.LogError(webRequest.error);
                errorHandler?.Invoke(webRequest.error);
                StopCoroutine(multimediaRequester);
            }
            //根据请求数据类型执行不同回调，返回结果
            switch (resultType)
            {
                case ResultType.AudioClip:
                    audioCallback?.Invoke(DownloadHandlerAudioClip.GetContent(webRequest));
                    break;
                case ResultType.Texture2D:
                    textureCallback?.Invoke(DownloadHandlerTexture.GetContent(webRequest));
                    break;
            }
        }
        /// <summary>
        /// 请求AudioClip音频
        /// </summary>
        /// <param name="path">资源路径，可以为uri或本地路径</param>
        /// <param name="audioType">音频格式，注意：mp3仅在移动平台支持</param>
        /// <param name="callback">获取成功后的回调函数</param>
        /// <param name="errorHandler">报错时的错误处理回调函数，string参数为报错信息</param>
        /// <param name="token">token认证</param>
        public void GET_AudioClip(string path, AudioType audioType, Action<AudioClip> callback, Action<string> errorHandler = null, string token = null)
        {
            multimediaRequester = StartCoroutine(MultimediaRequester(ResultType.AudioClip, path, audioType, callback, null, errorHandler, token));
        }
        /// <summary>
        /// 请求Texture2D图片
        /// </summary>
        /// <param name="path">资源路径，可以为uri或本地路径</param>
        /// <param name="callback">获取成功后的回调函数</param>
        /// <param name="errorHandler">报错时的错误处理回调函数，string参数为报错信息</param>
        /// <param name="token">token认证</param>
        public void GET_Texture2D(string path, Action<Texture2D> callback, Action<string> errorHandler = null, string token = null)
        {
            multimediaRequester = StartCoroutine(MultimediaRequester(ResultType.Texture2D, path, AudioType.WAV, null, callback, errorHandler, token));
        }
    }
}