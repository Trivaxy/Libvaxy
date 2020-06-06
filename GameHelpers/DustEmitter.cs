using Terraria;
using Microsoft.Xna.Framework;
using System;
using Microsoft.Xna.Framework.Graphics;

namespace Libvaxy.GameHelpers
{
	// TODO: this is very usable, but just finalize a bit
	public class DustEmitter
	{
		public Vector2 Position;
		public float SpeedX;
		public float SpeedY;
		public int Width;
		public int Height;
		public Vector2 ParentOffset;
		public readonly Entity Parent;
		public int TimeLeft;
		public int Index;
		public bool Active;
		public bool SyncParentPosition;
		public bool SyncParentSpeed;
		public Action<DustEmitter> UpdateAction;

		public DustEmitter(Vector2 position, float speedX, float speedY, int width, int height, Vector2 parentOffset, Entity parent, int timeLeft, bool active, bool syncParentSpeed, bool syncParentPosition, Action<DustEmitter> updateAction)
		{
			Position = position;
			SpeedX = speedX;
			SpeedY = speedY;
			Width = width;
			Height = height;
			ParentOffset = parentOffset;
			Parent = parent;
			TimeLeft = timeLeft;
			Active = active;
			SyncParentPosition = syncParentPosition;
			SyncParentSpeed = syncParentSpeed;
			UpdateAction = updateAction;
		}

		// TODO: sync multiplayer dust emitters
		public void Update()
		{
			if (!Active || Main.dedServ)
				return;

			if (Parent != null)
			{
				if (!Parent.active || (Parent is Player && (Parent as Player).dead))
					Kill();

				if (SyncParentPosition)
					Position = Parent.position + ParentOffset;

				if (SyncParentSpeed)
				{
					SpeedX = Parent.velocity.X;
					SpeedY = Parent.velocity.Y;
				}
			}
			else
				Position += new Vector2(SpeedX, SpeedY);

			UpdateAction?.Invoke(this);

			if (--TimeLeft == 0)
				Kill();
		}

		internal void DebugDrawRect(SpriteBatch spriteBatch)
		{
			Utils.DrawRectangle(spriteBatch, Position, Position + new Vector2(Width, Height), Color.White, Color.White, 1f);
		}

		public void Kill() => Libvaxy.DustEmitters.RemoveAt(Index);

		public static int SpawnDustEmitter(Vector2 position, float speedX, float speedY, int width, int height, int timeLeft, Action<DustEmitter> updateAction = null)
		{
			DustEmitter emitter = new DustEmitter(position, speedX, speedY, width, height, Vector2.Zero, null, timeLeft, true, false, false, updateAction);
			emitter.Index = Libvaxy.DustEmitters.Count;
			Libvaxy.DustEmitters.Add(emitter);
			return emitter.Index;
		}
		 
		public static int SpawnParentedDustEmitter(Entity parent, int width, int height, Vector2 parentOffset, int timeLeft, bool syncParentPosition = true, bool syncParentSpeed = true, Action<DustEmitter> updateAction = null)
		{
			DustEmitter emitter = new DustEmitter(parent.position, 0, 0, width, height, parentOffset, parent, timeLeft, true, syncParentPosition, syncParentSpeed, updateAction);
			emitter.Index = Libvaxy.DustEmitters.Count;
			Libvaxy.DustEmitters.Add(emitter);
			return emitter.Index;
		}
	}
}
