using System.Linq;
using Nez;
using Rot.Engine;
using RlEv = Rot.Engine.RlEv;
using Rot.Ui;

namespace Rot.Ui {
    /// <summary> Determines a player action </summary>
    public class PlayerControl : Control {
        EntityController controller;
        RlGameContext gameCtx;

        /// <summary> Filters cardinal directional input while it's on. </summary>
        KeyMode diaMode;
        /// <summary> Changes direction instead of walking while it's on. </summary>
        KeyMode dirMode;

        public PlayerControl(RlGameContext gameCtx) {
            this.gameCtx = gameCtx;
            this.diaMode = new KeyMode(VKey.Dia);
            this.dirMode = new KeyMode(VKey.Dir);
        }

        public void setController(EntityController ctrl) {
            this.controller = ctrl;
        }

        public override ControlResult update() {
            var input = base.ctrlCtx.input;

            var ev = this.updateModes(input) ?? this.handleInput(input);
            if (ev != null) {
                this.controller.setAction(ev);
                this.controller = null;
                base.ctrlCtx.cradle.pop();
                return ControlResult.Continue;
            }

            return ControlResult.SeeYouNextFrame;
        }

        RlEvent updateModes(VInput input) {
            this.diaMode.update(input);

            var result = this.dirMode.update(input);
            if (result == KeyMode.Switch.TurnOn &&
                this.findOnlyNeighbor(this.controller.actor) is Entity neighbor) {
                var entity = this.controller.actor;
                var dir = this.gameCtx.logic.dirTo(entity, neighbor);
                return new RlEv.Face(entity, dir);
            }

            return null;
        }

        /// <summary> Maybe dispatches a sub routine to the input </summary>
        RlEvent handleInput(VInput input) {
            // pressed VKey or down axis input
            var top = input.consumeTopPressedIgnoring(VKey.Dia, VKey.Dir, VKey.AxisKey);
            if (top.isKey) {
                return this.handleVKey(top.asKey, input);
            } else if (input.isDirDown) {
                // FIXME: 壁に頭をぶつけつづける？
                input.vDir.clearBuf();
                return this.handleDir(input.dirDown);
            } else {
                return null;
            }
        }

        // TODO: rest
        RlEvent handleDir(EDir dir) {
            if (this.diaMode.isOn && dir.isCardinal) {
                return null;
            }

            return dirMode.isOn ?
                (RlEvent) new RlEv.Face(this.controller.actor, dir) :
                (RlEvent) new RlEv.Walk(this.controller.actor, dir);
        }

        RlEvent handleVKey(VKey key, VInput input) {
            var e = this.controller.actor;
            var dir = e.get<Body>().facing;

            RlEvent ev = null;
            switch (key) {
                case VKey.Select:
                    ev = new RlEv.MeleeAttack(e);
                    break;

                case VKey.Cancel:
                    // this.cradle.push(new MenuControl(this.god, new InvMenu(this.god, this.context.controlled.get<Inventory>())));
                    break;

                case VKey.Ground:
                    break;

                case VKey.RestATurn:
                    break;

                default:
                    break;
            }

            return ev;
        }

        /// <summary> Returns the only interactable entity or null </summary>
        Entity findOnlyNeighbor(Entity entity) {
            var body = entity.get<Body>();
            var pos = body.pos;
            var stage = this.gameCtx.stage;
            var logic = this.gameCtx.logic;

            var es = pos.neighbors
                .Select(v => this.gameCtx.entitiesAt(v))
                // FIXME: detecting interactable/attackable entities
                .filterT(e => e.has<Body>());

            if (es.FirstOrDefault() is Entity e1) {
                return es.ElementAtOrDefault(1) == null ? e1 : null;
            } else {
                return null;
            }
        }
    }

    /// <summary> Observes switching of a <c>VKey</c>. </summary>
    public class KeyMode {
        VKey key;
        public bool isOn { get; private set; }
        public bool isOff => !this.isOn;

        public enum Switch {
            TurnOn,
            TurnOff,
            NoChange,
        }

        public KeyMode(VKey key) {
            this.key = key;
        }

        public Switch update(VInput input) {
            var isDown = input.isKeyDown(this.key);

            if (isDown && this.isOff) {
                this.isOn = true;
                return Switch.TurnOn;
            } else if (!isDown && this.isOn) {
                this.isOn = false;
                return Switch.TurnOff;
            } else {
                return Switch.NoChange;
            }
        }
    }
}