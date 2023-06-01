using System;
using System.Collections;

namespace DaisyForum.BackendServer.Helpers
{
    public class Content_Based
    {
        public static double CosineSimilarity(BitArray vector1, BitArray vector2)
        {
            // Tính tích vô hướng của hai vector
            int dotProduct = 0;
            for (int i = 0; i < vector1.Length; i++)
            {
                if (vector1[i] && vector2[i])
                {
                    dotProduct++;
                }
            }

            // Tính độ dài của từng vector
            double vector1Length = Math.Sqrt(vector1.Cast<bool>().Count(b => b));
            double vector2Length = Math.Sqrt(vector2.Cast<bool>().Count(b => b));

            // Tính độ tương đồng Cosine Similarity
            double cosineSimilarity = dotProduct / (vector1Length * vector2Length);

            return cosineSimilarity;
        }
    }
}