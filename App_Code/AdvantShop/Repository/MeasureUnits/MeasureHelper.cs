using System;
using System.Collections.Generic;
using System.Linq;

namespace AdvantShop.Repository
{
    public class MeasureHelper
    {
        public static int[] GetDimensions(List<string[]> dimensions, int heightAvg, int widthAvg, int lengthAvg)
        {
            var result = new int[3];
            var dims = new List<int[]>();

            foreach (var dimension in dimensions)
            {
                var arr = new int[3];

                arr[0] = dimension.Length >0 ? (int)Math.Ceiling(dimension[0].TryParseFloat()):0;
                arr[1] = dimension.Length >1 ? (int)Math.Ceiling(dimension[1].TryParseFloat()):0;
                arr[2] = dimension.Length >2 ? (int)Math.Ceiling(dimension[2].TryParseFloat()):0;

                if (arr[0] == 0)
                    arr[0] = lengthAvg;

                if (arr[1] == 0)
                    arr[1] = widthAvg;

                if (arr[2] == 0)
                    arr[2] = heightAvg;

                dims.Add(arr.OrderByDescending(d => d).ToArray());
            }

            foreach (var dim in dims)
            {
                if (dim[0] >= result[0])
                    result[0] = dim[0];

                if (dim[1] >= result[1])
                    result[1] = dim[1];

                result[2] += dim[2];
            }

            return result;
        }

    }
}