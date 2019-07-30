using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardFill : MonoBehaviour
{
    public int[,] map; /* = new int[,] {
    {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
    {0,0,0,0,1,1,1,1,1,0,1,1,1,1,0,0},
    {0,0,0,0,1,0,0,0,1,0,0,1,0,1,0,0},
    {0,1,1,1,1,0,0,0,1,1,1,0,0,1,0,0},
    {0,0,0,0,1,1,1,1,0,0,0,0,0,0,1,0},
    {0,0,0,0,0,0,0,1,0,0,0,0,0,1,0,0},
    {0,0,1,1,1,1,1,1,0,0,0,0,0,1,0,0},
    {0,0,1,1,1,0,1,1,0,0,1,1,0,1,0,0},
    {0,0,0,0,1,0,0,0,0,1,1,0,0,1,0,0},
    {0,0,0,0,1,1,1,1,1,1,0,1,1,1,0,0},
    {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0}
};
*/
    void borderfill(int minx, int miny, int maxx, int maxy, int borderflag)
    {

        int outflag = -1; //will fill outside grids with -1
        minx--;
        miny--;
        maxx++;
        maxy++;
        CPoint seed = new CPoint(minx, miny);
        CPoint[] queue = new CPoint[(maxx - minx + 1) * (maxy - miny + 1)];
        int head = 0;
        int tail = 0;
        queue[tail++] = seed;
        int[] dir = new int[] { -1, 1 };
        while (head < tail)
        { //BFS Search
            CPoint cur = queue[head++];

            for (int i = 0; i < 2; i++)
            { //up and down
                int checky = cur.y + dir[i];
                if (checky >= miny && checky <= maxy)
                {
                    int val = map[checky, cur.x];
                    if (val != borderflag && val != outflag)
                    {
                        CPoint tmp = new CPoint(cur.x, checky);
                        queue[tail++] = tmp;
                        map[checky, cur.x] = outflag;
                    }
                }
            }
            for (int i = 0; i < 2; i++)
            { //left and right
                int checkx = cur.x + dir[i];
                if (checkx >= minx && checkx <= maxx)
                {
                    int val = map[cur.y, checkx];
                    if (val != borderflag && val != outflag)
                    {
                        CPoint tmp = new CPoint(checkx, cur.y);
                        queue[tail++] = tmp;
                        map[cur.y, checkx] = outflag;
                    }
                }
            }
        }
        for (int i = minx; i <= maxx; i++)
        { //反向填充
            for (int j = miny; j <= maxy; j++)
            {
                if (map[j, i] == outflag)
                {
                    map[j, i] = 0;
                }
                else
                {
                    map[j, i] = borderflag;
                }
            }
        }
    }
    public int MAX_X = 14;
    public int MAX_Y = 9;
    public static BoardFill instance;
    void Awake()
    {
        instance = this;
      //  PrintArray(map, MAX_Y + 2, MAX_X + 2, "raw ");
      //  borderfill(1, 1, MAX_X, MAX_Y, 1); //minx, miny, maxx, maxy为待填充区域的最小覆盖矩形
      //  PrintArray(map, MAX_Y + 2, MAX_X + 2, "afterfill ");
    }
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
            FillBord();
    }
    public void FillBord()
    {
         PrintArray(map, MAX_Y + 2, MAX_X + 2, "raw ");
         borderfill(1, 1, MAX_X, MAX_Y, 1); //minx, miny, maxx, maxy为待填充区域的最小覆盖矩形
         PrintArray(map, MAX_Y + 2, MAX_X + 2, "afterfill ");
    }
    void PrintArray(int[,] a, int maxrow, int maxcol, string sTitle)
    {
        print("---%s--start--\n" + sTitle);
        int i, j;
        string s = "";
        for (i = 0; i < maxrow; i++)
        {

            for (j = 0; j < maxcol; j++)
            {
                s += "" + a[i, j];
            }
            s += "\n";
        }
        print(s);

    }
    public struct CPoint
    {

        public int x;
        public int y;

        public CPoint(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
    }

}
