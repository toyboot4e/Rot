using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Nez;
using Nez.Textures;
using Nez.Tweens;
using Rot.Engine;

namespace Rot.Ui {
	public class EntityBarStyle {
		public Dictionary < EntityBar.BarLayer, (NinePatchSubtexture, Color) > defs;
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
		Dictionary<BarLayer, NineSliceSprite> bars = new Dictionary<BarLayer, NineSliceSprite>();
		public IEnumerable<RenderableComponent> sprites => this.bars.Values;
		public readonly int renderLayer = Layers.Stage;
		// public float depthBase => ZOrders.CharaGage;
		float depth(BarLayer layer) => ZOrders.CharaGage - (int) layer * 0.001f;

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
				var(subtextures, color) = def;
				var sprite = new NineSliceSprite(subtextures);

				sprite.layer(layer: this.renderLayer, depth: this.depth(layer));
				sprite.SetColor(color);
				sprite.setSize(this.size);
				sprite.SetLocalOffset(new Vector2(-this.size.X / 2, posUtil.tileHeight / 2));

				this.Entity.AddComponent(sprite);
				this.bars.Add(layer, sprite);
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
			curBar.Tween("width", newWidth, currentAnimDuration)
				.SetEaseType(easeType)
				.Start();

			var effectBar = this.bars[BarLayer.Effect];
			effectBar.Width = preWidth;
			effectBar.SetColor(effectColor);
			// effectBar.tweenWidth(newWidth, e : easeType, d : effectDuration)
			effectBar.Tween("width", newWidth, effectDuration)
				.SetDelay(effectDelay)
				.SetEaseType(easeType)
				.SetCompletionHandler(_ => {
					this.bars[BarLayer.Effect].SetColor(Colors.Gage.opaque);
				})
				.Start();
		}
	}
}