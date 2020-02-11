using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Nez;
using Nez.Textures;
using Nez.Tweens;
using NezEp.Prelude;

namespace Rot.Ui {
	public class EntityBarStyle {
		public Dictionary < EntityBar.BarLayer, (NinePatchSprite, Color) > defs;
	}

	public class HpBar : EntityBar {
		public HpBar(PosUtil posUtil, EntityBarStyle style, Vector2 offset = default(Vector2)) : base(posUtil, style, offset) {

		}
	}

	// TODO: put bar in UI
	public class EntityBar : Component {
		public enum BarLayer {
			Background = 0,
			Effect = 1,
			Current = 2,
			Frame = 3,
		}

		EntityBarStyle style;
		Dictionary<BarLayer, NineSliceSpriteRenderer> bars = new Dictionary<BarLayer, NineSliceSpriteRenderer>();
		public IEnumerable<RenderableComponent> sprites => this.bars.Values;
		public readonly int renderLayer = Layers.Stage;
		// public float depthBase => ZOrders.CharaGage;
		float depth(BarLayer layer) => Depths.CharaGage - (int) layer * 0.001f;

		protected PosUtil posUtil;
		public Vector2 size { get; private set; }
		public Vector2 offset { get; private set; }

		public EntityBar(PosUtil posUtil, EntityBarStyle style, Vector2 offset = default(Vector2)) {
			this.posUtil = posUtil;
			this.offset = offset;
			this.size = new Vector2(posUtil.tileWidth - 2, 5);
			this.style = style;
		}

		public override void OnAddedToEntity() {
			var content = this.Entity.Scene.Content;
			foreach(var(layer, def) in this.style.defs) {
				var(ninepatch, color) = def;
				var img = new NineSliceSpriteRenderer(ninepatch);

				img.zCtx(layer: this.renderLayer, depth: this.depth(layer));
				img.SetColor(color);
				img.setSize(this.size);
				img.SetLocalOffset(new Vector2(-this.size.X / 2, posUtil.tileHeight / 2));

				this.Entity.AddComponent(img);
				this.bars.Add(layer, img);
			}
		}

		public void animate(float preRatio, float newRatio) {
			float currentAnimDuration = 0.2f;
			float effectDelay = 0.1f;
			float effectDuration = 0.5f;
			var easeType = EaseType.Linear;

			var preWidth = this.size.X * preRatio;
			var newWidth = this.size.X * newRatio;

			var effectColor = newRatio - preRatio < 0 ?
				Colors.Gage.damage :
				Colors.Gage.recover;

			var curBar = this.bars[BarLayer.Current];
			curBar.Width = preWidth;
			// curBar.tweenWidth(newWidth, e : easeType, d : currentAnimDuration)
			curBar.Tween("Width", newWidth, currentAnimDuration)
				.SetEaseType(easeType)
				.Start();

			var effectBar = this.bars[BarLayer.Effect];
			effectBar.Width = preWidth;
			effectBar.SetColor(effectColor);
			// effectBar.tweenWidth(newWidth, e : easeType, d : effectDuration)
			effectBar.Tween("Width", newWidth, effectDuration)
				.SetDelay(effectDelay)
				.SetEaseType(easeType)
				.SetCompletionHandler(_ => {
					this.bars[BarLayer.Effect].SetColor(Colors.Gage.opaque);
				})
				.Start();
		}
	}
}