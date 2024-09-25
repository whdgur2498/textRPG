using System.Xml;

namespace textRPG
{
    // 스타트메뉴 숫자를 토대로 주석 정리  (예시로 아이템은 상점 관련으로 상점탭이 3번이니 3-1)
    public class Character // 1-1 캐릭터 상태 요소
    {
        public string Name { get; } 
        public string Job { get; } 
        public int Level { get; }
        public int Atk { get; }
        public int Def { get; }
        public int Hp { get; set; } 
        public int Gold { get; set; } 

        public Character(string name, string job, int level, int atk, int def, int hp, int gold) // 1-2 캐릭터상태 인스턴스 생성
        {
            Name = name;
            Job = job;
            Level = level;
            Atk = atk;
            Def = def;
            Hp = hp;
            Gold = gold;
        }
    }

    public class Item // 2-1 아이템 클래스 정의
    {
        public string Name { get; }
        // get;         = 읽기 전용: 객체가 생성될 때 값이 설정 되고 이후에는 변경 할 수 없음 
        // get; set;    = 읽기/쓰기 전용: 객체값이 프로그램 내부에서 변경될 수 있음


        public string Description { get; } // 아이템 설명

        public int Type { get; } // 무기와 방어구 구별

        public int Atk { get; }

        public int Def { get; }

        public int Gold { get; set; } 

        public bool IsEquipped { get; set; } 

        public bool IsPurchased { get; set; } 

        public static int ItemCnt = 0;
        // 클래스에 공유가 되는 int형 변수
        // 각각의 인스턴스가 아닌, 아이템 클래스에 귀속 되어 게임에 전반적으로 공유 되는 변수
        public Item(string name, string description, int type, int atk, int def, int gold, bool isEquipped = false) // 아이템스탯 인스턴스 생성
        {
            Name = name;
            Description = description;
            Type = type;
            Atk = atk;
            Def = def;
            Gold = gold; 
            IsEquipped = isEquipped;
            IsPurchased = false; // 시작을 장비없이 시작하니 기본설정을 false로 해두었다
        }

        public void PrintItemStatDescription(bool withNumber = false, int idx = 0, bool showPrice = true, bool v = false)// 3-1. 아이템 설명 출력
        {
            Console.Write("- ");

            if (withNumber) // 아이템 장착관리 번호 컬러
            {
                Console.ForegroundColor = ConsoleColor.DarkMagenta;
                Console.Write("{0} ", idx);
                Console.ResetColor();
            }
            if (IsEquipped) // 아이템 착용 시
            {
                Console.Write("[");
                Console.ForegroundColor = ConsoleColor.Green; // 장착중임을 표시하는 E에 초록색
                Console.Write("E"); 
                Console.ResetColor(); 
                Console.Write("]");
                Console.Write(PadRightForMixedText(Name, 9)); // 장착 중인 경우는 9글자 출력
            }
            Console.Write(PadRightForMixedText(Name, 12)); // 장착 중이 아닌 경우 12글자 출력
            Console.Write(" | ");

            // 수치가 0이 아니라면 실행
            if (Atk != 0) Console.Write($"Atk {(Atk >= 0 ? " + " : "")}{Atk}");  // 공격력이 0보다 크거나 같으면 "+" 를 붙이고 아니면 아무런행동을 안하는걸""로 표시.
            if (Def != 0) Console.Write($"Def {(Def >= 0 ? " + " : "")}{Def}");

            Console.Write(" | " + Description); // 설명 출력
            if (showPrice) // 3-2 상점관련, 가격 및 구매 완료 표시 bool 코드
            {
                // IsPurchased가 true이면 "구매완료", 아니면 가격을 출력
                string priceOrStatus = IsPurchased ? "구매완료" : Gold + " G";
                Console.WriteLine(" | " + priceOrStatus);
            }
            Console.WriteLine();
        }

