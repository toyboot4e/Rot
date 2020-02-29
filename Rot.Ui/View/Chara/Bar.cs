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
	public class EntityBar : RenderableComponent {
		public enum BarLayer {
			Background = 0,
			Effect = 1,
			Current = 2,
			Frame = 3,
		}

		EntityBarStyle style;
		NineSliceSpriteRenderer[] bars;
		NineSliceSpriteRenderer bar(BarLayer layer) => this.bars[(int) layer];
		protected PosUtil posUtil;

		// preferences (hard coded for now)
		Vector2 size;
		Vector2 offset;

		public EntityBar(PosUtil posUtil, EntityBarStyle style, Vector2 offset = default(Vector2)) {
			this.posUtil = posUtil;
			this.style = style;

			this.offset = offset;
			this.size = new Vector2(posUtil.tileWidth - 2, 5);
		}

		public override void OnAddedToEntity() {
			this.setupBars();
			this.zCtx(Layers.Stage, Depths.CharaGage);
		}

		void setupBars() {
			this.bars = new NineSliceSpriteRenderer[4] { null, null, null, null };

			foreach(var(layer, def) in this.style.defs) {
				var(ninepatch, color) = def;
				var barImg = new NineSliceSpriteRenderer(ninepatch);

				barImg.zCtx(Layers.Stage, Depths.CharaGage).setEntity(this.Entity);
				barImg.setSize(this.size).SetColor(color);
				barImg.SetLocalOffset(new Vector2(-this.size.X / 2, posUtil.tileHeight / 2));

				this.bars[(int) layer] = barImg;
			}
		}

		#region impl RenderableComponent
		public override RectangleF Bounds => this.bars[0].Bounds;
		// public override RectangleF Bounds => this.bars?[0]?.Bounds ?? new RectangleF(0, 0, 0, 0);
		// public override RectangleF Bounds => new RectangleF(0, 0, 5000, 5000);
		public override void Render(Batcher batcher, Camera camera) {
			foreach(var bar in this.bars) {
				bar?.Render(batcher, camera);
			}
		}
		public override void OnEntityTransformChanged(Transform.Component comp) {
			foreach(var bar in this.bars) {
				bar?.OnEntityTransformChanged(comp);
			}
		}
		#endregion

		public ITween<float> barAnimTween(float preRatio, float newRatio) {
			float currentAnimDuration = 0.2f;
			float effectDelay = 0.1f;
			float effectDuration = 0.5f;
			var easeType = EaseType.Linear;

			var preWidth = this.size.X * preRatio;
			var newWidth = this.size.X * newRatio;

			var effectColor = newRatio - preRatio < 0 ?
				Colors.Gage.damage :
				Colors.Gage.recover;

			var curBar = this.bar(BarLayer.Current);
			curBar.Width = preWidth;
			// curBar.tweenWidth(newWidth, e : easeType, d : currentAnimDuration)
			curBar.Tween("Width", newWidth, currentAnimDuration)
				.SetEaseType(easeType)
				.Start();

			var effectBar = this.bar(BarLayer.Effect);
			effectBar.Width = preWidth;
			effectBar.SetColor(effectColor);
			// effectBar.tweenWidth(newWidth, e : easeType, d : effectDuration)
			return effectBar.Tween("Width", newWidth, effectDuration)
				.SetDelay(effectDelay)
				.SetEaseType(easeType)
				.SetCompletionHandler(_ => {
					this.bar(BarLayer.Effect).SetColor(Colors.Gage.opaque);
				});
		}
	}
}