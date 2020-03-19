using System.Collections.Generic;
using ImGuiNET;
using Nez.ImGuiTools.TypeInspectors;

namespace NezEp.Debug {
    public class ObjInspectorSpawn : Spawn {
        object obj;
        List<AbstractTypeInspector> inspectors;
        override protected string baseId() => "obj";

        ObjInspectorSpawn(object obj) {
            this.obj = obj;
            this.inspectors = TypeInspectorUtils.GetInspectableProperties(obj);
        }

        #region Global access
        public static ObjInspectorSpawn spawn(object obj) {
            return new ObjInspectorSpawn(obj).spawnSelf();
        }
        #endregion

        override protected void imGuiDrawImpl() {
            for (var i = this.inspectors.Count - 1; i >= 0; i--) {
                if (this.inspectors[i].IsTargetDestroyed) {
                    this.inspectors.RemoveAt(i);
                    continue;
                }
                this.inspectors[i].Draw();
            }
        }
    }

    // public class ObjInspectorImpl<T> { }

    // FIXME: can be crash? needs <T>?
    class ObjInspector<T> : AbstractTypeInspector {
        object obj;
        List<AbstractTypeInspector> inspectors;

        public override void Initialize() {
            this.obj = GetValue<T>();
            this.inspectors = TypeInspectorUtils.GetInspectableProperties(this.obj);
        }

        public override void DrawMutable() {
            var obj = GetValue<T>();
            if (obj == null) {
                ImGui.Text("<null>");
                return;
            }
            var isOpen = ImGui.CollapsingHeader($"{obj}", ImGuiTreeNodeFlags.DefaultOpen);
            if (isOpen) {
                for (var i = this.inspectors.Count - 1; i >= 0; i--) {
                    if (this.inspectors[i].IsTargetDestroyed) {
                        this.inspectors.RemoveAt(i);
                        continue;
                    }
                    this.inspectors[i].Draw();
                }
            }
            ImGui.TreePop();
        }
    }
}