        public static int GetPrintableLength(string str) // 아이템 텍스트 정렬을 위해 Length의 길이를 구함
        {
            int length = 0;
            foreach (char c in str)
            {
                if (char.GetUnicodeCategory(c) == System.Globalization.UnicodeCategory.OtherLetter)
                {
                    length += 2; // 한글과 같은 넓은 문자는 길이를 2로 취급
                }
                else
                {
                    length += 1; // 나머지 문자는 길이를 1로 취급
                }
            }
            return length;
        }
        public static string PadRightForMixedText(string str, int totalLength)
        {
            int currentLength = GetPrintableLength(str); // 텍스트의 실제 길이
            int padding = totalLength - currentLength; //  총길이 - 실제길이 = int padding 추가해야 할 길이
            return str.PadRight(str.Length + padding); // padding 만큼 PadRight(문자열의 오른쪽)에 공백을 추가
        }


    }
    internal class Program
    {
        static Character _player; // 실제 플레이에서 쓸 캐릭터와 아이템 추가
        static Item[] _items;     // 아이템은 여러 개 이므로 배열[] 사용

        static void Main(string[] args) // 메인 메서드
        {

            GameDataSetting(); // 게임 데이터 세팅

            StartMenu(); // 스타트 메뉴
        }


        private static void GameDataSetting() // 2. 게임 데이터 메서드 생성, Private(비공개)
        {
            _player = new Character("정진", "모험가", 1, 10, 5, 100, 5000); // new 플레이어 변수 선언(실제로 사용할 캐릭터의 데이터)

            _items = new Item[8];

            // 5-4. 아이템 추가
            AddItem(new Item("무쇠 갑옷", "무쇠로 만들어진 튼튼한 갑옷입니다.", 0, 0, 5, 1000)); // 맨 앞의 숫자가 0이면 방어구, 1이면 무기
            AddItem(new Item("수련자 갑옷", "수련에 도움을 주는 갑옷입니다.", 0, 0, 9, 2000));
            AddItem(new Item("스파르타의 갑옷", "스파르타의 전사들이 사용했다는 전설의 갑옷입니다.", 0, 0, 15, 3500));
            AddItem(new Item("심판의 갑옷", "성기사의 갑옷이다", 0, 0, 30, 5500)); 
            AddItem(new Item("낡은 검", "쉽게 볼 수 있는 낡은 검입니다.", 1, 2, 0, 600));
            AddItem(new Item("청동 도끼", "쉽게 볼 수 있는 낡은 검입니다.", 1, 5, 0, 1500));
            AddItem(new Item("펠로멜로른", "선스트라이더 왕조 대대로 전해져 내려오는 한손도검", 1, 7, 0, 3000));
            AddItem(new Item("둠해머", "오크의 고향인 드레노어 용암으로 제련된 거대한 망치", 1, 15, 0, 6000)); 
        }

        static void StartMenu() // 0-1 스타트 메뉴
        {
            Console.Clear(); // 게임 스타트 화면 정리
            Console.WriteLine("---------------------------------------------------");
            Console.WriteLine("아제로스에 오신 여러분 환영합니다.");
            Console.WriteLine("이곳에서 던전으로 들어가기전 활동을 할 수 있습니다.");
            Console.WriteLine("---------------------------------------------------");
            Console.WriteLine();  // Console.WriteLine(); 단축키 -> C + W + Tab
            Console.WriteLine("");
            Console.WriteLine("1. 상태 보기");
            Console.WriteLine("2. 인벤토리");
            Console.WriteLine("3. 상점");
            Console.WriteLine("4. 던전 입장");
            Console.WriteLine("5. 휴식");
            Console.WriteLine("6. 게임 종료");
            Console.WriteLine("");

            // int keyInput = int.TryParse(Console.ReadLine(), out keyInput);  는 안 쓰고 아래 따로 함수 생성

            // CheckValidInput(1, 6); 유효성 확인을 위한건데 일단 관련 함수들을 만들고, switch 문의 매개변수로 사용

            switch (CheckValidInput(1, 6))
            // CheckValidInput함수로 유효 값(1~6)을 입력 받으면 그에 맞는 함수 호출 후 break;로 벗어남.

            {
                case 1:
                    StatusMenu(); // 플레이어 상태
                    break;
                case 2:
                    InventoryMenu(); // 인벤토리
                    break;
                case 3:
                    StoreMenu(); // 상점
                    break;
                case 4:
                    DungeonMenu(); // 던전 입장
                    break;
                case 5:
                    RestMenu(); // 휴식
                    break;
                case 6:
                    GameOverMenu(); // 게임 종료
                    break;
            }
        }

