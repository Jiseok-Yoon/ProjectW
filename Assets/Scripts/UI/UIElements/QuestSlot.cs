using ProjectW.Util;
using UnityEngine;

namespace ProjectW.UI
{
    public class QuestSlot : MonoBehaviour, IPoolableObject
    {
        public bool CanRecycle { get; set; } = true;
    }
}
