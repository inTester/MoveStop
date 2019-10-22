﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using System.Diagnostics;

namespace OneButton
{
    class Map
    {
        //針山
        Texture2D prickle;
        const int PR_SPEED = 2;
        bool[] prDrawF;
        Vector2 prSize = new Vector2(120, 32);//針の部分のサイズ
        readonly Vector2[] prPosBase = { new Vector2(0, 400), new Vector2(400, 600) };
        Vector2[] prPos;
        public Vector2[] PrPos => prPos;
        public Vector2 PrSize => prSize;

        //床
        Texture2D floor;
        bool[] floorDrawF;
        const int F_SPEED = 2;
        Vector2 fSize = new Vector2(196,32);
        readonly Vector2[] floorPosBase = { new Vector2(300, 800), new Vector2(200, 2200) };
        Vector2[] floorPos; //書き込み用
        float[] movePos;
        public float[] MovePos => movePos;
        public Vector2[] FloorPos => floorPos;
        public Vector2 Fsize => fSize;

        //床の向き
        enum Dir
        {
            NONE = 0,
            RIGHT = 1,
            LEFT = -1,
        }
        readonly Dir[] floorDirBase = { Dir.LEFT , Dir.RIGHT };
        int[] fd; //書き込み用
        readonly Dir[] prDirBase = { Dir.LEFT, Dir.RIGHT };
        int[] prd; //書き込み用


        public Map()
        {
            prDrawF = new bool[prPosBase.Length];
            for (int i = 0; i < prPosBase.Length; i++)
            {
                prDrawF[i] = false;
            }
            prPos = prPosBase;
            prd = new int[prDirBase.Length];
            for (int i = 0; i < prDirBase.Length; i++)
            {
                prd[i] = (int)prDirBase[i]; //intに変換
                prDrawF[i] = false;
            }

            //床の情報を書き込み用に入れる
            floorPos = floorPosBase;
            fd = new int[floorDirBase.Length];
            floorDrawF = new bool[floorPosBase.Length];
            for (int i = 0; i < floorDirBase.Length; i++)
            {
                fd[i] = (int)floorDirBase[i]; //intに変換
                floorDrawF[i] = false;
            }
            movePos = new float[floorPosBase.Length];

        }

        public void Load(ContentManager content)
        {
            prickle = content.Load<Texture2D>("Prickle");
            floor = content.Load<Texture2D>("Floor");
        }

        public void FlagChange(Vector2 pPos)
        {
            for (int i = 0; i < floorPos.Length; i++)
            {
                if (64 * 15 > floorPos[i].Y - pPos.Y && -(64 * 15) < floorPos[i].Y - pPos.Y)
                    floorDrawF[i] = true;
                else
                    floorDrawF[i] = false;
            }
            for (int i = 0; i < prPosBase.Length; i++)
            {
                if (64 * 15 > PrPos[i].Y - pPos.Y && -(64 * 15) < PrPos[i].Y - pPos.Y)
                    prDrawF[i] = true;
                else
                    prDrawF[i] = false;
            }
        }
        public void FloorMove(int wid)
        {
            for (int i = 0; i < floorPos.Length; i++)
            {
                if (!floorDrawF[i]) continue;
                movePos[i] = fd[i] * F_SPEED;
                floorPos[i].X += movePos[i];
                if (floorPos[i].X+fSize.X > wid || floorPos[i].X < 0) fd[i] = -fd[i]; //反転
            }
            for (int i = 0; i < prPosBase.Length; i++)
            {
                if (!prDrawF[i]) continue;
                prPos[i].X += prd[i] * PR_SPEED;
                if (prPos[i].X + PrSize.X > wid || prPos[i].X < 0) prd[i] = -prd[i]; //反転
            }
        }

        public void Draw(SpriteBatch sb, int sc)
        {
            for (int i = 0; i < floorPos.Length; i++)
            {
                Debug.WriteLine("F"+i+":"+floorDrawF[i]);
                if (!floorDrawF[i]) continue;
                sb.Draw(floor, new Vector2(floorPos[i].X, floorPos[i].Y - sc), Color.White);
            }
            for (int i = 0; i < prPosBase.Length; i++)
            {
                Debug.WriteLine("P" + i + ":" + prDrawF[i]);

                if (!prDrawF[i]) continue;
                sb.Draw(prickle, new Vector2(prPosBase[i].X, prPosBase[i].Y - sc), Color.White);
            }

        }
    }
}