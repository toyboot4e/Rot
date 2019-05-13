using Nez;
using Rot.Engine;
using Rot.Ui;

namespace Rot.Ui {
	/// <summary> Created to decide the action of an entity. Deleted after finishing it. </summary>
	public class PlControl : Control {
		EntityControlContext entityCtx;
		/// <summary> Filters cardinal directional input while it's on. </summary>
		KeyMode diaMode;
		/// <summary> Changes direction instead of walking while it's on. </summary>
		KeyMode dirMode;
		bool isDone;

		public PlControl(ControlContext cc, EntityControlContext ec) : base(cc) {
			this.entityCtx = ec;
			this.diaMode = new KeyMode(VKey.Dia);
			this.dirMode = new KeyMode(VKey.Dir);
		}

		/// <summary> Decide action of the given entity and return to the `TickControl` </summary>
		void conclude(Action action) {
			this.entityCtx.decide(action);
			this.isDone = true;
		}

		public override ControlResult update() {
			var input = base.ctx.input;
			this.updateModes(input);
			this.handleInput(input);
			if (this.isDone) {
				base.ctx.cradle.popAndRemove();
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

		// TODO: rest
		void handleDir(EDir dir) {
			if (this.diaMode.isOn && dir.isCardinal) {
				return; // filtered
			}

			if (dirMode.isOn) {
				this.conclude(new Engine.Act.Face(this.entityCtx.actor, dir));
			} else {
				// TODO: don't consume turn if failed
				this.conclude(new Engine.Act.Walk(this.entityCtx.actor, dir));
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

	/// <summary> Data of on/off and a binded key. </summary>
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
}