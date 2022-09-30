using McProtoNet.Protocol340.Util;
using Org.BouncyCastle.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Org.BouncyCastle.Math.Primes;

namespace McProtoNet.Protocol340.Data.World.Chunk
{
    public class BlockStorage
    {
        private static readonly BlockState AIR = new BlockState(0, 0);

        private int bitsPerEntry;

        private List<BlockState> states;
        private FlexibleStorage storage;

        public BlockStorage()
        {
            this.bitsPerEntry = 4;

            this.states = new List<BlockState>();
            this.states.Add(AIR);

            this.storage = new FlexibleStorage(this.bitsPerEntry, 4096);
        }
        public BlockStorage(IMinecraftPrimitiveReader reader)
        {
            this.bitsPerEntry = reader.ReadUnsignedByte();

            this.states = new List<BlockState>();
            int stateCount = reader.ReadVarInt();
            for (int i = 0; i < stateCount; i++)
            {
                this.states.Add(reader.ReadBlockState());
            }

            this.storage = new FlexibleStorage(this.bitsPerEntry, reader.ReadLongArray());
        }
        /*
        public BlockStorage(NetInput in) throws IOException
        {
        this.bitsPerEntry = in.readUnsignedByte();

        this.states = new ArrayList<BlockState>();
        int stateCount = in.readVarInt();
        for(int i = 0; i<stateCount; i++) {
            this.states.add(NetUtil.readBlockState(in));
        }

        this.storage = new FlexibleStorage(this.bitsPerEntry, in.readLongs(in.readVarInt()));
    }
        */



        private static int index(int x, int y, int z)
        {
            return y << 8 | z << 4 | x;
        }

        private static BlockState rawToState(int raw)
        {
            return new BlockState(raw >> 4, raw & 0xF);
        }

        private static int stateToRaw(BlockState state)
        {
            return (state.Id << 4) | (state.Data & 0xF);
        }
        /*
        public void write(NetOutput out) throws IOException
        {
            out.writeByte(this.bitsPerEntry);

            out.writeVarInt(this.states.size());
            for(BlockState state : this.states) {
                NetUtil.writeBlockState(out, state);
            }

            long[]
            data = this.storage.getData();
            out.writeVarInt(data.length);
            out.writeLongs(data);
        }
        */

        public int getBitsPerEntry()
        {
            return this.bitsPerEntry;
        }

        public List<BlockState> getStates()
        {
            return new List<BlockState>(states);
        }

        public FlexibleStorage getStorage()
        {
            return this.storage;
        }

        public BlockState this[int x, int y, int z]
        {
            get
            {
                int id = this.storage[index(x, y, z)];
                return this.bitsPerEntry <= 8 ? (id >= 0 && id < this.states.Count ? this.states[id] : AIR) : rawToState(id);
            }
            set
            {
                BlockState state = value;
                int id = this.bitsPerEntry <= 8 ? this.states.IndexOf(state) : stateToRaw(state);
                if (id == -1)
                {
                    this.states.Add(state);
                    if (this.states.Count > 1 << this.bitsPerEntry)
                    {
                        this.bitsPerEntry++;

                        List<BlockState> oldStates = this.states;
                        if (this.bitsPerEntry > 8)
                        {
                            oldStates = new List<BlockState>(this.states);
                            this.states.Clear();
                            this.bitsPerEntry = 13;
                        }

                        FlexibleStorage oldStorage = this.storage;
                        this.storage = new FlexibleStorage(this.bitsPerEntry, this.storage.Size);
                        for (int index = 0; index < this.storage.Size; index++)
                        {
                            int newValue = this.bitsPerEntry <= 8 ? oldStorage[index] : stateToRaw(oldStates[index]);
                            this.storage[index] = newValue;
                        }
                    }

                    id = this.bitsPerEntry <= 8 ? this.states.IndexOf(state) : stateToRaw(state);
                }

                this.storage[index(x, y, z)] = id;
            }
        }




        public bool IsEmpty
        {
            get
            {
                for (int index = 0; index < this.storage.Size; index++)
                {
                    if (this.storage[index] != 0)
                    {
                        return false;
                    }
                }

                return true;
            }
        }

    }
}
