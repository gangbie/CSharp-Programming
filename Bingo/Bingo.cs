using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bingo
{
    internal class Bingo
    {
        /* [게임 규칙]
		 * 1.	5x5 판을 생성하고 랜덤한 숫자를 배치한다.
		 * 2.	원하는 숫자 입력시 해당 숫자는 특수기호로 바꾼다.
		 * 3.	생성한 숫자외의 다른 수를 입력할 수 없다.(예외처리)
		 * 4.	종료 조건은 빙고 3줄 이상시 종료한다.
		*/

        const int LEN = 5;
        const int MAX = LEN * LEN;

        public void DoPlay()
        {
            int[] bingoData = new int[LEN * LEN];
            string[,] map = SetMap(bingoData);
            int bingoLineCnt = 0;

            while (true)
            {
                PrintMap(map, bingoLineCnt);

                int inputNum = GetInputNum();
                while (IsUseableNum(inputNum) == false)
                {
                    int top = Console.CursorTop;

                    Console.Write("다시 0~24 범위내에 숫자만 ");
                    inputNum = GetInputNum();
                }
                CheckedBingo(map, bingoData, ref bingoLineCnt, inputNum);

                if (IsFinishedGame(bingoLineCnt))
                {
                    PrintMap(map, bingoLineCnt);
                    Console.WriteLine("★B★I★N★G★O★");
                    break;
                }
            }

        }

        /// <summary>
        /// 사용자 입력 값이 유효한지 확인한다.
        /// 유효한 값 : 0 ~ 24까지의 숫자
        /// </summary>
        /// <param name="inputNum"></param>
        /// <returns></returns>
        private bool IsUseableNum(int inputNum)
        {
            return !(inputNum < 0 || inputNum >= MAX);
        }

        /// <summary>
        /// 맵을 세팅한다.
        /// </summary>
        /// <param name="bingoData">맵에 들어갈 빙고 숫자 반환용</param>
        /// <returns></returns>
        private string[,] SetMap(int[] bingoData)
        {
            int lastIndex = MAX - 1;

            for (int i = 1; i < MAX; i++)
            {
                int index = GetRandomNum(0, lastIndex);

                while (bingoData[index] != 0)
                {
                    index = GetRandomNum(0, lastIndex);
                }
                bingoData[index] = i;
            }

            int k = 0;
            string[,] map = new string[LEN, LEN];
            for (int i = 0; i < LEN; i++)
            {
                for (int j = 0; j < LEN; j++)
                {
                    map[i, j] = bingoData[k++].ToString();
                }
            }
            return map;
        }

        /// <summary>
        /// 맵에 들어갈 숫자를 랜덤하게 가져온다
        /// </summary>
        /// <param name="min">최솟값</param>
        /// <param name="max">최댓값</param>
        /// <returns></returns>
        private int GetRandomNum(int min, int max)
        {
            Random randomObj = new Random();
            return randomObj.Next(min, max);
        }

        /// <summary>
        /// 맵을 출력한다.
        /// </summary>
        /// <param name="map">맵</param>
        /// <param name="bingoLineCnt">빙고 라인 수</param>
        private void PrintMap(string[,] map, int bingoLineCnt)
        {
            Console.Clear();

            Console.WriteLine("==========|빙고|==========");
            Console.WriteLine($"{bingoLineCnt}개 빙고!! \n\n");

            for (int i = 0; i < LEN; i++)
            {
                for (int j = 0; j < LEN; j++)
                {
                    Console.Write(string.Format("{0}\t", map[i, j]));
                }
                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine();
            }
        }

        /// <summary>
        /// 빙고가 되었는지 확인한다.
        /// </summary>
        /// <param name="map">맵</param>
        /// <param name="bingoData">맵에 세팅된 숫자 정보</param>
        /// <param name="bingoLineCnt"></param>
        /// <param name="inputNum"></param>
        private void CheckedBingo(string[,] map, int[] bingoData, ref int bingoLineCnt, int inputNum)
        {
            int index = Array.IndexOf(bingoData, inputNum);

            if (index < 0) return;

            int i = index / 5;
            int j = index - (i * 5);
            map[i, j] = "#";
            bingoData[index] = MAX;

            bingoLineCnt = CheckedBingoLine(map, bingoData);
        }

        /// <summary>
        /// 빙고 한 줄이 성립 되었는지 확인한다.
        /// </summary>
        /// <param name="map">맵</param>
        /// <param name="bingoData">맵에 세팅된 숫자 정보</param>
        /// <returns></returns>
        private int CheckedBingoLine(string[,] map, int[] bingoData)
        {
            int cnt = 0;

            StringBuilder chkHorizontal = new StringBuilder();
            StringBuilder chkVertical = new StringBuilder();

            for (int i = 0; i < LEN; i++)
            {
                for (int j = 0; j < LEN; j++)
                {
                    chkHorizontal.Append(map[i, j]);
                    chkVertical.Append(map[j, i]);
                }
                cnt += chkHorizontal.ToString() == "#####" ? 1 : 0;
                cnt += chkVertical.ToString() == "#####" ? 1 : 0;

                chkHorizontal.Clear();
                chkVertical.Clear();
            }

            StringBuilder DiagonalL = new StringBuilder();
            StringBuilder DiagonalR = new StringBuilder();

            for (int i = 0; i < LEN;)
            {
                DiagonalL.Append(map[i, i]);
                DiagonalR.Append(map[i, LEN - (++i)]);
            }
            cnt += DiagonalL.ToString() == "#####" ? 1 : 0;
            cnt += DiagonalR.ToString() == "#####" ? 1 : 0;

            return cnt;
        }

        /// <summary>
        /// 입력 값을 받아온다.
        /// </summary>
        /// <returns></returns>
        private int GetInputNum()
        {
            Console.Write("숫자 입력 : ");
            string inputNum = Console.ReadLine();

            int num = -1;
            if (int.TryParse(inputNum, out num) == false)
            {
                return -1;
            }

            return num;
        }

        /// <summary>
        /// 게임이 종료되었는지 확인한다
        /// </summary>
        /// <param name="bingoLineCnt">빙고 라인 수</param>
        /// <returns></returns>
        private bool IsFinishedGame(int bingoLineCnt)
        {
            return (bingoLineCnt == 3);
        }
    }
}
