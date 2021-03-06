﻿using System;
using Rot.Engine.Fov;

namespace Rot.Engine {
    // TODO: refactor RlStage
    /// <summary> Decopules any stage instance from the engine </summary>
    public interface iRlStage {
        Rect2Di bounds { get; }
        bool isBlocked(Vec2i pos);
        bool isBlocked(int x, int y);
    }

    public static class RlStageExt {
        public static bool contains(this iRlStage self, Vec2i pos) =>
            self.bounds.contains(pos);
    }
}