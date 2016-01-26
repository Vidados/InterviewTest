using System;
using System.Collections.Generic;


namespace InterviewTest.Models
{
    public class ItemCollection : System.Collections.CollectionBase
    {
        public void add(Items anItem)
        {
            List.Add(anItem);
        }
        public Items getitem(int Index)
        {
            return (Items)List[Index];
        }
    }

}