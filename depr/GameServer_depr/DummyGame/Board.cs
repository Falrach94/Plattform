using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DummyGame
{
    public class Board
    {
        private readonly int[,] _map;
        private readonly int _size;

        public Board(int size)
        {
            _size = size;
            _map = new int[size, size];
            
            for(int x = 0; x < size; x++)
            {
                for (int y = 0; y < size; y++)
                {
                    _map[y, x] = -1;
                }
            }

        }

        public void SetField(int x, int y, int owner)
        {
            if(x < 0 || y < 0 || x >= _size || y >= _size)
            {
                throw new ArgumentException("Coordinates out of range!");
            }
            if(_map[y, x] != -1)
            {
                throw new ArgumentException("Field already occupied!");
            }
            _map[y, x] = owner;
        }
   
        public int TestWinner()
        {
            int d = _map[0, 0];
            if (d != -1)
            {
                for (int i = 1; i < _size; i++)
                {
                    if (_map[i, i] != d)
                    {
                        d = -1;
                        break;
                    }
                }
            }
            if (d != -1)
            {
                return d;
            }
            d = _map[0, _size-1];
            if (d != -1)
            {
                for (int i = 1; i < _size; i++)
                {
                    if (_map[i, _size-1-i] != d)
                    {
                        d = -1;
                        break;
                    }
                }
            }
            if (d != -1)
            {
                return d;
            }

            for (int i = 0; i < _size; i++)
            {
                int v = _map[i, 0];
                if (v != -1)
                {
                    for (int x = 1; x < _size; x++)
                    {
                        if(_map[i, x] != v)
                        {
                            v = -1;
                            break;
                        }
                    }
                    if(v != -1)
                    {
                        return v;
                    }
                }
                v = _map[0, i];
                if (v != -1)
                {
                    for (int y = 0; y < _size; y++)
                    {
                        if (_map[y, i] != v)
                        {
                            v = -1;
                            break;
                        }
                    }
                    if (v != -1)
                    {
                        return v;
                    }
                }
            }
            return -1;
        }
    }
}