        private static void StatusMenu() // 1 플레이어 상태
        {
            Console.Clear();
            ShowHighlightText("■ 상태 보기 ■"); // 제목에 첫 줄 색 변경
            Console.WriteLine("캐릭터의 정보가 표기됩니다.");

            PrintTextWithHighlights("Lv ", _player.Level.ToString("00")); // 문자열 하이라이트 "00" 은 01,02,03 이런 식으로 두 자릿수로 표현
            Console.WriteLine("");
            Console.WriteLine("{0} ({1})", _player.Name, _player.Job);

            int bonusAtk = GetSumBonusAtk(); 
            int bonusDef = GetSumBonusDef();
            PrintTextWithHighlights("공격력 : ", (_player.Atk + bonusAtk).ToString(), bonusAtk > 0 ? string.Format(" (+{0})", bonusAtk) : "");
            // 플레이어 공격력 + 보너스 공격력을 문자열로, (삼항연산자) 보너스 공격력이 0보다 크면 +(보너스 어택)을 출력해주고, 아니면은 빈칸을 추가
            PrintTextWithHighlights("방어력 : ", (_player.Def + bonusDef).ToString(), bonusDef > 0 ? string.Format(" (+{0})", bonusDef) : "");

            // 각각 PrintTextWithHighlights 함수의 s1 , s2, s3인데, Atk(공격력) 의 자료형은 Int이므로, s2의 노란색 컬러를 적용 시키기 위해 Tostring 해줌
            PrintTextWithHighlights("체력 : ", _player.Hp.ToString());
            PrintTextWithHighlights("골드 : ", _player.Gold.ToString());
            Console.WriteLine("");
            Console.WriteLine("0. 뒤로가기");
            Console.WriteLine("");

            switch (CheckValidInput(0, 0)) // 0번 나가기
            {
                case 0:
                    StartMenu();
                    break;
            }
        }
        private static int GetSumBonusAtk() // 공격력 합산 표시
        {
            int sum = 0; // 장비에 붙어있는 스탯 합산 
            for (int i = 0; i < Item.ItemCnt; i++)   // 아이템을 전부 확인
            {
                if (_items[i].IsEquipped) sum += _items[i].Atk;
                // 아이템 목록의 아이템이 장착되어 있다면, 장착 아이템의 Atk를 다 더해라.
            }
            return sum; // 그 후 sum에게 값을 반환
        }
        private static int GetSumBonusDef()  // 방어력 합산
        {
            int sum = 0;
            for (int i = 0; i < Item.ItemCnt; i++) 
            {
                if (_items[i].IsEquipped) sum += _items[i].Def;
            }
            return sum;
        }

        private static void InventoryMenu() // 2 인벤토리 
        {
            Console.Clear();
            ShowHighlightText("■ 인벤토리 ■");
            Console.WriteLine("보유중인 아이템을 관리 할 수 있습니다.");
            Console.WriteLine("");
            Console.WriteLine("[아이템 목록]");
            Console.WriteLine("");

            // 아이템 설명 출력 반복문
            for (int i = 0; i < Item.ItemCnt; i++) // 아이템의 Itemcnt 반복문, 아이템의 가짓 수 만큼 출력되어 보임
            {
                if (_items[i].IsPurchased) // 구매한 아이템 이라면 표시
                {
                    _items[i].PrintItemStatDescription(true, i + 1, false, true);
                }
            }
            Console.WriteLine("");
            Console.WriteLine("0. 나가기");
            Console.WriteLine("1. 장착관리");
            Console.WriteLine("");

            switch (CheckValidInput(0, 1))
            {
                case 0:
                    StartMenu();
                    break;
                case 1:
                    EquipMenu(); // 장착 관리
                    break;
            }
        }

