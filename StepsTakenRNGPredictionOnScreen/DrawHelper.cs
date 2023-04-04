using Microsoft.Xna.Framework.Graphics;
using StardewValley.Menus;
using StardewValley;
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace StepsTakenOnScreen
{
    internal static class DrawHelper
    {
        
        public static Vector2 DrawHoverBox(SpriteBatch spriteBatch, string label, in Vector2 position, float wrapWidth)
        {
            Vector2 labelSize = spriteBatch.DrawTextBlock(Game1.smallFont, label, position + new Vector2(20f), wrapWidth);
            IClickableMenu.drawTextureBox(spriteBatch, Game1.menuTexture, new Rectangle(0, 256, 60, 60), (int)position.X, (int)position.Y, (int)labelSize.X + 27 + 20, (int)labelSize.Y + 27 + 10, Color.White);
            spriteBatch.DrawTextBlock(Game1.smallFont, label, position + new Vector2(20f), wrapWidth);
            return labelSize + new Vector2(27f);
        }
        
        public static Vector2 DrawTextBlock(this SpriteBatch batch, SpriteFont font, string text, Vector2 position, float wrapWidth, Color? color = null, bool bold = false, float scale = 1f)
        {
            return batch.DrawTextBlock(font, new IFormattedText[]
            {
                new FormattedText(text, color, bold)
            }, position, wrapWidth, scale);
        }
        
        public static Vector2 DrawTextBlock(this SpriteBatch batch, SpriteFont font, IEnumerable<IFormattedText> text, Vector2 position, float wrapWidth, float scale = 1f)
        {
            Vector2 blockSize = new(0f, 0f);
            if (text == null) return blockSize;
            foreach (IFormattedText snippet in text)
            {
                if (snippet?.Text == null) continue;

                string[] lines = snippet.Text.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string line in lines)
                {
                    Vector2 size = font.MeasureString(line);
                    if (size.X > blockSize.X)
                    {
                        blockSize.X = size.X;
                    }
                    blockSize.Y += size.Y;

                    if (snippet.Bold)
                    {
                        Utility.drawBoldText(batch, line, font, position, snippet.Color ?? Color.Black, scale);
                    }
                    else
                    {
                        batch.DrawString(font, line, position, snippet.Color ?? Color.Black, 0f, Vector2.Zero, scale, SpriteEffects.None, 1f);
                    }

                    position.Y += size.Y;
                }
            }
            return blockSize;
        }
    }
}
