﻿using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace OneButton
{
    class PositionBar
    {
        Size size = new Size();

        //バー上のplayer
        Texture2D playertex;
        Texture2D enemyBarTex;
        Vector2 enemyBarPos;
        Vector2 playerbarpos;
        public Vector2 PlayerPos { get { return playerbarpos; } }

        //バー
        Texture2D bartex;
        Vector2 barpos;
        public Vector2 BarPos { get { return barpos; } }

        public PositionBar() { Init(); }

        public void Init()
        {
            playerbarpos = new Vector2(size.Width+16 , 0);
            barpos = new Vector2(size.Width + 16, 0);
            enemyBarPos = new Vector2(size.Width + 16, 0);
        }

        public void Load(ContentManager content)
        {
            playertex = content.Load<Texture2D>("pos_player");
            enemyBarTex = content.Load<Texture2D>("pos_enemy");
        }

        public void Update(Vector2 playerPos, Vector2 enemyPos, int enemySize)
        {
            Calcu(playerPos);
            Calcu(enemyPos, enemySize);
        }

        public void Calcu(Vector2 playerpos)
        {
            playerbarpos.Y = (size.Height - 32) * playerpos.Y / size.World;
            if (playerpos.Y <= 0) playerbarpos.Y = 0;
            if (playerpos.Y >= size.World) playerbarpos.Y = size.Height-34;
        }
        public void Calcu(Vector2 enemyPos, int enemySize)
        {
            enemyBarPos.Y = (size.Height-32) * (enemyPos.Y + enemySize) / size.World;
            if (enemyPos.Y+enemySize <= 0) enemyBarPos.Y = 0;
            if (enemyPos.Y >= size.World) enemyBarPos.Y = size.Height-34;
        }
        public void Draw(SpriteBatch sb)
        {
            sb.Draw(playertex, playerbarpos, Color.White);
            sb.Draw(enemyBarTex, enemyBarPos, Color.White);
        }
    }
}