        private static void EquipMenu() // 2-1 장착관리 메뉴
        {
            Console.Clear();
            ShowHighlightText("■ 인벤토리 - 장착 관리 ■");
            Console.WriteLine("보유중인 아이템을 장착/해제 할 수 있습니다.");
            Console.WriteLine("장착/해제을 위해서 장비 앞 번호를 한번더 입력해주세요");
            Console.WriteLine("");
            Console.WriteLine("[아이템 목록]");
            Console.WriteLine("");

            // 잠시 PrintItemStatDescription함수로 이동, withNumber 변수를 활용 예정
            for (int i = 0; i < Item.ItemCnt; i++)
            {
                if (_items[i].IsPurchased) // 구매한 아이템 이라면 표시
                {
                    _items[i].PrintItemStatDescription(true, i + 1, false);
                }
            }

            Console.WriteLine("");
            Console.WriteLine("0. 나가기");

            // default를 활용한 switch문 ->  모든 케이스가 아니면, 마지막에 케이스 default가 실행
            int keyInput = CheckValidInput(0, Item.ItemCnt); // 아까 만든 입력 유효성 확인 함수(0에서 아이템 숫자만큼) 인풋값에 활용
            switch (keyInput)
            {
                case 0:
                    InventoryMenu();
                    break;
                default:
                    ToggleEquipStatus(keyInput - 1); // ToggleEquipStatus는 이 아래에 만들 예정(아이템 장착 상태 변경),
                                                     // 유저 입력값은 123이며 실제 배열에는 012 이므로 -1해서 맞춰줌
                    EquipMenu();
                    break;
            }
        }

        private static void ToggleEquipStatus(int idx) // 2-2 아이템 장착 상태 변경  
        {
            // 타입 별로 하나의 아이템만 장착
            // 같은 타입의 다른 아이템이 이미 장착되어 있다면 해제
            for (int i = 0; i < Item.ItemCnt; i++)
            {
                if (i != idx && _items[i].IsEquipped && _items[i].Type == _items[idx].Type)
                {
                    _items[i].IsEquipped = false; // 기존 아이템 해제   // IsEquipped;가 true면 [E]가 나온다.
                }
            }
            // 새 아이템 장착 / 해제
            _items[idx].IsEquipped = !_items[idx].IsEquipped; // _items의 목록[idx]에 들어가서 IsEquipped이면, !(bool값을 반대로)로 장착 상태 변경

        }

        private static void StoreMenu() // 3 상점
        {
            Console.Clear();
            ShowHighlightText("■ 상 점 ■");
            Console.WriteLine("필요한 아이템을 얻을 수 있는 상점입니다.");
            Console.WriteLine("");
            Console.WriteLine("[보유 골드]");
            Console.WriteLine("");
            PrintTextWithHighlights("", _player.Gold.ToString(), " G"); 
            Console.WriteLine("");
            Console.WriteLine("[아이템 목록]");
            Console.WriteLine("");

            for (int i = 0; i < Item.ItemCnt; i++)
            {
                _items[i].PrintItemStatDescription(false, i + 1, true); // 아이템 설명 출력 메서드를 사용합니다.
            }

            Console.WriteLine("");
            Console.WriteLine("0. 나가기");
            Console.WriteLine("1. 아이템 구매");
            Console.WriteLine("2. 아이템 판매");
            Console.WriteLine("");

            switch (CheckValidInput(0, 2))
            {
                case 0:
                    StartMenu();
                    break;
                case 1:
                    BuyItemMenu();
                    break;
                case 2:
                    SellItemMenu();
                    break;
            }
        }


        private static void BuyItemMenu() // 3-3. 구매
        {
            Console.Clear();
            ShowHighlightText("■ 상 점 ■");
            Console.WriteLine("필요한 아이템을 구매 할 수 있습니다.\n");
            Console.WriteLine("\n구매하고 싶은 아이템 번호를 입력 해주세요.");
            Console.WriteLine("0을 입력하면 상점으로 돌아갑니다.");
            Console.WriteLine("");
            Console.WriteLine("[아이템 목록]");
            Console.WriteLine("");
            for (int i = 0; i < Item.ItemCnt; i++)
            {
                _items[i].PrintItemStatDescription(true, i + 1);
            }

            int choice = CheckValidInput(0, _items.Length); // 0 입력 가능
            if (choice == 0) // 0 입력 시 상점 메뉴로 복귀
            {
                StoreMenu();
                return;
            }

            choice -= 1; // 사용자 입력에서 1을 빼서 실제 인덱스로
            Item selectedItem = _items[choice];
            if (selectedItem.IsPurchased)   // 이미 구매한 아이템인지 확인
            {
                Console.WriteLine("이미 구매한 아이템입니다.");
            }
            else if (_player.Gold >= selectedItem.Gold) // 플레이어 골드가 선택 아이템 골드보다 크면 = 구매 가능한 경우
            {
                selectedItem.IsPurchased = true; // 구매 표시를 true로
                _player.Gold -= selectedItem.Gold; // 플레이어 골드 감소
                Console.WriteLine($"\n{selectedItem.Name} 구매를 완료했습니다.");
            }
            else // 골드가 부족한 경우
            {
                Console.WriteLine("Gold가 부족합니다.");
            }
            Console.WriteLine("아무 키나 누르면, 상점으로 돌아갑니다.");
            Console.ReadKey();
            StoreMenu();
        }

