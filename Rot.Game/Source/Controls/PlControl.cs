using Nez;
using Rot.Engine;
using Rot.Ui;

namespace Rot.Ui {
	/// <summary> Data of on/off and binded key. Calls logic. </summary>
	public class KeyMode {
		VKey key;
		public bool isOn { get; private set; }
		public bool isOff => !this.isOn;

		public enum Switch {
			TurnOn,
			TurnOff,
			None,
		}

		public KeyMode(VKey key) {
			this.key = key;
		}

		public Switch update(VInput input) {
			var isDown = input.isDown(this.key);

			if (isDown && this.isOff) {
				return Switch.TurnOn;
			} else if (!isDown && this.isOn) {
				return Switch.TurnOff;
			} else {
				return Switch.None;
			}
		}
	}

	/// <summary> Created to decide action of an entity. Deleted after doing it. </summary>
	public class PlControl : Control {
		EntityControlContext context;
		KeyMode diaMode; // Filters cardinal directional input while it's on.
		KeyMode dirMode; // Changes direction instead of walking while it's on.

		public PlControl(EntityControlContext context) {
			this.context = context;
			this.diaMode = new KeyMode(VKey.Dia);
			this.dirMode = new KeyMode(VKey.Dir);
		}

		void decide(Cradle cradle, IAction action) {
			this.context.decide(action);
			cradle.removeTop(); // return to the `TickControl`
		}

		public override ControlResult update(ControlContext context) {
			var input = context.input;
			this.updateModes(input);
			this.handleInput(input);
			return ControlResult.SeeYouNextFrame;
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

		// TODO: rest
		/// <summary> Dispatches a sub routine to the input </summary>
		void handleInput(VInput input) {
			var top = input.consumeTopPressedIgnoring(VKey.Dia, VKey.Dir);

			switch (top.kind) {
				case VKeyResult.Kind.Dir:
					this.handleDir(top.dir);
					break;

				case VKeyResult.Kind.Key:
					this.handleDir(top.dir);
					break;

				case VKeyResult.Kind.None:
					break;
			}
		}

		void handleDir(EDir dir) {
			if (this.diaMode.isOn && dir.isCardinal) {
				return; // filtered
			}

			if (dirMode.isOn) {
				// .faceIn(dir); // just change direction
			} else {
				// this.conclude(CF.walk(plEntity, dir));
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

		Entity findOnlyNighbor(Entity entity) {
			var body = entity.get<Body>();
			var pos = body.pos;
			// var stage = body.stage;
			// try {
			//     return pos.neighbors
			//         .Select(v => stage.tokenAt(v)) // Vec2 -> Entity
			//         .SingleOrDefault(e => e != null && e.hasAnyOf<Interactable, Item, Stats>());
			// } catch {
			//     return null;
			// }
			return null;
		}
	}

	// public class CommandFactory {
	//     public CommandFactory() { }

	//     // Attack, interact or swing.
	//     public Engine.IAction interact(Entity actor) {
	//         var dir = actor.body().facing;
	//         return interactIn(actor, dir);
	//     }

	//     public Engine.IAction interactIn(Entity actor, Engine.EDir dir) {
	//         var action = this.tryInvoke(actor, dir, (i) => i.onInteract(actor)) ??
	//             AF.swing(actor, dir);
	//         return action;
	//     }
	//     public Engine.IAction walk(Entity actor, Engine.EDir dir) {

	//         var body = actor.get<Body>();
	//         if (body.canWalkIn(dir)) {
	//             return AF.walk(actor, dir);
	//         } else {
	//             var ctrl = new ActorCtrl(actor);
	//             ctrl.faceIn(dir);
	//             return this.tryInvoke(actor, dir, (i) => i.onCollision(actor));
	//         }
	//     }

	//     // May attack or call an event function: onInteract() or onCollision().
	//     Engine.IAction tryInvoke(Entity actor, Engine.EDir dir, System.Func<Interactable, IAction> f) {
	//         var nextPos = actor.body().pos + dir.vec;
	//         var interactable = stage.tokenAt(nextPos) ?
	//             .get<Interactable>() ?? null;
	//         if (interactable != null) {
	//             return f.Invoke(interactable);
	//         }
	//         var battler = stage.battlerTokenAt(nextPos);
	//         if (battler != null) {
	//             return AF.meleeAttack(actor, dir);
	//         }
	//         return null;
	//     }

	//     public Engine.IAction searchGround(Entity actor) {
	//         return new Actions.SearchGround(actor, this.game);
	//     }
	// }
}