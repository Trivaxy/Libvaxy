using Microsoft.CSharp;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.GameInput;
using Terraria.UI;

namespace Libvaxy.Debug.REPL
{
	public class ReplUI : UIState
	{
		private static UIPanel panel;
		private static UIText script;
		private static bool focused;

		public override void OnInitialize()
		{
			panel = new UIPanel();
			panel.Width.Set(300f, 0f);
			panel.Height.Set(30f, 0f);
			panel.Left.Set(50f, 0f);
			panel.Top.Set(300f, 0f);
			panel.OnClick += Click;
			Append(panel);

			script = new UIText("Main.NewText(\"Hello, World!\");");
			script.Top.Set(-5f, 0f);
			panel.Append(script);
		}

		private void Click(UIMouseEvent evt, UIElement listeningElement) => Focus();

		public override void Draw(SpriteBatch spriteBatch)
		{
			DrawChildren(spriteBatch);

			if (focused)
			{
				PlayerInput.WritingText = true;
				Main.instance.HandleIME();
				script.SetText(Main.GetInputText(script.Text));
			}

			if (Main.oldKeyState.IsKeyDown(Keys.Enter) && Main.keyState.IsKeyUp(Keys.Enter))
				try
				{
					CompileScript();

				}
				catch (Exception e)
				{
					LibvaxyMod.Logger.Error(e);
				}
		}

		public void CompileScript()
		{
			string script = @"
public class LibvaxyREPLTest
{
    public static void Execute()
    {
        [CODE]
    }
}
"
			.Replace("[CODE]", ReplUI.script.Text);

			CSharpCodeProvider compiler = new CSharpCodeProvider(new Dictionary<string, string>() { { "CompilerVersion", "v7.3" } });
			CompilerResults results = compiler.CompileAssemblyFromSource(new CompilerParameters(new[] { "tModLoader.exe" }));
		}

		public override void Update(GameTime gameTime)
		{
			if (!panel.ContainsPoint(Main.MouseScreen) && Main.mouseLeft)
				Unfocus();
		}

		public void Focus()
		{
			Main.clrInput();
			focused = true;
			Main.blockInput = true;
		}

		public void Unfocus()
		{
			focused = false;
			Main.blockInput = false;
		}
	}
}