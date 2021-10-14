using System.Collections.Generic;
using GameMain.Scripts.UI.GamePlay.MessageUIForm;
using UnityEngine;

namespace GameMain.Scripts.Runtime.ScrptableObject
{
    public class ItemMessageObject : MessageObject
    {
        [SerializeField] private List<string> addItemMessage; //添加物品消息
        [SerializeField] private List<string> reduceItemMessage; //减少物品消息
        [SerializeField] private List<string> useItemMessage; //使用物品消息

        public void RandomPushAddMsg()
        {
            PushMsg(addItemMessage[Random.Range(0, addItemMessage.Count)]);
        }

        public void RandomPushReduceMsg()
        {
            PushMsg(reduceItemMessage[Random.Range(0, reduceItemMessage.Count)]);
        }

        public void RandomPushUseMsg()
        {
            PushMsg(useItemMessage[Random.Range(0, useItemMessage.Count)]);
        }
    }
}