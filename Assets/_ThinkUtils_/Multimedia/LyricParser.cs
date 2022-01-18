//用于解析并实时显示歌词的脚本

//Version:0.2

//By NeilianAryan

//2020_09_10 - 公开歌词列表，微调歌词内容时机和脚本结构
//2020_08_21

using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace ThinkUtils
{
    public class LyricParser : Singleton<LyricParser>
    {
        /// <summary>
        /// 存储每句歌词的列表
        /// </summary>
        /// <typeparam name="Lyric">歌词类，包括时间和内容</typeparam>
        /// <returns></returns>
        public List<Lyric> lyrics = new List<Lyric>();
        /// <summary>
        /// 播放歌曲使用的AudioSource
        /// </summary>
        AudioSource musicPlayer;
        /// <summary>
        /// 当前歌曲进度对应的歌词
        /// </summary>
        public string currentLyric;

        /// <summary>
        /// 播放歌词
        /// </summary>
        /// <param name="musicPlayer">传入播放歌曲的AudioSource以供监测歌曲进度</param>
        /// <param name="lyricString">传入歌词字符串</param>
        public void Play(AudioSource musicPlayer, string lyricString)
        {
            currentLyric = "";
            this.musicPlayer = null;
            if (lyricString != null)
            {
                SplitLyric(lyricString);
                this.musicPlayer = musicPlayer;
            }
        }
        /// <summary>
        /// 结束播放，清空歌词
        /// </summary>
        public void Stop()
        {
            musicPlayer = null;
            currentLyric = "";
        }

        void Update()
        {
            //如果AudioSource不为空，则令歌词实时匹配歌曲进度
            if (musicPlayer != null)
                MatchLyric();
        }
        /// <summary>
        /// 解析歌词并存储
        /// </summary>
        /// <param name="lyricString">歌词字符串</param>
        void SplitLyric(string lyricString)
        {
            lyrics.Clear();
            string regularExpression = @"\[(?<MIMUTE>\d{2}):(?<SECOND>\d{2}.\d{2})\](?<LYRIC>.*)(?:\s|$)";
            MatchCollection matches = Regex.Matches(lyricString, regularExpression);
            foreach (Match m in matches)
            {
                float time = float.Parse(m.Groups["MIMUTE"].Value) * 60 + float.Parse(m.Groups["SECOND"].Value);
                string lyric = m.Groups["LYRIC"].Value;
                lyrics.Add(new Lyric(time, lyric));
            }
        }
        /// <summary>
        /// 根据歌曲进度实时匹配歌词的方法
        /// </summary>
        void MatchLyric()
        {
            currentLyric = "";
            //按时间从大到小倒序遍历歌词列表，如果当句歌词时间小于歌曲进度，则显示该句歌词
            for (int i = lyrics.Count - 1; i >= 0; i--)
            {
                if (lyrics[i].LyricTime <= musicPlayer.time)
                {
                    currentLyric = lyrics[i].LyricString;
                    break;
                }
            }
        }
        /// <summary>
        /// 歌词存储类
        /// </summary>
        public class Lyric
        {
            float time;
            string lyric;
            public Lyric(float time, string lyric)
            {
                this.time = time;
                this.lyric = lyric;
            }
            public float LyricTime { get { return this.time; } }
            public string LyricString { get { return this.lyric; } }
            public override string ToString()
            {
                return "Time:" + time + "\nLyric:" + lyric;
            }
        }
    }
}
