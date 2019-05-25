using System.Linq;
using Nez;
using Rot.Engine;
using Rot.Ui;

namespace Rot.Ui {
	/// <summary> Created to decide the action of an entity </summary>
	public class PlControl : Control {
		EntityController ctrl;
		RlGameContext ctx;
		/// <summary> Filters cardinal directional input while it's on. </summary>
		KeyMode diaMode;
		/// <summary> Changes direction instead of walking while it's on. </summary>
		KeyMode dirMode;
		bool isDone;

		public PlControl(RlGameContext ctx) {
			this.ctx = ctx;
			this.diaMode = new KeyMode(VKey.Dia);
			this.dirMode = new KeyMode(VKey.Dir);
		}

		public void setController(EntityController ctrl) {
			this.ctrl = ctrl;
		}

		/// <summary> Decide action of the given entity and return to the `TickControl` </summary>
		void conclude(Action action) {
			this.ctrl.decide(action);
		}

		public override ControlResult update() {
			var input = base.ctx.input;
			this.updateModes(input);
			this.handleInput(input);

			if (this.ctrl.isDecided) {
				this.ctrl = null;
				base.ctx.cradle.pop();
				return ControlResult.Continue;
			} else {
				return ControlResult.SeeYouNextFrame;
			}
		}

		void updateModes(VInput input) {
			this.diaMode.update(input);

			var result = this.dirMode.update(input);
			if (result == KeyMode.Switch.TurnOn) {
				// var entity =
				// var target = this.findOnlyNighbor(this.context.controllerd);
				// .faceIn(target)
			}
		}

		/// <summary> Mayve dispatches a sub routine to the input </summary>
		void handleInput(VInput input) {
			// pressed VKey or down axis input
			var top = input.consumeTopPressedIgnoring(VKey.Dia, VKey.Dir, VKey.AxisKey);
			if (top.isKey) {
				this.handleVKey(top.asKey, input);
			} else if (input.isDirDown) {
				// FIXME: なぜ壁に頭をぶつけ続けることが無いのか……？
				input.vDir.clearBuf();
				this.handleDir(input.dirDown);
			}
		}

		// TODO: rest
		void handleDir(EDir dir) {
			if (this.diaMode.isOn && dir.isCardinal) {
				return; // filtered
			}

			if (dirMode.isOn) {
				this.conclude(new Engine.Act.Face(this.ctrl.actor, dir));
			} else {
				// TODO: don't consume turn if failed
				this.conclude(new Engine.Act.Walk(this.ctrl.actor, dir));
			}
		}

		void handleVKey(VKey key, VInput input) {
			switch (key) {
				case VKey.Select:
					// Return if take turn or not
					// this.conclude(CF.interact(plEntity));
					break;

				case VKey.Cancel:
					// this.cradle.push(new MenuControl(this.god, new InvMenu(this.god, this.context.controlled.get<Inventory>())));
					break;

				case VKey.Ground:
					// this.conclude(CF.searchGround(plEntity));
					break;

				case VKey.RestATurn:
					break;

				default:
					break;
			}
		}

		// /// <summary> Returns the only interactable entity or null </summary>
		// Entity findOnlyNighbor(Entity entity) {
		// 	var body = entity.get<Body>();
		// 	var pos = body.pos;
		// 	var stage = this.ctx.stage;
		// 	try {
		// 		return pos.neighbors
		// 			.Select(v => stage.tokenAt(v)) // Vec2 -> Entity
		// 			.SingleOrDefault(e => e != null && e.hasAnyOf<Interactable, Item, Stats>());
		// 	} catch {
		// 		return null;
		// 	}
		// 	return null;
		// }
	}

	/// <summary> Data of on/off and a binded key. </summary>
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