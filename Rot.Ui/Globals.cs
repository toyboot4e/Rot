using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Nez;
using Nez.Tweens;
using Karcero.Engine; // DungeonGenerator
using KCell = Karcero.Engine.Models.Cell;
using KMap = Karcero.Engine.Models.Map<Karcero.Engine.Models.Cell>;
using KTerrain = Karcero.Engine.Models.TerrainType;
using KConf = Karcero.Engine.Models.DungeonConfiguration;

namespace Rot.Ui {
    // TODO: remove it or make it data-driven
    /// <summary> Temporary class to detect hard-coded values </summary>
    public static class ViewPreferences {
        // FoV animations
        public static float fovUpdateDuration => 12 / 60f;
        public static int fovRadius => 6;

        // input
        public static float vAxisRepeatFirst => 0.1f;
        public static float vAxisRepeatMulti => 0.1f;

        // body animation configuration
        public static EaseType walkEase => EaseType.Linear;

        // body animation durations
        public static float walkDuration => isQuick ? quickWalkDuration : _walkDuration;
        public static float walkAnimFps => isQuick ? quickWalkFps : _walkAnimFps; // ?
        public static float turnDirDuration => isQuick ? _turnDirDuration : quickTurnDirDuration;
        public static float swingDuration = isQuick ? _swingDuration : quickSwingDirDuration;

        // FIXME: the hack to enable quick mode
        static bool isQuick => Input.IsKeyDown(Keys.LeftControl) || Input.IsKeyDown(Keys.RightControl);
        static float _walkDuration => 8f / 60f;
        static float _walkAnimFps => 60f / 16f;
        static float _turnDirDuration => 0.02f;
        static float _swingDuration = 4f / 60f;

        static float quickWalkDuration = 4f / 60f;
        static float quickWalkFps => 60f / 32f;
        static float quickTurnDirDuration => 0.01f;
        static float quickSwingDirDuration => 2f / 60f;
    }

    public static class FixedSize {
        public static int nMaxLog => 100;
    }

    // TODO: remove the class and cache them
    public static class EntityNames {
        public static string player => "player";
        public static string gameLog => "game-log";
        public static string fovRenedrer => "fov-renderer";
        public static string debugDungeon => "debug-dungeon";
        public static string tiled => "tiled";
        public static string camera => "camera";
        public static string stair => "stair";
        public static string logView => "log-view";
        // public static string
    }

    /// <summary> The higher, the deeper </summary>
    public static class Layers {
        public static int Stage => 1000;
        public static int Screen => 0;
        public static int DebugScreen => -1000;

        /// <summary> Small value to make one layer prior to another </summary>
        public static float _inc => 0.0001f;
    }

    /// <summary> The higher, the deeper </summar>
    public static class Depths {
        public static float Stage => 0.6f;
        public static float Charachip => 0.5f;
        public static float Fov => 0.45f;
        public static float CharaGage => 0.4f;
        public static float DamageVal => 0.3f;
        public static float Menu => 0.25f;
        public static float Hud => 0.2f;
        public static float Talk => 0.15f;
        public static float TitleScreen => 0.12f;
        public static float Debug => 0.1f;

        /// <summary> Small value to make one sprite prior to another </summary>
        public static float _inc => 0.0001f;
    }

    /// <summary> Ease for prototyping </summary>
    public static class Colors {
        public static class Gage {
            public static Color frame => new Color(255, 255, 255);
            public static Color background => new Color(0, 0, 0);
            public static Color life => new Color(32, 32, 255);
            public static Color recover => new Color(204, 255, 255);
            public static Color damage => new Color(255, 0, 0);
            public static Color opaque => new Color(0, 0, 0, 0);
        }

        public static class Message {
            public static Color default_ => new Color(255, 255, 255);
            public static Color endDelta => new Color(32, 196, 128);
            public static Color keyword => new Color(192, 192, 32);
            public static Color chara => new Color(32, 196, 128);
            public static Color item => new Color(32, 178, 196);
            public static Color mana => new Color(178, 32, 170);

        }

        public static class Ui {

        }

        public static Color available => new Color(32, 178, 170);
        public static Color unavaliable => new Color(255, 100, 100);
    }

    public static class DirKeys {
        public static string left => "left";
        public static string right => "right";
        public static string up => "up";
        public static string down => "down";
        public static string leftUp => "leftUp";
        public static string leftDown => "leftDown";
        public static string rightUp => "rigthUp";
        public static string rightDown => "rigthDown";
    }

    public static class TiledMapSettings {
        public static class Main {
            public static Dictionary<KTerrain, uint[]> terrainMap = new Dictionary<KTerrain, uint[]> { //
                { KTerrain.Door, new uint[] { 2, 0, 0 } },
                { KTerrain.Floor, new uint[] { 2, 0, 0 } },
                { KTerrain.Rock, new uint[] { 2, 18, 0 } },
            };
        }
    }
}