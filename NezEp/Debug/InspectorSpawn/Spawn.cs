using System.Collections.Generic;
using ImGuiNET;
using Nez;
using Nez.ImGuiTools;
using Nez.ImGuiTools.TypeInspectors;
using NezEp.Ui;
using Num = System.Numerics;

namespace NezEp.Debug {
    public static class SpawnExt {
        public static T spawnSelf<T>(this T self) where T : Spawn {
            AnyInspectorSpawn.spawn(self);
            return self;
        }
    }

    // TODO: consider making AnyObjInspector generic and use it as a concrete implementation
    /// <summary>
    /// Template of an inspector. See <c>EpCanvasInspectorSpawn</c> as an example.
    /// </summary>
    /// <remark> Based on <c>EntityInspector</c> </remark>
    public abstract class Spawn {
        abstract protected string baseId();
        abstract protected void imGuiDrawImpl();
        protected virtual void onBegin() {
            ImGui.SetNextWindowSize(new Num.Vector2(335, 400), ImGuiCond.FirstUseEver);
            ImGui.SetNextWindowSizeConstraints(new Num.Vector2(335, 200), new Num.Vector2(Screen.Width, Screen.Height));
        }
        protected virtual bool isDisposed() => false;

        bool shouldFocus;
        public void focus() {
            this.shouldFocus = true;
        }

        string imGuiId;
        protected Spawn() {
            this.imGuiId = baseId() + " #" + NezImGui.GetScopeId().ToString();
        }

        public void imGuiDraw() {
            if (this.isDisposed()) {
                AnyInspectorSpawn.rm(this);
                return;
            }

            if (this.shouldFocus) {
                this.shouldFocus = false;
                ImGui.SetNextWindowFocus();
                ImGui.SetNextWindowCollapsed(false);
            }

            bool isOpen = true;
            if (ImGui.Begin(imGuiId, ref isOpen)) {
                this.onBegin();
                this.imGuiDrawImpl();
                ImGui.End();
            }

            if (!isOpen) {
                AnyInspectorSpawn.rm(this);
            }
        }
    }

    public class AnyInspectorSpawn : Component {
        public List<Spawn> inspectors = new List<Spawn>();

        #region Global access
        static string entityName = "ImGuiWindowSpawn";
        public static AnyInspectorSpawn instance() {
            return Core.Scene.Entities.FindEntity(entityName)?.GetComponent<AnyInspectorSpawn>() ??
                Core.Scene.CreateEntity(entityName).AddComponent(new AnyInspectorSpawn());
        }
        public static void spawn(Spawn spawn) {
            AnyInspectorSpawn.instance().inspectors.Add(spawn);
        }
        public static void rm(Spawn spawn) {
            AnyInspectorSpawn.instance().inspectors.Remove(spawn);
        }
        #endregion

        #region Component
        static string imGuiId = "ImGUiWindowSpawn";
        public override void OnAddedToEntity() {
            Core.GetGlobalManager<ImGuiManager>()?.RegisterDrawCommand(imGuiDraw);
        }
        public override void OnRemovedFromEntity() {
            Core.GetGlobalManager<ImGuiManager>()?.UnregisterDrawCommand(imGuiDraw);
        }
        #endregion

        public void imGuiDraw() {
            for (int i = 0; i < this.inspectors.Count; i++) {
                this.inspectors[i].imGuiDraw();
            }
        }
    }
}