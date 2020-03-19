using ImGuiNET;
using Nez.ImGuiTools.TypeInspectors;
using NezEp.Ui;

namespace NezEp.Debug {
    /// <summary> Wrapper of <c>EpCanvasInspector.inspectTree</c> that can be <c>spawn</c> </summary>
    public class EpCanvasInspectorSpawn : Spawn {
        EpCanvas canvas;
        override protected string baseId() => "ep node";

        EpCanvasInspectorSpawn(EpCanvas canvas) {
            this.canvas = canvas;
        }

        #region Global access
        public static EpCanvasInspectorSpawn spawn(EpCanvas canvas) {
            return new EpCanvasInspectorSpawn(canvas).spawnSelf();
        }
        #endregion

        override protected void imGuiDrawImpl() {
            EpCanvasInspector.inspectTree(this.canvas.root);
        }
    }

    /// <summary> A custom inspector that can be seen in <c>EntityPane</c> </summary>
    class EpCanvasInspector : AbstractTypeInspector {
        public override void DrawMutable() {
            var canvas = GetValue<EpCanvas>();
            if (canvas == null) {
                ImGui.Text("<null EpCanvas>");
            } else {
                EpCanvasInspector.inspectTree(canvas.root);
            }
        }

        // TODO: consider using more recursive solution
        // FIXME: it has errors
        public static void inspectTree(ContainerNode node) {
            if (inspectNode(node)) {
                foreach(var child in node.children) {
                    if (child is ContainerNode parent) {
                        EpCanvasInspector.inspectTree(parent);
                    } else {
                        inspectNode(child);
                    }
                    handleClick(node);
                    ImGui.TreePop();
                }
            }
            ImGui.TreePop();
        }

        static void handleClick<T>(T node) {
            // we are looking for a double-click that is not on the arrow
            if (ImGui.IsMouseDoubleClicked(0) && ImGui.IsItemClicked() &&
                (ImGui.GetMousePos().X - ImGui.GetItemRectMin().X) > ImGui.GetTreeNodeToLabelSpacing()) {
                ObjInspectorSpawn.spawn(node).focus();
            }
        }

        static bool inspectNode(iNode node) {
            bool isOpen;
            if (node is ContainerNode parent) {
                isOpen = ImGui.TreeNodeEx($"{node}", ImGuiTreeNodeFlags.OpenOnArrow | ImGuiTreeNodeFlags.DefaultOpen);
            } else {
                isOpen = ImGui.TreeNodeEx($"{node}", ImGuiTreeNodeFlags.Leaf);
            }
            handleClick(node);
            return isOpen;
        }
    }
}