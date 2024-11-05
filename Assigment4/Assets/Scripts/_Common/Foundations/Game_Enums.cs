/*
 * Description:
 * All enums ingame declare here
 * 
 * Usage: 
 * include this line of code in any scripts
 * -- using GameBase.Enums;
 */

namespace GameBase.Enums
{
    // DO NOT DELETE THIS ENUM
    // USE FOR CHANGE SCENE (SCENE CONFIGS + SCENE MANAGER)
    public enum SceneType
    {
        UNKNOWN = -999,
        MAINMENU = 0,
        LOBBY = 1,
        LEVEL = 100,
    }

    // DO NOT DELETE THIS ENUM
    // USE FOR TRANSITION (GAME SCENE TRANSITION)
    public enum TransitionType
    {
        NONE,
        UP,
        DOWN,
        LEFT,
        RIGHT,
        IN,
        OUT,
        FADE,
    }

    // DO NOT DELETE THIS ENUM
    // USE FOR SOUND (GAME SCENE TRANSITION)
    public enum SoundType
    {
        NONE = -1,
        COMMON = 0,
        MUSIC,
        SOUND_EFFECT,
    }
}
