//----------------------------------------------
// C# 简化全局事件监控
// @author: ChenXing
// @email:  onechenxing@163.com
// @date:   2015/09/02
//----------------------------------------------
using UnityEngine;
using UnityEditor;
using System.Collections;

namespace CxLib.EventBox
{
    /// <summary>
    /// EventBox监控窗口
    /// </summary>
    public class EventBoxProfilerEditor : EditorWindow
    {
        /// <summary>
        /// 滚动位置
        /// </summary>
        Vector2 scrollPosition;

        /// <summary>
        /// 打开面板
        /// </summary>
        [MenuItem("CxTools/EventBoxProfiler")]
        public static void Open()
        {
            var window = EditorWindow.GetWindow(typeof(EventBoxProfilerEditor), false, "EventBoxProfiler");
            window.autoRepaintOnSceneChange = true;
        }

        /// <summary>
        /// 主绘制入口函数
        /// </summary>
        void OnGUI()
        {
            scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.Width(position.width), GUILayout.Height(position.height));
            GUILayout.Label(EventBox.Print());
            GUILayout.EndScrollView();
        }
    }
}