        private static void SellItemMenu() // 3-4. 판매
        {
            Console.Clear();
            ShowHighlightText("■ 상 점 ■");
            Console.WriteLine("필요한 아이템을 판매 할 수 있습니다.\n");
            Console.WriteLine("판매하고 싶은 아이템 번호를 입력 해주세요.");
            Console.WriteLine("\n0을 입력하면 상점으로 돌아갑니다.");
            Console.WriteLine("");
            Console.WriteLine("[아이템 목록]");
            Console.WriteLine("");
            for (int i = 0; i < _items.Length; i++)
            {
                if (_items[i].IsPurchased) // 구매한 아이템만 표시
                {
                    _items[i].PrintItemStatDescription(true, i + 1, false);
                    // PrintItemStatDescription 메서드를 호출할 때 showPrice를 false로 설정하여 가격 대신 판매가를

                    int sellPrice = (int)(_items[i].Gold * 0.85); // 실제 판매가를 계산.
                    Console.WriteLine($" - 판매가: {sellPrice} G");
                }
            }

            int choice = CheckValidInput(0, _items.Length); // 0 입력 가능
            if (choice == 0) // 0 입력 시 상점 메뉴로 복귀
            {
                StoreMenu();
                return;
            }

            choice -= 1; // 사용자 입력에서 1을 빼서 실제 인덱스로 변환
            Item selectedItem = _items[choice];
            if (selectedItem.IsPurchased)
            {
                int sellPrice = (int)(selectedItem.Gold * 0.85); // 실제 판매가를 계산
                _player.Gold += sellPrice; // 플레이어의 골드를 판매가만큼 증가
                selectedItem.IsPurchased = false; // 아이템의 구매 상태를 false로 설정
                Console.WriteLine($"{selectedItem.Name}을(를) {sellPrice}G에 판매했습니다.");
            }
            else
            {
                Console.WriteLine("판매할 수 없는 아이템입니다.");
            }

            Console.WriteLine("아무 키나 누르면, 상점으로 돌아갑니다.");
            Console.ReadKey();
            StoreMenu();
        }

        private static void ShowHighlightText(string text) // 첫 줄 색 변경 마젠타 색
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine(text);
            Console.ResetColor();
        }

        private static void PrintTextWithHighlights(string s1, string s2, string s3 = "") // 문자열 하이라이트 효과 함수 
        {
            Console.Write(s1);
            Console.ForegroundColor = ConsoleColor.Yellow; 
            Console.Write(s2);
            Console.ResetColor(); 
            Console.WriteLine(s3);
        }

        private static int CheckValidInput(int min, int max) // 0-2 입력 값 유효성 확인
        {
            // 아래 두 가지 상황은 비정상, 재입력 수행
            // 1. 숫자가 아닌 입력을 받은 경우
            // 2. 숫자가 최솟값 에서 최댓값의 범위를 벗어난 경우
            
            int keyInput; 
            bool result;  // while 뒤에는 true값이 들어가야 하기 때문에 bool 타입을 써야 한다
            do // 일단 한 번 실행
            {
                Console.WriteLine("\n원하시는 행동을 입력 해주세요.");
                result = int.TryParse(Console.ReadLine(), out keyInput);
                // 입력을 정수로 변환하여 int KeyInput에 저장하고, 결과 값을 result로 설정.
                // 결과 값이 숫자(정수)면 가져오고, 그 외면 안 가져오면서 실행되지 않음
            }
            while (result == false || CheckIfVaild(keyInput, min, max) == false); // result가 false거나 CheckIfVaild함수가 false면 반복 


            return keyInput;
        }

