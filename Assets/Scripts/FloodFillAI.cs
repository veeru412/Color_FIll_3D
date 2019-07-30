using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ColorFill.Core
{
    public static class FloodFillAI
    {
        public static void FloodFill(Square s, bool border)
        {
            if (s._color == 0 && border)
                return;
            if (s._color == 0 || s._color == 1)
            {
                bool _border = s._color == 1;

                s.SetPixel(2);


                Square rightNeighb = GameManager.instance.GetBlock(s.row, s.col + 1);
                if (rightNeighb != null)
                    FloodFill(rightNeighb, _border);

                Square leftNeighb = GameManager.instance.GetBlock(s.row, s.col - 1);
                if (leftNeighb != null)
                    FloodFill(leftNeighb, _border);

                Square frontNeighb = GameManager.instance.GetBlock(s.row - 1, s.col);
                if (frontNeighb != null)
                    FloodFill(frontNeighb, _border);

                Square backNeighb = GameManager.instance.GetBlock(s.row + 1, s.col);
                if (backNeighb != null)
                    FloodFill(backNeighb, _border);

            }
        }

    }
}

