using McProtoNet.NBT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace McProtoNet.Protocol754.Data.Window
{  
    
    public class ItemStack
    {
        public int Id { get; private set; }
        public sbyte Amount { get; private set; }
        public NbtTag? Nbt { get; private set; }

        public ItemStack(int id, sbyte amount, NbtTag? nbt)
        {            
            Id = id;
            Amount = amount;
            Nbt = nbt;
        }
    }
}
