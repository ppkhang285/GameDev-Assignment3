using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameBase.Enums;
using NaughtyAttributes;

namespace GameBase.AudioPlayer
{
    [CreateAssetMenu(menuName = "GameBase/Sound Map Config", fileName = "Sound Map Config")]
    public class SoundMapConfig : ScriptableObject
    {
        [ReorderableList]
        public List<SoundMapPath> MapPath;

        [System.Serializable]
        public class SoundMapPath
        {
            public SoundType Type;
            public string Path;
        }
    }
}
