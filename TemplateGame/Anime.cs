﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;
using System.Collections.Generic;

namespace OneButton
{
    class Anime
    {
        Size size = new Size();
        List<Vector2> pos = new List<Vector2>();
        List<int> hp = new List<int>();
        List<float> ppx = new List<float>();
        List<float> ppy = new List<float>();
        System.Random rnd = new System.Random();

        const int SIZE_PLAYER = 64;
        const int RUDIOS = 32;
        const int NEW_COUNT_10 = 10;
        const int NEW_COUNT_30 = 30;

        const int TIME_LIGHT = 40;
        const int MAX_LIGHT = 4;
        const int MIN_LIGHT = 1;
        const float SPEED_LIGHT = 0.4f;

        Texture2D player,enemy,maru;
        
        int enemy_x, enemy_y;

        enum State { drop, fly, stop, accele,dead}
        enum tex { dropPre,drop,flyPre,fly,stop,dead}
        int x,y;
        int count;
        int Pnum = 1;

        bool dead;
        public bool Dead { get { return dead; } }
        public void Ini()
        {
            enemy_x = 0; enemy_y = 0;
            x = 0;y = 0;
            count = NEW_COUNT_10;
            Pnum = MIN_LIGHT;
            dead = false;
            pos.Clear();
            hp.Clear();
            ppx.Clear();
            ppy.Clear();
        }

        public void Load(ContentManager content)
        {
            player = content.Load<Texture2D>("pp");
            maru = content.Load<Texture2D>("maru");
        }
        public void Pre(int state,int statePre)
        {
            switch (statePre)
            {
                case (int)State.drop://落下から浮遊
                    if(state == (int)State.fly)
                    {
                        y = (int)tex.fly;
                        x = 0;
                    }
                    else if(state == (int)State.stop)
                    {
                        y = (int)tex.stop;
                        x = 0;
                    }
                    break;
                case (int)State.fly://浮遊から落下
                    if (state == (int)State.drop)
                    {
                        y = (int)tex.dropPre;
                        x = 0;
                    }
                    else if (state == (int)State.stop)
                    {
                        y = (int)tex.stop;
                        x = 0;
                    }
                    break;
                case (int)State.stop://静止から浮遊
                    if(state == (int)State.fly)
                    {
                        y = (int)tex.flyPre;
                        x = 0;
                    }
                    break;
            }
            
            count--;
            if(count <= 0)
            {
                x++;
                if (x >= player.Width / SIZE_PLAYER)
                {
                    x = 0;
                    if (y == (int)tex.dropPre) y = (int)tex.drop;
                    if (y == (int)tex.flyPre) y = (int)tex.fly;
                    if (y == (int)tex.dead) dead = true;
                }
                if (y == (int)tex.drop || y == (int)tex.fly || y == (int)tex.dead) count = NEW_COUNT_30;
                else count = NEW_COUNT_10;
            }
            
        }
        public void DD()
        {
            y = (int)tex.dead;
            x = 0;
        }
        //public void Enemy()
        //{
        //    count[(int)name.enemy]--;
        //    if(count[(int)name.enemy] <= 0)
        //    {
        //        enemy_x++;
        //        //if(enemy_x >= enemy.Width/)enemy_x = 0;
        //        count[(int)name.enemy] = NEW_COUNT_30;
        //    }
        //}
        public void Lights(Vector2 playerPos,int sc,bool accel)//パーティクル練習
        {
            if (accel) Pnum = MAX_LIGHT;
            else Pnum = MIN_LIGHT;
            for (int i = 0; i < Pnum; i++)
            {
                pos.Add(new Vector2(rnd.Next((int)playerPos.X-RUDIOS,(int)(playerPos.X-RUDIOS)+SIZE_PLAYER-maru.Width), rnd.Next((int)playerPos.Y - 48,(int)playerPos.Y - RUDIOS+(SIZE_PLAYER - SIZE_PLAYER/4))));
                hp.Add(TIME_LIGHT);
                ppx.Add(0);
                ppy.Add(0);
            }
            for (int i = 0; i < hp.Count; i++)
            {
                hp[i]--;
                ppy[i] += SPEED_LIGHT;
                if(hp[i] <= TIME_LIGHT - 10)
                {
                    if (pos[i].X <= playerPos.X - RUDIOS + SIZE_PLAYER/4) ppx[i] -= SPEED_LIGHT;
                    else if(pos[i].X >= playerPos.X - RUDIOS +SIZE_PLAYER - (SIZE_PLAYER/4)) ppx[i] += SPEED_LIGHT;
                }
                if (hp[i] <= 0)
                {
                    hp.Remove(hp[i]);
                    pos.Remove(pos[i]);
                    ppx.Remove(ppx[i]);
                    ppy.Remove(ppy[i]);
                }
            }
        }
        ////できたらやる
        //public void LLL()//accele時の画面演出
        //{

        //}
        public void Draw(SpriteBatch sb,Vector2 pos,int sc,int state)
        {
            if (!dead)
            {
                for (int i = 0; i < this.pos.Count; i++) sb.Draw(maru, new Vector2(this.pos[i].X + ppx[i], this.pos[i].Y - sc - ppy[i]), Color.LightGreen);//パーティクル
                sb.Draw(player, new Vector2(pos.X - RUDIOS, pos.Y - RUDIOS - sc), new Rectangle(SIZE_PLAYER * x, SIZE_PLAYER * y, SIZE_PLAYER, SIZE_PLAYER), Color.White);//動作確認
            }
        }
    }
}