        private static bool CheckIfVaild(int keyInput, int min, int max) // 유효성 확인 (bool 반환)
        {
            if (min <= keyInput && keyInput <= max) return true; 
            return false; 
        }

        static void AddItem(Item item) // 3-5 아이템 추가 함수
        {                               
            if (Item.ItemCnt == 8) return; // Item클래스 객체의 ItemCnt 변수가 8이면 아무 것도 안 만든다.
            _items[Item.ItemCnt] = item;  // 0개 -> 0번 인덱스, 1개 -> 1번 인덱스
            Item.ItemCnt++;
        }

       

        private static void RestMenu() // 5 휴식하기
        {
            Console.Clear();
            ShowHighlightText("■ 휴 식 하 기 ■");
            Console.WriteLine("돈을 내고 여관에서 휴식을 하여 체력을 회복 시킬 수 있습니다.");
            Console.WriteLine("");
            Console.WriteLine($" 500 G 를 내면 체력을 회복할 수 있습니다. (보유 골드 : {_player.Gold} G)\n");
            Console.WriteLine("1. 휴식하기");
            Console.WriteLine("0. 나가기");

            string input = Console.ReadLine(); // 입력한 input은 문자열이다.
            if (input == "0") // 0번 나가기 입력 시 실행
            {
                StartMenu();
                return;
            }

            if (input == "1") // 휴식을 선택한 경우
            {
                if (_player.Hp == 100) // 이미 최대 체력인 경우
                {
                    Console.WriteLine("이미 최대 체력입니다.");
                }
                else if (_player.Gold >= 500) // 골드가 500이상이며, 최대 체력이 아닌 경우(생략)
                {
                    _player.Gold -= 500; // 골드 차감
                    _player.Hp = 100; // 체력을 최대(100)로 회복
                    Console.WriteLine($"휴식을 완료했습니다. (보유 골드 : {_player.Gold} G)");
                }
                else // 골드가 부족한 경우
                {
                    Console.WriteLine("Gold가 부족합니다.");
                }
            }
            else
            {
                Console.WriteLine("잘못된 입력입니다.");
            }

            Console.WriteLine("\n아무 키나 누르면 돌아갑니다.");
            Console.ReadKey();
            StartMenu();
        }

        private static void DungeonMenu() // 4 던전 입장
        {
            Console.Clear();
            ShowHighlightText("■ 던전 입장 ■");
            Console.WriteLine("이곳에서 던전으로 들어가기 전 다음 활동을 할 수 있습니다.");
            Console.WriteLine("");
            Console.WriteLine("1. 상태 보기");
            Console.WriteLine("2. 인벤토리");
            Console.WriteLine("3. 상점");
            Console.WriteLine("4. 던전 입장");
            Console.WriteLine("0. 메인 화면으로 나가기");
            Console.WriteLine("");

            switch (CheckValidInput(0, 4))
            {
                case 0:
                    StartMenu(); // 메인 메뉴로 나가기
                    break;
                case 1:
                    StatusMenu(); // 상태 보기
                    break;
                case 2:
                    InventoryMenu(); // 인벤토리
                    break;
                case 3:
                    StoreMenu(); // 상점
                    break;
                case 4:
                    DungeonChoiceMenu(); // 던전 난이도 선택
                    break;
            }
        }

        private static void DungeonChoiceMenu() // 4-1 던전 난이도 선택
        {
            Console.Clear();
            ShowHighlightText("■ 던전 난이도 선택 ■");
            Console.WriteLine("다양한 난이도의 던전을 선택할 수 있습니다.");
            Console.WriteLine("");
            Console.WriteLine("1. 죽음의 폐광   | 방어력 5 이상 권장");
            Console.WriteLine("2. 검은바위산    | 방어력 11 이상 권장");
            Console.WriteLine("3. 지옥불성채    | 방어력 17 이상 권장");
            Console.WriteLine("0. 던전 입구로 돌아가기");
            Console.WriteLine("");

             
                if (_player.Hp <= 0) // 체력이 0이하인 경우 입장제한
                {
                    Console.Clear();
                    Console.WriteLine("!! 체력이 부족합니다. 회복 후에 도전해주세요 !!");
                    Console.WriteLine("");
                    Console.WriteLine("0. 던전 입구로 돌아가기");
                    CheckValidInput(0, 0);
                    DungeonMenu();
                    return;
                }

            switch (CheckValidInput(0, 3))
            {
                case 0:
                    DungeonMenu(); // 던전 메뉴
                    break;
                case 1:
                    ExecuteDungeon("죽음의 폐광", 5, 1000);  
                    break;
                case 2:
                    ExecuteDungeon("검은바위산", 11, 1700);
                    break;
                case 3:
                    ExecuteDungeon("지옥불성채", 17, 2500);
                    break;
            }
        }

