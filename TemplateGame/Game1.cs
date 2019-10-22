﻿// このファイルで必要なライブラリのnamespaceを指定
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;

/// <summary>
/// プロジェクト名がnamespaceとなります
/// </summary>
namespace OneButton
{
    /// <summary>
    /// ゲームの基盤となるメインのクラス
    /// 親クラスはXNA.FrameworkのGameクラス
    /// </summary>
    public class Game1 : Game
    {
        // フィールド（このクラスの情報を記述）
        private GraphicsDeviceManager graphicsDeviceManager;//グラフィックスデバイスを管理するオブジェクト
        private SpriteBatch spriteBatch;//画像をスクリーン上に描画するためのオブジェクト

        Size size= new Size();
        Player player;
        Key key;
        Map map;
        Collition coll;
        PositionBar positionBar;
        Enemy enemy;

        UI_Button button;//※※
        Anime anime;//※※
        UI ui;//※※
        Scene_Count sceneCount;//※※

        enum Scene { title, tutlial, play, end ,retry}
        Scene scene;

        /// <summary>
        /// コンストラクタ
        /// （new で実体生成された際、一番最初に一回呼び出される）
        /// </summary>
        public Game1()
        {
            //グラフィックスデバイス管理者の実体生成
            graphicsDeviceManager = new GraphicsDeviceManager(this);
            graphicsDeviceManager.PreferredBackBufferHeight = size.Height;
            graphicsDeviceManager.PreferredBackBufferWidth = size.Win_Width;
            //コンテンツデータ（リソースデータ）のルートフォルダは"Contentに設定
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// 初期化処理（起動時、コンストラクタの後に1度だけ呼ばれる）
        /// </summary>
        protected override void Initialize()
        {
            map = new Map();
            key = new Key();
            player = new Player();
            coll = new Collition();
            positionBar = new PositionBar();
            enemy = new Enemy();

            button = new UI_Button();//※※
            anime = new Anime();//※※
            ui = new UI();//※※
            sceneCount = new Scene_Count();//※※

            Ini();
            base.Initialize();// 親クラスの初期化処理呼び出し。絶対に消すな！！
        }
        public void Ini()
        {
        }
        /// <summary>
        /// コンテンツデータ（リソースデータ）の読み込み処理
        /// （起動時、１度だけ呼ばれる）
        /// </summary>
        protected override void LoadContent()
        {
            // 画像を描画するために、スプライトバッチオブジェクトの実体生成
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // この下にロジックを記述
            anime.Load(Content);
            map.Load(Content);
            //***
            positionBar.Load(Content);
            enemy.Load(Content);

            button.Load(Content);//※※
            ui.Load(Content,GraphicsDevice);//※※
            
            // この上にロジックを記述
        }

        /// <summary>
        /// コンテンツの解放処理
        /// （コンテンツ管理者以外で読み込んだコンテンツデータを解放）
        /// </summary>
        protected override void UnloadContent()
        {
            // この下にロジックを記述


            // この上にロジックを記述
        }

        /// <summary>
        /// 更新処理
        /// （1/60秒の１フレーム分の更新内容を記述。音再生はここで行う）
        /// </summary>
        /// <param name="gameTime">現在のゲーム時間を提供するオブジェクト</param>
        protected override void Update(GameTime gameTime)
        {
            // ゲーム終了処理（ゲームパッドのBackボタンかキーボードのエスケープボタンが押されたら終了）
            if ((GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed) ||
                 (Keyboard.GetState().IsKeyDown(Keys.Escape)))
            {
                Exit();
            }

            // この下に更新ロジックを記述
            key.Push_Triger();
            switch (scene)
            {
                case Scene.title:
                    button.Button();//※※
                    if (ui.Scene_Change(key.IsPushKey)) scene = Scene.tutlial;
                    break;
                case Scene.tutlial:
                    button.Button();//※※
                    if (ui.Scene_Change(key.IsPushKey)) scene = Scene.play;
                    break;
                case Scene.play:
                    if(player.St != 4)//※※
                    {
                        ui.Scroll(player.SC);//※※
                        key.Update();
                        enemy.Update();
                        positionBar.Update(player.Pos);
                        map.FloorMove(size.Width);
                        map.FlagChange(player.Pos);
                        player.Update(key);

                        if (coll.FloorColl(player.Pos, player.R, map.FloorPos, map.Fsize) != -1 && player.DropF)
                        {
                            int i = coll.FloorColl(player.Pos, player.R, map.FloorPos, map.Fsize);
                            player.FloorMove(map.MovePos[i]);
                        }
                        if (coll.PrColl(player.Pos, player.R, map.PrPos, map.PrSize) || coll.EnemyColl(player.Pos, player.R, enemy.Pos, enemy.Size))
                        {
                            Debug.WriteLine("ゲームオーバー");
                            player.DeadFlag();//※※
                            anime.DD();//※※
                        }
                        else if (!(coll.FloorColl(player.Pos, player.R, map.FloorPos, map.Fsize) != -1 && player.DropF))
                            player.Drop();
                    }
                    anime.Lights(player.PosPre,player.SC,player.Ac);//※※
                    anime.Pre(player.St,player.StPre);//※※
                    if (sceneCount.Change(anime.Dead)) scene = Scene.retry;//※※
                    break;
                    //※※↓
                case Scene.retry:
                    key.Update();

                    player.Ini();
                    enemy.Init();
                    anime.Ini();

                    if (key.OnePush) scene = Scene.play;
                    if (key.TwoPush) scene = Scene.title;
                    break;
                    //※※↑
                case Scene.end:
                    button.Button();//※※
                    if (key.IsPushKey) scene = Scene.title;
                    break;
            }

            Debug.WriteLine(scene);

            // この上にロジックを記述
            base.Update(gameTime); // 親クラスの更新処理呼び出し。絶対に消すな！！
        }

        /// <summary>
        /// 描画処理
        /// </summary>
        /// <param name="gameTime">現在のゲーム時間を提供するオブジェクト</param>
        protected override void Draw(GameTime gameTime)
        {
            // 画面クリア時の色を設定
            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin();

            switch (scene)
            {
                case Scene.title:
                    button.Draw(spriteBatch);//※※
                    ui.Draw_Title(spriteBatch);//※※
                    break;
                case Scene.tutlial:
                    button.Draw(spriteBatch);//※※
                    ui.Draw_Tutlial(spriteBatch);//※※
                    break;
                case Scene.play:
                    anime.Draw(spriteBatch,player.Pos,player.SC,player.St);
                    map.Draw(spriteBatch, player.SC);
                    //***
                    enemy.Draw(spriteBatch,player.SC);
                    positionBar.Draw(spriteBatch);
                    //***
                    ui.Draw(spriteBatch,player.SC);
                    break;
                case Scene.retry:
                    ui.Draw_Lose(spriteBatch);
                    break;
                case Scene.end:
                    button.Draw(spriteBatch);//※※
                    break;
            }
            spriteBatch.End();

            base.Draw(gameTime); // 親クラスの更新処理呼び出し。絶対に消すな！！
        }
    }
}