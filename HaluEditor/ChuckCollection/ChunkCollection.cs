using System;

namespace ngprojects.HaluEditor.ChuckCollection
{
    public class ChunkCollection<T>
    {
        private T[][] chunks = new T[0][];

        private int chunkSize = 25;
        private int totalLength = 0;

        public ChunkCollection()
        {
        }

        public void Add(int len, T bsp)
        {
            T[] de = new T[len];

            int actOffset = 0;
            for (int i = 0; i < chunks.Length; i++)
            {
                int Laenge = chunks[i].Length;
                Array.Copy(chunks[i], 0, de, actOffset, Laenge);
                actOffset += Laenge;
            }

            int newChunksLen = (int)Math.Ceiling((double)de.Length / chunkSize);
            T[][] newChunks = new T[newChunksLen][];
            int offs = 0;
            for (int i = 0; i < newChunks.Length; i++)
            {
                int valLen = i == newChunksLen - 1 ? de.Length % chunkSize : chunkSize;
                T[] Chunk = new T[valLen];

                Array.Copy(de, offs, Chunk, 0, valLen);
                offs += valLen;
                newChunks[i] = Chunk;
                totalLength++;
            }
            chunks = newChunks;
        }

        public void Add(T obj)
        {
            int Len = chunks.Length;
            if (totalLength % chunkSize == 0 || chunks.Length == 0) // Neuer Chunk muss erstellt werden
            {
                Array.Resize(ref chunks, chunks.Length + 1);
                chunks[chunks.Length - 1] = new T[] { obj };
                totalLength++;
            }
            else
            {
                var actLen = chunks[Len - 1].Length;
                T[] _tmpArr = new T[actLen + 1];
                Array.Copy(chunks[Len - 1], 0, _tmpArr, 0, actLen);
                _tmpArr[actLen] = obj;
                chunks[Len - 1] = _tmpArr;
                totalLength++;
            }
        }

        public void DefragChunks()
        {
            int totalLen = 0;
            for (int i = 0; i < chunks.Length; i++)
            {
                totalLen += chunks[i].Length;
            }
            T[] PositionBeforeSplit = new T[totalLen];
            int actOffset = 0;
            for (int i = 0; i < chunks.Length; i++)
            {
                int Laenge = chunks[i].Length;
                Array.Copy(chunks[i], 0, PositionBeforeSplit, actOffset, Laenge);
                actOffset += Laenge;
            }

            int newChunksLen = (int)Math.Ceiling((double)PositionBeforeSplit.Length / chunkSize);
            T[][] newChunks = new T[newChunksLen][];
            int offs = 0;
            for (int i = 0; i < newChunks.Length; i++)
            {
                int valLen = i == newChunksLen - 1 ? PositionBeforeSplit.Length % chunkSize : chunkSize;
                T[] Chunk = new T[valLen];

                Array.Copy(PositionBeforeSplit, offs, Chunk, 0, valLen);
                offs += valLen;
                newChunks[i] = Chunk;
            }
            chunks = newChunks;
        }

        public T[] GetList(int Start, int End)
        {
            int arrayStart = (Start - (Start % chunkSize) / Start);
            int arrayEnd = (End - (End % chunkSize) / End);

            int totalLen = 0;
            for (int i = 0; i < chunks.Length; i++)
            {
                totalLen += chunks[i].Length;
            }
            int actOffset = 0;
            T[] PositionBeforeSplit = new T[totalLen];
            for (int i = 0; i < chunks.Length; i++)
            {
                int Laenge = chunks[i].Length;
                Array.Copy(chunks[i], 0, PositionBeforeSplit, actOffset, Laenge);
                actOffset += Laenge;
            }
            return PositionBeforeSplit;
        }

        public void Insert(T obj, int index)
        {
            int ChunkPosition = index / chunkSize;
            int PositionInChunk = index % chunkSize;
            int ChunkSpaceAvailable = chunkSize - chunks[ChunkPosition].Length;

            if (ChunkSpaceAvailable >= 1) // Es muss kein neuer Chunk hinzugefügt werden.
            {
                T[] _SourceChunk = chunks[ChunkPosition];
                T[] _tmp1 = new T[1];
                T[] _tmp2 = new T[1];
                //Split(_SourceChunk, ref _tmp1, ref _tmp2, index);
            }
        }

        public void Remove(int index)
        {
            int pos = index % chunkSize;
            int ChunckPos = (index - pos) % chunkSize;
            T[] newArr = new T[chunks[ChunckPos].Length - 1];
            Array.Copy(chunks[ChunckPos], 0, newArr, 0, pos);
            Array.Copy(chunks[ChunckPos], pos, newArr, pos - 1, (newArr.Length - (pos - 1)) - 1);
            chunks[ChunckPos] = newArr;
            totalLength--;
            DefragChunks();
        }
    }
}