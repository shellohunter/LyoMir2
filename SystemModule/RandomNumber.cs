using System;
using System.Collections.Generic;

namespace SystemModule
{
    
    
    
    public class RandomNumber
    {
        private static Random random = null;

        
        private static RandomNumber singleton;
        
        
        private static readonly object syncObject = new object();

        private RandomNumber() { }

        public static RandomNumber GetInstance()
        {
            if (singleton == null)
            {
                lock (syncObject)
                {
                    if (singleton == null)
                    {
                        random = new Random();
                        singleton = new RandomNumber();
                    }
                }
            }
            return singleton;
        }

        
        
        
        
        
        
        public IList<int> RandomSelect(IList<int> sourceList, int selectCount)
        {
            if (selectCount > sourceList.Count)
                throw new ArgumentOutOfRangeException("selectCount必需大于sourceList.Count");
            IList<int> resultList = new List<int>();
            for (int i = 0; i < selectCount; i++)
            {
                int nextIndex = GetRandomNumber(1, sourceList.Count);
                int nextNumber = sourceList[nextIndex - 1];
                sourceList.RemoveAt(nextIndex - 1);
                resultList.Add(nextNumber);
            }
            return resultList;
        }

        
        
        
        
        
        
        public int GetRandomNumber(int minValue, int maxValue)
        {
            return random.Next(minValue, maxValue + 1);
        }

        
        
        
        
        
        public int Random()
        {
            return random.Next();
        }

        
        
        
        
        
        public int Random(int Value)
        {
            return random.Next(Value);
        }

        
        
        
        
        
        public int Random(int minValue, int maxValue)
        {
            return random.Next(minValue, maxValue);
        }
    }
}