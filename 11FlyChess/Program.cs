using System;

namespace _11FlyChess
{
    #region 2.场景选择相关
    //场景枚举
    enum E_SceneType
    {
        /// <summary>
        /// 开始场景
        /// </summary>
        Begin,
        /// <summary>
        /// 游戏场景
        /// </summary>
        Game,
        /// <summary>
        /// 结束场景
        /// </summary>
        End
    }
    #endregion
    #region 格子枚举
    enum E_Grid_Type
    {
        /// <summary>
        /// 空格子
        /// </summary>
        normal,
        /// <summary>
        /// 暂停
        /// </summary>
        stop,
        /// <summary>
        /// 炸弹
        /// </summary>
        bomb,
        /// <summary>
        /// 时空穿梭
        /// </summary>
        timer,
    }
    #endregion
    #region 格子坐标结构体
    struct Position
    {
        public int x;
        public int y;
        public Position(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
    }
    #endregion
    #region 格子结构体
    struct Grid
    {
        //格子类型
        public  E_Grid_Type type;
        //格子坐标
        public Position pos;

        //初始化构造函数
        public Grid(E_Grid_Type type,int x,int y)
        {
            this.type = type;
            pos.x = x;
            pos.y = y;
        }

        //画出格子的函数
        public void Draw()
        {
            Console.SetCursorPosition(pos.x, pos.y);
            switch (this.type)
            {
                case E_Grid_Type.normal:
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write("□");
                    break;
                case E_Grid_Type.stop:
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.Write("||");
                    break;
                case E_Grid_Type.bomb:
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write("●");
                    break;
                case E_Grid_Type.timer:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.Write("¤");
                    break;
                default:
                    break;
            }
        }
    }
    #endregion
    #region 地图结构体
    struct Map
    {
        public Grid[] grids;
        //构造函数
        public Map(int x,int y,int number)
        {
            //创建number个格子
            grids = new Grid[number];
            Random rand = new Random();
            //用来标记x轴移动的次数
            int indexX = 0;
            //用来标价y轴移动的次数
            int indexY = 0;
            //用来标记x轴每次增长的步长
            int Xtemp = 2;
            //随机格子类型--------
            for (int i = 0; i < grids.Length; i++)
            {
                //生成随机数来判断格子的类型
                int value = rand.Next(1, 101);
                //普通格子的概率：85%
                if (value<=85||i==0||i==number-1)
                {
                    grids[i].type = E_Grid_Type.normal;
                }
                //炸弹的概率：5%
               else if (value>85&&value<=90)
                {
                    grids[i].type = E_Grid_Type.bomb;
                }
                //暂停的概率：5%
                else if(value>90&&value<=95)
                {
                    grids[i].type = E_Grid_Type.stop;
                }
                //时空穿梭的概率
                else
                {
                    grids[i].type = E_Grid_Type.timer;
                }

                //设置格子坐标-------
                grids[i].pos = new Position(x, y);
                //如果x轴增加到10，y轴加1
                if (indexX ==10)
                {
                    //y轴加1
                    y += 1;
                    indexY++;
                    //如果y轴标记增加到2
                    if (indexY==2)
                    {
                        //x轴标记清为0
                        indexX = 0;
                        //y轴标记清为0
                        indexY = 0;
                        //使x的步长反向增长
                        Xtemp = -Xtemp;
                    }
                }
                //如果x轴的移动次数小于10
                else
                {
                    //次数加1
                    indexX++;
                    //x轴的坐标加上步长
                    x += Xtemp;
                }
            }
        }

        //画出地图函数
        public void Draw()
        {
            //通过循环画出格子数组中的每一个格子
            for (int i = 0; i < grids.Length; i++)
            {
                //通过格子结构体重的绘画函数画出格子
                grids[i].Draw();
            }
        }
    }
    #endregion
    #region 玩家类型枚举
    enum E_Player_Type
    {
        /// <summary>
        /// 玩家
        /// </summary>
        Player,
        /// <summary>
        /// 电脑
        /// </summary>
        Computer
    }
    #endregion
    #region 玩家结构体
    struct Player
    {
        //玩家类型
        public E_Player_Type type;
        //玩家在哪一个格子
        public int nowIndex;
        //判断玩家是否暂停
        public bool isStop;

        //构造函数
        public Player(int index,E_Player_Type type)
        {
            this.nowIndex = index;
            this.type = type;
            this.isStop = false;
        }
        //在地图上画出玩家
        public void DrawPlayer(Map mapInfo)
        {
            //得到我在哪一个格子上
            Grid grid = mapInfo.grids[nowIndex];
            //得到我的位置
            Console.SetCursorPosition(grid.pos.x,grid.pos.y);
            Console.ForegroundColor = ConsoleColor.Blue;
            //绘画出不同的玩家类型
            switch (type)
            {
                case E_Player_Type.Player:
                    Console.Write("★");
                    break;
                case E_Player_Type.Computer:
                    Console.Write("▲");
                    break;
                default:
                    break;
            }
        }
    }
    #endregion
   
