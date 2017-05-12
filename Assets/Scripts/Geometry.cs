using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Geometry
{

    public static List<Vector2> FindSquaresInCircle(Vector2 origin, int radius)
    {
        List<Vector2> coveredSquares = new List<Vector2>();
        for (int x = (int)origin.x - radius; x <= (int)origin.x + radius; x++)
        {
            for (int y = (int)origin.y - radius; y <= (int)origin.y + radius; y++)
            {
                Vector2 checkSquare = new Vector2(x, y);
                if (Vector2.Distance(checkSquare, origin) <= radius + 0.9f)
                {
                    coveredSquares.Add(checkSquare);
                }
            }
        }
        return coveredSquares;
    }

    public static List<Vector2> FindSquaresOnCircle(Vector2 origin, int radius, bool chunky = false)
    {
        int x0 = (int)origin.x;
        int y0 = (int)origin.y;
        int x = radius;
        int y = 0;
        int err = 0;
        List<Vector2> coveredSquares = new List<Vector2>();

        while (x >= y)
        {

            coveredSquares.Add(new Vector2(x0 + x, y0 + y));
            coveredSquares.Add(new Vector2(x0 + y, y0 + x));
            coveredSquares.Add(new Vector2(x0 - y, y0 + x));
            coveredSquares.Add(new Vector2(x0 - x, y0 + y));
            coveredSquares.Add(new Vector2(x0 - x, y0 - y));
            coveredSquares.Add(new Vector2(x0 - y, y0 - x));
            coveredSquares.Add(new Vector2(x0 + y, y0 - x));
            coveredSquares.Add(new Vector2(x0 + x, y0 - y));

            if (err <= 0)
            {
                y += 1;
                err += 2 * y + 1;
            }
            else if (err > 0 && chunky)
            {
                x -= 1;
                err -= 2 * x + 1;
            }
            if (err > 0 && !chunky)
            {
                x -= 1;
                err -= 2 * x + 1;
            }
        }

        return coveredSquares;
    }

    //TODO: Optimize by determining Array size before running
    public static List<Vector2> DrawLine(Vector2 origin, Vector2 target)
    {
        int y0 = (int)origin.y;
        int y1 = (int)target.y;
        int x0 = (int)origin.x;
        int x1 = (int)target.x;
        List<Vector2> pts = new List<Vector2>();

        bool swapXY = Mathf.Abs(y1 - y0) > Mathf.Abs(x1 - x0);
        int tmp;
        if (swapXY)
        {
            // swap x and y
            tmp = x0; x0 = y0; y0 = tmp; // swap x0 and y0
            tmp = x1; x1 = y1; y1 = tmp; // swap x1 and y1
        }

        if (x0 > x1)
        {
            // make sure x0 < x1
            tmp = x0; x0 = x1; x1 = tmp; // swap x0 and x1
            tmp = y0; y0 = y1; y1 = tmp; // swap y0 and y1
        }

        int deltax = x1 - x0;
        int deltay = (int)Mathf.Floor(Mathf.Abs(y1 - y0));
        var error = Mathf.Floor(deltax / 2);
        var y = y0;
        var ystep = y0 < y1 ? 1 : -1;
        if (swapXY)
            // Y / X
            for (int x = x0; x < x1 + 1; x++)
            {
                pts.Add(new Vector2(y, x));
                error -= deltay;
                if (error < 0)
                {
                    y = y + ystep;
                    error = error + deltax;
                }
            }
        else
            // X / Y
            for (int x = x0; x < x1 + 1; x++)
            {
                pts.Add(new Vector2(x, y));
                error -= deltay;
                if (error < 0)
                {
                    y = y + ystep;
                    error = error + deltax;
                }
            }
        //Force pts to always be ordered from origin to target
        if (pts[0] != origin)
            pts.Reverse();
        return pts;
    }
}
