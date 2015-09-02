# EventBox
C# 简化全局事件发送/接收类

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
                       private void OnEventTestHandler(string eventType, object eventData)
                       {
                          Debug.Log("type:" + eventType + ",data:" + eventData);
                       }
    4.移除事件：       EventBox.Remove(MyEventBoxType.EVENT_TEST, OnEventTestHandler);
    5.移除对象上的所有事件监听：   EventBox.RemoveAll(this);
    6.清除系统内所有事件监听：     EventBox.Dispose();
