//----------------------------------------------
// C# 简化全局事件发送/接收类
// @author: ChenXing
// @email:  onechenxing@163.com
// @date:   2014/12/31
//----------------------------------------------

/*
    使用方式：
    1.定义事件类型（可以直接定义在发送者类，或者单独创建的事件类）
    (注意事件字符串名字中前缀加入类名，防止事件命名冲突，
     如果有携带data的注释中注明数据类型，便于接收者解析)
    public class MyEventBoxType
    {
        /// <summary>
        /// 测试事件
        /// data为int型数据
        /// </summary>
        public const string EVENT_TEST = "MyEventBoxType_EVENT_TEST";
    }
    2.发送事件：       EventBox.Send(MyEventBoxType.EVENT_TEST, 1);
    3.接收事件：       EventBox.Add(MyEventBoxType.EVENT_TEST, OnEventTestHandler);
                       private void OnEventTestHandler(object eventData)
                       {
                          Debug.Log("[EVENT_TEST] data:" + eventData);
                       }
    4.移除事件：       EventBox.Remove(MyEventBoxType.EVENT_TEST, OnEventTestHandler);
    5.移除对象上的所有事件监听：   EventBox.RemoveAll(this);
    6.清除系统内所有事件监听：     EventBox.Dispose();
*/

using System;
using System.Collections.Generic;

/// <summary>
/// 全局事件类
/// </summary>
public class EventBox
{
    /// <summary>
    /// 事件监听函数代理
    /// </summary>
    /// <param name="eventData">携带数据</param>
    public delegate void EventBoxHandler(object eventData);

    /// <summary>
    /// 事件监听记录表
    /// </summary>
    private static Dictionary<string, List<EventBoxHandler>> _listenerDic = new Dictionary<string, List<EventBoxHandler>>();
    
    /// <summary>
    /// 对象关联的事件监听记录表
    /// </summary>
    private static Dictionary<object, ObjectListenerMap> _objListenerDic = new Dictionary<object, ObjectListenerMap>();

    /// <summary>
    /// 发送事件
    /// </summary>
    /// <param name="type">事件类型</param>
    /// <param name="data">携带数据</param>
    public static void Send(string type, object data = null)
    {
        if (_listenerDic.ContainsKey(type))
        {
            var handlerList = _listenerDic[type];
            for (var i = 0; i < handlerList.Count; i++)
            {
                handlerList[i](data);
            }
        }
    }

    /// <summary>
    /// 添加事件监听
    /// </summary>
    /// <param name="type">事件类型</param>
    /// <param name="handler">事件监听回调函数</param>
    public static void Add(string type, EventBoxHandler handler)
    {
        //_listenerDic
        if (_listenerDic.ContainsKey(type) == false)
        {
            List<EventBoxHandler> handlerList = new List<EventBoxHandler> { handler };
            _listenerDic.Add(type, handlerList);
        }
        else
        {
            if (_listenerDic[type].Contains(handler) == false)
            {
                _listenerDic[type].Add(handler);
            }
        }

        //_objListenerDic
        if (handler.Target != null)
        {
            if (_objListenerDic.ContainsKey(handler.Target) == false)
            {
                ObjectListenerMap map = new ObjectListenerMap();
                map.Add(type, handler);
                _objListenerDic.Add(handler.Target, map);
            }
            else
            {
                _objListenerDic[handler.Target].Add(type, handler);
            }
        }
    }

    /// <summary>
    /// 移除事件监听
    /// </summary>
    /// <param name="type">事件类型</param>
    /// <param name="handler">事件监听回调函数</param>
    public static void Remove(string type, EventBoxHandler handler)
    {
        //_listenerDic
        if (_listenerDic.ContainsKey(type))
        {
            if (_listenerDic[type].Remove(handler))
            {
                if (_listenerDic[type].Count == 0)
                {
                    _listenerDic.Remove(type);
                }
            }
        }

        //_objListenerDic
        if (_objListenerDic.ContainsKey(handler.Target))
        {
            _objListenerDic[handler.Target].Remove(type, handler);
            if(_objListenerDic[handler.Target].Get().Count == 0)
            {
                _objListenerDic.Remove(handler.Target);
            }
        }
    }

    /// <summary>
    /// 移除某个对象上的所有事件监听
    /// </summary>
    /// <param name="listener">监听者对象</param>
    public static void RemoveAll(object listener)
    {
        if (_objListenerDic.ContainsKey(listener))
        {
            foreach (var item in _objListenerDic[listener].Get())
            {
                //_listenerDic
                if (_listenerDic.ContainsKey(item.Key))
                {
                    if (_listenerDic[item.Key].Remove(item.Value))
                    {
                        if (_listenerDic[item.Key].Count == 0)
                        {
                            _listenerDic.Remove(item.Key);
                        }
                    }
                }
            }
        }

        //_objListenerDic
        _objListenerDic.Remove(listener);
    }

    /// <summary>
    /// 释放系统内所有监听
    /// </summary>
    public static void Dispose()
    {
        _listenerDic.Clear();
        _objListenerDic.Clear();
    }

    /// <summary>
    /// 打印内部消息列表
    /// </summary>
    /// <returns></returns>
    public static string Print()
    {
        string txt = "【所有监听类型】\n";
        foreach(var item in _listenerDic)
        {
            txt += string.Format("-->类型:{0},监听数量:{1}\n", item.Key, item.Value.Count);
        }
        txt += "\n【所有监听对象】\n";
        foreach(var map in _objListenerDic)
        {
            txt += string.Format("-->对象:{0},监听数量:{1}\n", map.Key, map.Value.Get().Count);
        }
        return txt;
    }
}

/// <summary>
/// object事件引用关系内部map类
/// </summary>
class ObjectListenerMap
{
    /// <summary>
    /// 关联列表(每条记录代表在对象上的Add的一个监听)
    /// </summary>
    private List<KeyValuePair<string, EventBox.EventBoxHandler>> _mapList = new List<KeyValuePair<string, EventBox.EventBoxHandler>>();

    /// <summary>
    /// 添加一个监听记录
    /// </summary>
    /// <param name="type"></param>
    /// <param name="handler"></param>
    public void Add(string type, EventBox.EventBoxHandler handler)
    {
        KeyValuePair<string, EventBox.EventBoxHandler> item = new KeyValuePair<string, EventBox.EventBoxHandler>(type, handler);
        if(_mapList.Contains(item) == false)
        {
            _mapList.Add(item);
        }
    }

    /// <summary>
    /// 移除一个监听记录
    /// </summary>
    /// <param name="type"></param>
    /// <param name="handler"></param>
    public void Remove(string type, EventBox.EventBoxHandler handler)
    {
        KeyValuePair<string, EventBox.EventBoxHandler> item = new KeyValuePair<string, EventBox.EventBoxHandler>(type, handler);
        _mapList.Remove(item);
    }

    /// <summary>
    /// 获得所有监听列表
    /// </summary>
    /// <returns></returns>
    public List<KeyValuePair<string, EventBox.EventBoxHandler>> Get()
    {
        return _mapList;
    }
}
