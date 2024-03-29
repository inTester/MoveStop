﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OneButton
{
    class Size
    {
        //クラスで共通のサイズプロパティ
        const int CELL = 64;
        const int WIDTH = CELL * 9;
        const int HEIGHT = CELL * 15;
        const int WIN_WIDTH = WIDTH + CELL;//32はプレイ画面外のUI分
        //***
        const int UNDER = HEIGHT - CELL * 8;
         
        const int WORLD = 12000;
        //***
        const int END_COORD = WORLD - CELL * 8;

        public int World { get { return WORLD; } }
        public int EndSc
        { get { return END_COORD; } }
        public int Win_Width { get { return WIN_WIDTH; } }
        public int Width { get { return WIDTH; } }
        public int Height { get { return HEIGHT; } }
        public int Under { get { return UNDER; } }
    }
}
