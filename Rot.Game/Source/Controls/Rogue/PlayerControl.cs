using System.Linq;
using Nez;
using Rot.Engine;
using RlEv = Rot.RlEv;
using Rot.Ui;

namespace Rot.Game {
    /// <summary> Basic controller for UI to inject an action to an entity </summary>
    public class EntityController {
        public readonly Entity actor;
        public RlEvent action { get; private set; }

        public EntityController(Entity entity) {
            Insist.IsNotNull(entity);
            (this.actor, this.action) = (entity, null);
        }

        public void setAction(RlEvent action) {
            this.action = action;
        }

        public void resetAction() {
            this.action = null;
        }

        public bool isDecided => this.action != null;
    }

    /// <summary> Maps input → RlEvent </summary>
    public static class PlayerCommands {
        /// <summary> Attack, interact or just swing </summary>
        public static RlEvent onSelectKeyPressed(Entity actor, RlGameContext ctx) {
            var body = actor.get<Body>();
            var dir = body.facing;
            var es = ctx.entitiesAt(body.pos + body.facing.vec).ToList(); // avoid null entity
            if (es.Count == 0) {
                // no entity found
                return new RlEv.JustSwing(actor, dir);
            }
            // trying to find an attackable or interactable entity
            for (int i = 0; i < es.Count; i++) {
                var e = es[i];
                if (e.has<Interactable>()) {
                    return new RlEv.Interact(actor, dir);
                } else if (e.has<Performance>()) {
                    return new RlEv.MeleeAttack(actor, dir);
                }
            }
            return new RlEv.JustSwing(actor, dir);
        }
    }

    /// <summary> Determines a player action </summary>
    public class PlayerControl : Control {
        EntityController controller;
        RlGameContext gameCtx;

        /// <summary> Filters cardinal directional input while it's on. </summary>
        KeyMode diaMode;
        /// <summary> Forces just to change direction, not to walk, while it's on. </summary>
        KeyMode dirMode;

        public PlayerControl(RlGameContext gameCtx) {
            this.gameCtx = gameCtx;
            this.diaMode = new KeyMode(VKey.Dia);
            this.dirMode = new KeyMode(VKey.Dir);
        }

        /// <summary> To be called before entering it </summary>
        public void setController(EntityController ctrl) {
            this.controller = ctrl;
        }

        public override ControlResult update() {
            var input = base.ctrlCtx.input;

            var ev = this.updateModes(input) ?? this.handleInput(input);
            if (ev == null) {
                return ControlResult.SeeYouNextFrame;
            } else {
                this.controller.setAction(ev);
                this.controller = null;
                base.ctrlCtx.cradle.pop();
                return ControlResult.Continue;
            }
        }

        /// <summary> Updates input mode and maybe creates directional modification for the player controllerd </summary>
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
            var top = input.consumeTopPressedIgnoring(VKey.Dia, VKey.Dir, VKey.AxisKey);
            if (top.isKey) {
                return this.handleVKey(top.asKey, input);
            } else if (input.isDirDown) {
                // XXX: HACK to avoid walking to block at the same frame again
                input.vDir.clearBuf();
                return this.handleDir(input.dirDown);
            } else {
                return null;
            }
        }

        /// <summary> Maps directional input to events depending on mode </summary>
        RlEvent handleDir(EDir dir) {
            if (this.diaMode.isOn && dir.isCardinal) {
                return null; // filtered
            }

            return dirMode.isOn ?
                (RlEvent) new RlEv.Face(this.controller.actor, dir) :
                (RlEvent) new RlEv.Walk(this.controller.actor, dir);
        }

        /// <summary> Maps key input to events </summary>
        RlEvent handleVKey(VKey key, VInput input) {
            var entity = this.controller.actor;
            var dir = entity.get<Body>().facing;

            switch (key) {
                case VKey.Select:
                    return PlayerCommands.onSelectKeyPressed(entity, this.gameCtx);

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

            return null;
        }

        /// <summary> Returns the only adjacent, interactive entity or null </summary>
        Entity findOnlyNeighbor(Entity entity) {
            var body = entity.get<Body>();
            var pos = body.pos;

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
            bool isDown = input.isKeyDown(this.key);

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