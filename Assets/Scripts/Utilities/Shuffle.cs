using System.Collections;
using System.Collections.Generic;
using System;


public static class ShuffleExtension
{
    private static Random rng = new Random();

    //Shuffle any (I)List with an extension method based on the Fisher-Yates shuffle
    //http://en.wikipedia.org/wiki/Fisher-Yates_shuffle
    public static void Shuffle<T>(this IList<T> list)
    {

        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }

}

