using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleProject1
{
    class Program
    {
        // 수정 테스트 3

        #region 이넘
        enum Classes
        {
            전사, 도적, 사냥꾼, 마법사, 사제
        }

        enum Tribe
        {
            인간, 오크, 하플링, 엘프, 하프엘프, 드래곤본
        }

        enum MonsterType
        {
            일반, 엘리트, 보스
        }

        enum Map
        {
            마을, 상점, 평화로운사냥터, 위험한사냥터
        }
        #endregion

        #region 구조체

        struct MapData
        {
            public string worldName;
            public char[,] mapping;
            public Portal[] portals;
        }

        struct PlayerData
        {
            public string playerName;
            public int playerCode;
            public Classes classes;
            public Tribe tribe;

            public Item[] inventory;
            public float money;
        }


        struct Item
        {
            public string itemName;
            public int itemCode;

            public string itemDefine;

            public float priceBuy;
            public float priceSell;

            public float dropProb;
        }

        struct Monster
        {
            public string monsterName;
            public Position mosnterPos;
            public MonsterType monsterType;
            public float dropMoney;
            public bool isDead;
        }


        struct Position
        {
            public int x;
            public int y;
        }

        struct Portal
        {
            public Position portalPos;
            public Map nextMap;
            public Position nextPos;
        }
        #endregion

        #region 시작 설정

        static void ShowTitle()
        {
            Console.WriteLine("-----------------");
            Console.WriteLine(" 레전드 RPG");
            Console.WriteLine("-----------------");
            Console.WriteLine();
            Console.WriteLine("아무키나 눌러서 시작하세요...");

            Console.ReadKey(true);
            Console.Clear();
        }

        // 게임시작시 작업
        static void Start(out Position playerPos, out MapData mapData, ref char[,] mapping)
        {
            // 게임 설정
            Console.CursorVisible = false;

            // 플레이어 초기 위치 설정하기
            playerPos.x = 1;
            playerPos.y = 1;


            // 포탈 설정

            // 맵 설정하기
            settingMaps(Map.마을, out mapData, ref mapping);
            ShowTitle();
        }

        #endregion

        /***메인 함수**********************************************************************************************************/
        static void Main(string[] args)
        {
            bool gameOver = false;
            Position playerPos = default;
            Position playerDir = default;
            MapData mapData = default;                    //Update에서 포탈 체크가 되면 갱신됨.
            char[,] mapping = null;


            // 시작 설정 - 1회만
            Start(out playerPos, out mapData, ref mapping);



            // 반복 진행 (Update)
            while (gameOver == false)
            {
                Update(ref playerPos, ref playerDir, ref mapData, ref gameOver, ref mapping);
            }
            return;
        }
        /**************************************************************************************************************************/

        static void Update(ref Position playerPos, ref Position playerDir, ref MapData mapData, ref bool gameOver, ref char[,] mapping)
        {
            Render(playerPos, playerDir, mapData);
            ConsoleKey key = Input();

            Move(key, ref playerPos, mapData, ref playerDir);

            //PlayerAttack(key, playerPos, playerDir);

            // 포탈 체크하고
            CheckPortal(ref playerPos, ref mapData, ref mapping);
            mapping = mapData.mapping;

            // esc 키입력 체크
            EndCheck(key, ref gameOver);
        }


        static void PlayerAttack(ConsoleKey key, Position playerPos, Position dir, char[,] mapping)
        {
            if (key == ConsoleKey.Z || key == ConsoleKey.Delete)
            {
                // 공격 방향
                Console.SetCursorPosition(playerPos.x + dir.x, playerPos.y + dir.y);

                if (dir.x == 1)
                {
                    if (mapping[playerPos.y, playerPos.x + 1] == '●')
                    {
                        // 공격 효과
                        // 공격 처리
                    }

                }
                else if (dir.x == -1)
                {
                    Console.Write("◀");
                }
                else if (dir.y == 1)
                {
                    Console.Write("▼");
                }
                else if (dir.y == -1)
                {
                    Console.Write("▲");
                }
                else
                {
                    Console.Write('♥');
                }

            }
        }

        static Monster MonsterMaker(string name, int x, int y, float money, MonsterType type, bool isDead)
        {
            Monster monster;
            monster.monsterName = name;
            monster.mosnterPos.x = x;
            monster.mosnterPos.y = y;
            monster.dropMoney = 30f;
            monster.monsterType = type;
            monster.isDead = isDead;
            return monster;
        }

        static void SpawnMonster(Monster mons, char[,] map)
        {
            if (mons.monsterType == MonsterType.보스)
            {
                map[mons.mosnterPos.y, mons.mosnterPos.x] = '★';
            }
            else if (mons.monsterType == MonsterType.엘리트)
            {
                map[mons.mosnterPos.y, mons.mosnterPos.x] = '◆';
            }
            else
            {
                map[mons.mosnterPos.y, mons.mosnterPos.x] = '●';
            }
        }

        // 출력작업
        static void Render(Position playerPos, Position playerDir, MapData mapData)
        {
            Console.SetCursorPosition(0, 0);
            PrintMap(mapData);
            PrintPlayer(playerPos, playerDir);
        }

        static void PrintPlayer(Position playerPos, Position dir)
        {
            // 플레이어 위치로 커서 옮기기
            Console.SetCursorPosition(playerPos.x, playerPos.y);
            // 플레이어 출력
            Console.ForegroundColor = ConsoleColor.Green;

            if (dir.x == 1)
            {
                Console.Write('▶');
            }
            else if (dir.x == -1)
            {
                Console.Write("◀");
            }
            else if (dir.y == 1)
            {
                Console.Write("▼");
            }
            else if (dir.y == -1)
            {
                Console.Write("▲");
            }
            else
            {
                Console.Write('♥');
            }
            Console.ResetColor();
        }



        static void Move(ConsoleKey key, ref Position playerPos, MapData mapData, ref Position playerDirection)
        {
            switch (key)
            {
                case ConsoleKey.A:
                case ConsoleKey.LeftArrow:
                    if (mapData.mapping[playerPos.y, playerPos.x - 1] == ' ')
                    {
                        playerPos.x--;
                        playerDirection.x = -1;
                        playerDirection.y = 0;
                    }
                    break;
                case ConsoleKey.D:
                case ConsoleKey.RightArrow:
                    if (mapData.mapping[playerPos.y, playerPos.x + 1] == ' ')
                    {
                        playerPos.x++;
                        playerDirection.x = 1;
                        playerDirection.y = 0;
                    }
                    break;
                case ConsoleKey.W:
                case ConsoleKey.UpArrow:
                    if (mapData.mapping[playerPos.y - 1, playerPos.x] == ' ')
                    {
                        playerPos.y--;
                        playerDirection.x = 0;
                        playerDirection.y = -1;
                    }
                    break;
                case ConsoleKey.S:
                case ConsoleKey.DownArrow:
                    if (mapData.mapping[playerPos.y + 1, playerPos.x] == ' ')
                    {
                        playerPos.y++;
                        playerDirection.x = 0;
                        playerDirection.y = 1;
                    }
                    break;
            }
        }


        #region 잠시


        // 포탈 도착 체크
        static void CheckPortal(ref Position playerPos, ref MapData mapData, ref char[,] mapping)
        {
            for (int i = 0; i < mapData.portals.Length; i++)
            {
                if ((playerPos.x == mapData.portals[i].portalPos.x) && (playerPos.y == mapData.portals[i].portalPos.y))
                {
                    Console.Clear();
                    MapData Map;

                    // 먼저 기존 맵의 포탈에서 플레이어 위치 데이터 얻어오기
                    playerPos.x = mapData.portals[i].nextPos.x;
                    playerPos.y = mapData.portals[i].nextPos.y;

                    // 그 이후 맵데이터 갱신
                    settingMaps(mapData.portals[i].nextMap, out Map, ref mapping);
                    mapData = Map;
                    mapping = mapData.mapping;

                    break;
                }
            }
        }


        static Portal settingPortal(int x, int y, Map nextMap, int nextX, int nextY)
        {
            Portal portal;
            portal.portalPos.x = x;
            portal.portalPos.y = y;
            portal.nextPos.x = nextX;
            portal.nextPos.y = nextY;
            portal.nextMap = nextMap;

            return portal;
        }


        static void settingMaps(Map map, out MapData mapData, ref char[,] mapping)
        {
            mapData.worldName = "";
            mapData.mapping = null;


            switch (map)
            {
                case Map.마을:
                    mapData.worldName = "마을";
                    mapData.portals = new Portal[3];
                    mapData.portals[0] = settingPortal(6, 1, Map.상점, 6, 8);
                    mapData.portals[1] = settingPortal(13, 5, Map.평화로운사냥터, 1, 5);
                    mapData.portals[2] = settingPortal(1, 8, Map.위험한사냥터, 6, 1);
                    mapData.mapping = new char[10, 15]
                    {
                          // 0    1    2    3    4    5    6    7    8    9    10   11   12   13   14
                    /*0*/ { '▒', '▒', '▒', '▒', '▒', '▒', '▒', '▒', '▒', '▒', '▒', '▒', '▒', '▒', '▒' },
                    /*1*/ { '▒', ' ', ' ', ' ', ' ', '▒', ' ', '▒', ' ', ' ', ' ', ' ', '▒', '▒', '▒' },
                    /*2*/ { '▒', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '▒', '▒' },
                    /*3*/ { '▒', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '▒', '▒', '▒' },
                    /*4*/ { '▒', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '▒', '▒', '▒' },
                    /*5*/ { '▒', '▒', '▒', '▒', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '▒' },
                    /*6*/ { '▒', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '▒', '▒', '▒' },
                    /*7*/ { '▒', ' ', '▒', '▒', '▒', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '▒', '▒', '▒' },
                    /*8*/ { '▒', ' ', '▒', '▒', '▒', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '▒', '▒', '▒' },
                    /*9*/ { '▒', '▒', '▒', '▒', '▒', '▒', '▒', '▒', '▒', '▒', '▒', '▒', '▒', '▒', '▒' },
                    };

                    break;

                case Map.상점:
                    mapData.worldName = "상점";
                    mapData.portals = new Portal[1];
                    mapData.portals[0] = settingPortal(6, 8, Map.마을, 6, 1);
                    mapData.mapping = new char[10, 15]
                    {
                          // 0    1    2    3    4    5    6    7    8    9    10   11   12   13   14
                    /*0*/ { '▒', '▒', '▒', '▒', '▒', '▒', '▒', '▒', '▒', '▒', '▒', '▒', '▒', '▒', '▒' },
                    /*1*/ { '▒', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '▒', ' ', ' ', ' ', '▒', '▒', '▒' },
                    /*2*/ { '▒', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '▒', ' ', '$', ' ', '▒', '▒', '▒' },
                    /*3*/ { '▒', ' ', ' ', '▒', ' ', ' ', ' ', ' ', '▒', ' ', ' ', ' ', '▒', '▒', '▒' },
                    /*4*/ { '▒', '$', '$', '▒', ' ', ' ', ' ', ' ', '▒', ' ', ' ', ' ', '▒', '▒', '▒' },
                    /*5*/ { '▒', '▒', '▒', '▒', ' ', ' ', ' ', ' ', ' ', '▒', '▒', ' ', '▒', '▒', '▒' },
                    /*6*/ { '▒', '▒', '▒', '▒', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '▒', '▒', '▒' },
                    /*7*/ { '▒', '▒', '▒', '▒', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '▒', '▒', '▒' },
                    /*8*/ { '▒', '▒', '▒', '▒', '▒', '▒', ' ', '▒', '▒', '▒', '▒', '▒', '▒', '▒', '▒' },
                    /*9*/ { '▒', '▒', '▒', '▒', '▒', '▒', '▒', '▒', '▒', '▒', '▒', '▒', '▒', '▒', '▒' },
                    };
                    break;

                case Map.평화로운사냥터:
                    mapData.worldName = "평화로운 사냥터";

                    mapData.portals = new Portal[1];
                    mapData.portals[0] = settingPortal(1, 5, Map.마을, 13, 5);

                    mapData.mapping = new char[10, 15]
                    {
                          // 0    1    2    3    4    5    6    7    8    9    10   11   12   13   14
                    /*0*/ { '▒', '▒', '▒', '▒', '▒', '▒', '▒', '▒', '▒', '▒', '▒', '▒', '▒', '▒', '▒' },
                    /*1*/ { '▒', '▒', '▒', '▒', ' ', ' ', ' ', ' ', ' ', '▒', ' ', ' ', '▒', '▒', '▒' },
                    /*2*/ { '▒', '▒', '▒', '▒', ' ', ' ', ' ', ' ', ' ', '▒', ' ', ' ', '▒', '▒', '▒' },
                    /*3*/ { '▒', '▒', '▒', '▒', ' ', ' ', ' ', ' ', ' ', '▒', ' ', ' ', '▒', '▒', '▒' },
                    /*4*/ { '▒', '▒', '▒', '▒', ' ', ' ', ' ', ' ', ' ', '▒', ' ', ' ', '▒', '▒', '▒' },
                    /*5*/ { '▒', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '▒', '▒', ' ', '▒', '▒', '▒' },
                    /*6*/ { '▒', '▒', '▒', '▒', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '▒', '▒', '▒' },
                    /*7*/ { '▒', '▒', '▒', '▒', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '▒', '▒', '▒' },
                    /*8*/ { '▒', '▒', '▒', '▒', '▒', '▒', ' ', '▒', '▒', '▒', '▒', '▒', '▒', '▒', '▒' },
                    /*9*/ { '▒', '▒', '▒', '▒', '▒', '▒', '▒', '▒', '▒', '▒', '▒', '▒', '▒', '▒', '▒' },
                    };

                    mapping = mapData.mapping;
                    Monster slime = MonsterMaker("슬라임", 8, 3, 30f, MonsterType.일반, false);
                    SpawnMonster(slime, mapping);

                    break;

                case Map.위험한사냥터:
                    mapData.worldName = "위험한 사냥터";

                    mapData.portals = new Portal[1];
                    mapData.portals[0] = settingPortal(6, 1, Map.마을, 1, 8);

                    mapData.mapping = new char[10, 15]
                    {
                          // 0    1    2    3    4    5    6    7    8    9    10   11   12   13   14
                    /*0*/ { '▒', '▒', '▒', '▒', '▒', '▒', '▒', '▒', '▒', '▒', '▒', '▒', '▒', '▒', '▒' },
                    /*1*/ { '▒', '▒', '▒', '▒', '▒', '▒', ' ', '▒', '▒', '▒', ' ', ' ', '▒', '▒', '▒' },
                    /*2*/ { '▒', '▒', '▒', '▒', '▒', '▒', ' ', '▒', '▒', '▒', ' ', ' ', '▒', '▒', '▒' },
                    /*3*/ { '▒', '▒', ' ', ' ', '▒', '▒', ' ', '▒', '▒', '▒', ' ', ' ', '▒', '▒', '▒' },
                    /*4*/ { '▒', '▒', ' ', ' ', ' ', '▒', ' ', '▒', '▒', '▒', ' ', ' ', '▒', '▒', '▒' },
                    /*5*/ { '▒', ' ', ' ', ' ', ' ', ' ', ' ', '▒', '▒', ' ', ' ', ' ', '▒', '▒', '▒' },
                    /*6*/ { '▒', ' ', ' ', '▒', ' ', ' ', ' ', ' ', ' ', '▒', ' ', ' ', '▒', '▒', '▒' },
                    /*7*/ { '▒', ' ', ' ', '▒', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '▒', '▒', '▒' },
                    /*8*/ { '▒', ' ', ' ', '▒', '▒', '▒', ' ', '▒', '▒', '▒', '▒', '▒', '▒', '▒', '▒' },
                    /*9*/ { '▒', '▒', '▒', '▒', '▒', '▒', '▒', '▒', '▒', '▒', '▒', '▒', '▒', '▒', '▒' },
                    };

                    mapping = mapData.mapping;
                    Monster dragon = MonsterMaker("드래곤", 11, 2, 500f, MonsterType.보스, false);
                    Monster witch = MonsterMaker("마녀", 1, 7, 500f, MonsterType.엘리트, false);
                    SpawnMonster(dragon, mapping);
                    SpawnMonster(witch, mapping);

                    break;

                default:
                    mapData.portals = new Portal[0];
                    break;


            }


        }





        // 맵 출력     
        static void PrintMap(MapData mapData)
        {
            for (int y = 0; y < mapData.mapping.GetLength(0); y++)
            {
                for (int x = 0; x < mapData.mapping.GetLength(1); x++)
                {
                    if (mapData.mapping[y, x] == '★' || mapData.mapping[y, x] == '◆')
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                    }
                    else if (mapData.mapping[y, x] == '●')
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                    }
                    else if (mapData.mapping[y, x] == '$')
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                    }
                    Console.Write(mapData.mapping[y, x]);
                    Console.ResetColor();
                }
                Console.WriteLine();
            }


            Console.WriteLine();
            Console.WriteLine("현재 장소 : {0}", mapData.worldName);


            for (int i = 0; i < mapData.portals.Length; i++)
            {
                PrintPortal(mapData.portals[i]);
            }
        }

        // 포탈 출력
        static void PrintPortal(Portal portal)
        {
            Console.SetCursorPosition(portal.portalPos.x, portal.portalPos.y);
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write('◎');
            Console.ResetColor();
        }







        // 입력작업
        static ConsoleKey Input()
        {
            ConsoleKey input = Console.ReadKey(true).Key;
            return input;
        }

        static void EndCheck(ConsoleKey key, ref bool gameOver)
        {
            if (key == ConsoleKey.Escape)
            {
                Console.SetCursorPosition(3, 20);
                Console.WriteLine("종료하시겠습니까?");
                Console.WriteLine("Yes : Enter  ||||  No : OtherKey");
                ConsoleKey inputLast = Console.ReadKey(true).Key;
                if (inputLast == ConsoleKey.Enter)
                {
                    gameOver = true;
                    Console.Clear();
                    Console.SetCursorPosition(5, 5);
                    Console.WriteLine("게임 종료\n\n\n\n\n");
                }
                else
                {
                    Console.Clear();
                }
            }
        }
        #endregion

    }
}
