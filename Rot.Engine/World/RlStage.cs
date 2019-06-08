using System;
using System.Collections.Generic;
using System.Linq;
using Nez;

namespace Rot.Engine {
    /// <summary> Decopules any stage instance from the engine </summary>
    public interface RlStage {
        Rect bounds { get; }
        RlTiles tilesAt(Vec2 pos);
        // RlCell at(Vec2 pos);
    }

    /// <summary> Helper <summary>
    public interface RlTiles {
        Vec2 pos { get; }
        RlStage stage { get; }
        bool arePassable();
    }

    public static class RlStageExt {
        public static bool contains(this RlStage self, Vec2 pos) =>
            self.bounds.contains(pos);
    }

    public static class RlTilesExt {
        public static bool isInsideStage(this RlTiles self) =>
            self.stage.contains(self.pos);

        public static bool areBlocking(this RlTiles self) =>
            !self.arePassable();
    }
}