/*
 * Description:
 * All specific events ingame declare here
 * 
 * Usage: 
 * include this line of code in any scripts
 * -- using GameBase.Events;
 */
using System;
using GameBase.Enums;

namespace GameBase.Events
{
    public class GameEvents
    {
        public static Action<SceneType, System.Action> LOAD_SCENE;
        public static Action<SceneType, System.Action> LOAD_SCENE_ASYNC;
        public static Action<SceneType, System.Action> UNLOAD_SCENE;

        public static Action<bool> ON_LOADING;
    }
}
