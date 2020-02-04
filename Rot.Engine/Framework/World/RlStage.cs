using System;
using Rot.Engine.Fov;

namespace Rot.Engine {
    // TODO: refactor RlStage
    /// <summary> Decopules any stage instance from the engine </summary>
    public interface iRlStage {
        Rect bounds { get; }
        RlTiles tilesAt(Vec2 pos);
        bool isBlocked(Vec2 pos);
        bool isBlocked(int x, int y);
    }

    /// <summary> Helper <summary>
    public interface RlTiles {
        Vec2 pos { get; }
        iRlStage stage { get; }
        bool arePassable();
    }

    public static class RlStageExt {
        public static bool contains(this iRlStage self, Vec2 pos) =>
            self.bounds.contains(pos);
    }

    public static class RlTilesExt {
        public static bool isInStage(this RlTiles self) =>
            self.stage.contains(self.pos);

        public static bool areBlocking(this RlTiles self) =>
            !self.arePassable();
    }
}