    class Program
    {
        #region 1.控制台的基本设置函数
        public static void ConsoleInit(int x,int y)
        {
            //设置窗口的大小
            Console.SetWindowSize(x, y);
            //设置缓冲区大小
            Console.SetBufferSize(x, y);
            //对光标进行隐藏
            Console.CursorVisible= false; ;
        }
        #endregion
        #region 2.开始和结束场景逻辑函数
        public static void BeginAndEndScene(int x,int y,ref E_SceneType nowSceneType)
        {
            Console.SetCursorPosition(nowSceneType == E_SceneType.Begin ? x / 2 - 3 : x / 2 - 4, 5);
            Console.Write(nowSceneType == E_SceneType.Begin ? "飞行棋":"游戏结束");
            int nowSceneId = 0;
            bool isSelect = false;
            while (true)
            {
                Console.SetCursorPosition(nowSceneType == E_SceneType.Begin ? x / 2 - 4 : x / 2 - 6, 9);
                Console.ForegroundColor = nowSceneId == 0 ? ConsoleColor.Red : ConsoleColor.White;
                Console.Write(nowSceneType == E_SceneType.Begin ? "开始游戏" : "回到开始界面");
                Console.SetCursorPosition(x / 2-4, 11);
                Console.ForegroundColor = nowSceneId == 1 ? ConsoleColor.Red : ConsoleColor.White;
                Console.Write("退出游戏");
                switch (Console.ReadKey(true).Key)
                {
                    case ConsoleKey.W:
                        nowSceneId = 0;
                        break;
                    case ConsoleKey.S:
                        nowSceneId = 1;
                        break;
                    case ConsoleKey.J:
                        if (nowSceneId==0)
                        {
                            nowSceneType = E_SceneType.Game;
                            isSelect = true;
                        }
                        else
                        {
                            Environment.Exit(0);
                        }
                        break;
                }
                if (isSelect)
                {
                    break;
                }
            }
        }
        #endregion
        #region   设置红墙
        public static void RedWall(int x, int y)
        {
            string wall = "■";
            string gezi = "□";
            string stop = "‖";
            string bomb = "●";
            string timer = "¤";
            string player = "★";
            string com = "▲";
            string cover = "※";
            //设置第二行红墙y轴坐标
            int row2 = y - 11;
            //设置第三行红墙y轴坐标
            int row3 = y - 6;
            for (int i = 0; i < x - 1; i += 2)
            {
                //最上方的墙
                Console.ForegroundColor = ConsoleColor.Red;
                Console.SetCursorPosition(i, 0);
                Console.Write(wall);
                //第二行的墙
                Console.SetCursorPosition(i, row2);
                Console.Write(wall);
                //第三行的墙
                Console.SetCursorPosition(i, row3);
                Console.Write(wall);
                //最下方的墙
                Console.SetCursorPosition(i, y - 1);
                Console.Write(wall);
            }
            for (int i = 0; i < y; i++)
            {
                //左边的墙
                Console.SetCursorPosition(0, i);
                Console.Write(wall);
                //右边的墙
                Console.SetCursorPosition(x - 2, i);
                Console.Write(wall);
            }
            //设置显示的固定信息
            Console.SetCursorPosition(2, row2 + 1);
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write($"{gezi}:普通格子");
            Console.SetCursorPosition(2, row2 + 2);
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write($"{stop}:暂停，一回合不动");
            Console.SetCursorPosition(x / 2, row2 + 2);
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write($"{bomb}:炸弹，倒退五格");
            Console.SetCursorPosition(2, row2 + 3);
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write($"{timer}:时空隧道:随机倒退，暂停，换位置");
            Console.SetCursorPosition(2, row2 + 4);
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write($"{player}:玩家  {com}:电脑  {cover}:玩家电脑重合");
            Console.SetCursorPosition(2, row3 + 1);
            Console.WriteLine("按任意键开始扔骰子");
        }
        #endregion
        #region 画出玩家
        public static void DrawPlayer(Player player, Player Computer, Map map)
        {
            //当玩家和电脑重合时
            if (player.nowIndex==Computer.nowIndex)
            {
                //获取重合位置所在的格子
                Grid grid = map.grids[player.nowIndex];
                Console.SetCursorPosition(grid.pos.x,grid.pos.y);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("※");
            }
            //当玩家和电脑没有重合时
            else
            {
                player.DrawPlayer(map);
                Computer.DrawPlayer(map);
            }
        }
        #endregion
        #region 玩家和电脑投色子
        //清空上一回合输出的内容
        static void ClearPrint(int y)
        {
            Console.SetCursorPosition(2, y - 5);
            Console.Write("                                            ");
            Console.SetCursorPosition(2, y - 4);
            Console.Write("                                            ");
            Console.SetCursorPosition(2, y - 3);
            Console.Write("                                            ");
            Console.SetCursorPosition(2, y - 2);
            Console.Write("                                            ");
        }    
        static bool RandMove(ref Player p,ref Player OtherP,Map map,int x,int y )
        {
            //根据玩家类型设置前景色
            Console.ForegroundColor = p.type == E_Player_Type.Player ? ConsoleColor.White : ConsoleColor.DarkYellow;
            //如果这一回合暂停
            if (p.isStop)
            {
                Console.SetCursorPosition(2, y - 5);
                Console.Write("此回合{0}暂停", p.type == E_Player_Type.Player ? "你" : "电脑");
                Console.SetCursorPosition(2, y - 4);
                Console.Write("按任意键{0}投色子", p.type == E_Player_Type.Player ? "电脑" : "你");
                p.isStop = false;
                return false;
            }
            Random rand = new Random();
            //生成玩家随机色子随机数1-6
            int randNumber = rand.Next(1, 7);
            //玩家的当前位置
            Grid grid = map.grids[p.nowIndex];
            //玩家随机移动
            p.nowIndex += randNumber;
            //如果玩家的坐标超过了地图最大的索引，证明到达终点
            if (p.nowIndex >= map.grids.Length - 1)
            {
                //让玩家的位置在最后一个格子上
                p.nowIndex = map.grids.Length - 1;
                //游戏结束
                return true;
            }
            else
            {
                //如果所在格子了类型是炸弹
                if (map.grids[p.nowIndex].type == E_Grid_Type.bomb)
                {
                    //玩家后退5格
                    p.nowIndex -= 5;
                    if (p.nowIndex<=0)
                    {
                        p.nowIndex = 0;
                    }
                    Console.SetCursorPosition(2, y - 5);
                    Console.Write("{1}投出了{0}点", randNumber, p.type == E_Player_Type.Player ? "你" : "电脑");
                    Console.SetCursorPosition(2, y - 4);
                    Console.Write("遇到炸弹倒退5格");

                }
                //如果玩家所在的表格类型是正常表格
                else if (map.grids[p.nowIndex].type == E_Grid_Type.normal)
                {
                    Console.SetCursorPosition(2, y - 5);
                    Console.Write("{1}投出了{0}点", randNumber, p.type == E_Player_Type.Player ? "你" : "电脑");
                    Console.SetCursorPosition(2, y - 4);
                    Console.Write("到达一个安全位置");
                    Console.SetCursorPosition(2, y - 3);
                    Console.Write("按任意键{0}投色子", p.type == E_Player_Type.Player ? "电脑" : "你");
                }
                //如果玩家所在的表格是暂停
                else if (map.grids[p.nowIndex].type==E_Grid_Type.stop)
                {
                    Console.SetCursorPosition(2, y - 5);
                    Console.Write("{1}投出了{0}点", randNumber, p.type == E_Player_Type.Player ? "你" : "电脑");
                    Console.SetCursorPosition(2, y - 4);
                    Console.Write("处于暂停点，下一回合暂停！！！");
                    p.isStop = true;
                }
                //如果玩家所在的表格是时空隧道
                else
                {
                    //有三分之一的概率后退1~5格
                    if (rand.Next(1,91)<30)
                    {
                        int TimeRand = rand.Next(1, 6);
                        p.nowIndex -= TimeRand;
                        Console.SetCursorPosition(2, y - 5);
                        Console.Write("{1}投出了{0}点", randNumber, p.type == E_Player_Type.Player ? "你" : "电脑");
                        Console.SetCursorPosition(2, y - 4);
                        Console.Write("到达时空隧道，往后退{0}格！",TimeRand);
                        Console.SetCursorPosition(2, y - 3);
                        Console.Write("按任意键{0}投色子", p.type == E_Player_Type.Player ? "电脑" : "你");
                    }
                    //有三分之一的概率暂停一局
                    else if (rand.Next(1,91)<60)
                    {
                        Console.SetCursorPosition(2, y - 5);
                        Console.Write("{1}投出了{0}点", randNumber, p.type == E_Player_Type.Player ? "你" : "电脑");
                        Console.SetCursorPosition(2, y - 4);
                        Console.Write("到达时空隧道，下一回合进行暂停！！！");
                        Console.SetCursorPosition(2, y - 3);
                        Console.Write("按任意键{0}投色子", p.type == E_Player_Type.Player ? "电脑" : "你");
                        p.isStop = true;
                    }
                    //其它三分之一的几率交换位置
                    else
                    {
                        Console.SetCursorPosition(2, y - 5);
                        Console.Write("{1}投出了{0}点", randNumber, p.type == E_Player_Type.Player ? "你" : "电脑");
                        Console.SetCursorPosition(2, y - 4);
                        Console.Write("到达时空隧道，和对手交换位置！！！");
                        Console.SetCursorPosition(2, y - 3);
                        Console.Write("按任意键{0}投色子", p.type == E_Player_Type.Player ? "电脑" : "你");
                        int temp = p.nowIndex;
                        p.nowIndex = OtherP.nowIndex;
                        OtherP.nowIndex = temp;
                    }
                }
                return false;
            }
        }
        #endregion
        #region 玩家和电脑移动函数
        static bool PlayerMove(int x,int y,ref Player player,ref Player computer,Map map,ref E_SceneType sceneType)
        {
            //检测输入
            Console.ReadKey(true);
            //清空上次输出信息
            ClearPrint(y);
            //投色子
            bool isGameOver = RandMove(ref player, ref computer, map, x, y);
            //绘制地图
            map.Draw();
            //绘制玩家
            DrawPlayer(player, computer, map);
            //判断游戏是否结束
            if (isGameOver)
            {
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.SetCursorPosition(x / 2 - 9, y - 5);
                Console.Write("恭喜{0}胜利了！！！",player.type==E_Player_Type.Player?"你":"电脑");
                Console.SetCursorPosition(2, y - 4);
                Console.Write("按任意键继续！！");
                Console.ReadKey(true);
                sceneType = E_SceneType.End;
                return true;
            }
            return false;
        }
        #endregion

