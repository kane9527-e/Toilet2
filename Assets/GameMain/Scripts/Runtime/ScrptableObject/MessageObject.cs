using GameMain.Scripts.Runtime.Message;
using UnityEngine;

namespace GameMain.Scripts.UI.GamePlay.MessageUIForm
{
    //[CreateAssetMenu(menuName = "Message")]
    public class MessageObject : ScriptableObject
    {
        /// <summary>
        /// 推送消息
        /// </summary>
        /// <param name="info"></param>
        public virtual void PushMsg(string info)
        {
            info = info.Replace(@"\n", "\n");
            MessageSystem.PushMessage(info);
        }
    }
}