        private static void ExecuteDungeon(string dungeonName, int requiredDef, int reward)
        {
            Console.WriteLine($"{dungeonName}에 입장했습니다. 각오 단단히 하세요.");
            Console.WriteLine("");

            int totalDef = _player.Def + GetSumBonusDef();                    // 총 방어력 = 플레이어 방어력 + 아이템 추가 방어력
            int hpLoss = new Random().Next(20, 36);                           // 무작위 체력 감소 20 ~ 35
            int bonusReward = reward;                                         // 기본 보상 설정 + 공격력에 따른 추가 보상이 더 해질 수 있게 정의

            double perBonusReward = new Random().Next(_player.Atk, _player.Atk * 2) / 100.0;
            // 플레이어의 공격력에 따른 추가 보상의 비율을 결정.
            // 공격력의 1 ~ 2배 사이의 값을 백분율 % 로 계산

            if (totalDef < requiredDef) // 총 방어력이 권장 방어력보다 낮은 경우
            {
                if (new Random().NextDouble() < 0.4)// 랜덤 조건문 40% 확률로 실패
                {
                    Console.WriteLine("던전을 클리어하지 못했습니다.. 체력이 반 감소합니다.");
                    _player.Hp = Math.Max(_player.Hp - hpLoss / 2, 0); // 실패시 체력이 절반으로 감소, 0보다는 낮아지지 않음.
                    _player.Hp = Math.Min(_player.Hp, 100);
                }
                else // 권장 방어력 이상이거나, 권장 방어력 이하이지만 던전 실패가 아닌 경우
                {
                    Console.WriteLine("던전을 클리어했습니다! 축하 드립니다.");

                    bonusReward += (int)(reward * perBonusReward);

                    _player.Hp = Math.Max(_player.Hp - hpLoss, 0); // 플레이어 체력 - 손실 체력,  0보다는 낮아지지 않음.
                    _player.Hp = Math.Min(_player.Hp, 100);
                    _player.Gold += bonusReward; // 플레이어 골드 + 보너스 골드
                }
            }
            else // 권장 방어력 이상인 경우
            {
                Console.WriteLine("던전을 클리어했습니다!");
                hpLoss -= totalDef - requiredDef; // 체력손실량 = 방어력에 따라 체력 감소량 조정  

                bonusReward += (int)(reward * perBonusReward); // 위와 동일

                _player.Hp = Math.Max(_player.Hp - (hpLoss > 0 ? hpLoss : 0), 0);

                _player.Hp = Math.Min(_player.Hp, 100);

                _player.Gold += bonusReward; // 플레이어의 골드에 최종 보상 더하기
            }

            Console.WriteLine("\n[탐험 결과]");

            Console.WriteLine($"체력 {_player.Hp + hpLoss} -> {_player.Hp}");
            // {_player.Hp + hpLoss} = 던전 전 체력

            Console.WriteLine($"Gold + {reward} G -> {_player.Gold} G\n");
            // 얼마가 추가되었는지 알 수 있 도록 {reward} 표시

            Console.WriteLine("0. 나가기");

            CheckValidInput(0, 0);
            StartMenu();
        }

        private static void GameOverMenu() // 6 게임 종료
        {
            Console.Clear();
            ShowHighlightText("■ 정말 종료하시겠습니까? 진짜로? ■");
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("1. 네, 종료합니다.\n");
            Console.WriteLine("0. 아니요? 제가요? 왜요?");

            switch (CheckValidInput(0, 1))
            {
                case 1: // 게임 종료
                    Console.WriteLine("아무 키나 누르면, 게임을 종료합니다.");
                    Console.ReadKey();
                    Environment.Exit(0); // 애플리케이션 종료
                    break;
                case 0: // 메인 메뉴로 돌아가기
                    StartMenu();
                    break;
            }
        }

       
        }
    }