        #region 3.游戏场景
        public static void GameScene(int x,int y,ref E_SceneType sceneType)
        {
            //画出红墙
            RedWall(x, y);
            //设置格子的起始坐标和格子数量
            //创建地图结构体
            Map map = new Map(16, 3, 86);
            //调用地图结构体的绘画函数，画出地图
            map.Draw();

            //创建玩家和电脑的结构体
            Player player = new Player(0, E_Player_Type.Player);
            Player computer = new Player(0, E_Player_Type.Computer);
            //调用绘制玩家的函数
            DrawPlayer(player, computer, map);

            //判断是否结束游戏
            //bool isGameOver = false;
            while (true)
            {
                ////玩家扔色子逻辑
                ////检测输入
                //Console.ReadKey(true);
                ////扔色子逻辑
                //isGameOver = RandMove(ref player,ref computer,map,x,y);
                ////绘制地图
                //map.Draw();
                ////绘制玩家
                //DrawPlayer(player, computer, map);
                ////判断是否要结束游戏
                //if (isGameOver)
                //{
                //    Console.ForegroundColor = ConsoleColor.Gray;
                //    Console.SetCursorPosition(x/2-9, y - 5);
                //    Console.Write("恭喜你胜利了！！！");
                //    Console.SetCursorPosition(2, y - 4);
                //    Console.Write("按任意键继续！！");
                //    Console.ReadKey(true);
                //    sceneType = E_SceneType.End;
                //    break;
                //}

                ////电脑扔色子逻辑
                ////检测输入
                //Console.ReadKey(true);
                ////扔色子逻辑
                //isGameOver = RandMove(ref computer, ref player, map, x, y);
                ////绘制地图
                //map.Draw();
                ////绘制电脑
                //DrawPlayer(player, computer, map);
                ////判断是否要结束游戏
                //if (isGameOver)
                //{
                //    Console.ForegroundColor = ConsoleColor.DarkGray;
                //    Console.SetCursorPosition(x/2-10, y - 5);
                //    Console.Write("恭喜电脑胜利了！！！");
                //    Console.SetCursorPosition(2, y - 4);
                //    Console.Write("按任意键继续！！");
                //    Console.ReadKey(true);
                //    sceneType = E_SceneType.End;
                //    break;
                //}

                //调用投色子后玩家移动的函数
                if (PlayerMove(x,y,ref player,ref computer,map,ref sceneType))
                {
                    break;
                }
                if (PlayerMove(x, y, ref computer, ref player, map,ref sceneType))
                {
                    break;
                }
            }
        }
        #endregion
        static void Main(string[] args)
        {
            #region 1.控制台宽高设置
            int x = 50;
            int y = 30;
            ConsoleInit(x, y);
            #endregion
            #region 2.场景选择
            E_SceneType nowSceneType = E_SceneType.Begin;
            while (true)
            {
                switch (nowSceneType)
                {
                    case E_SceneType.Begin:
                        BeginAndEndScene(x, y,ref nowSceneType);
                        break;
                    case E_SceneType.Game:
                        Console.Clear();
                        GameScene(x, y,ref nowSceneType);
                        break;
                    case E_SceneType.End:
                        Console.Clear();
                        BeginAndEndScene(x, y, ref nowSceneType);
                        break;
                }
            }
            #endregion
        }
    }
}
