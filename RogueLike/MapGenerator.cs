using Meadow.Update;

namespace RogueLike;

public enum TileType : byte
{
    Blank,
    Wall,
    Room,
    Path
}

public enum Direction : byte
{
    Left,
    Right,
    Up, 
    Down,
}

internal class MapGenerator
{
    public int Width { get; set; } = 40;
    public int Height { get; set; } = 30;

    public int MaxRooms { get; set; } = 8;

    public int RoomMinDimension { get; set; } = 6;
    public int RoomMaxDimension { get; set; } = 12;

    public TileType[,]? MapTiles { get; set; }

    public List<Room> Rooms { get; protected set; }

    Random rand = new ((int)DateTime.UtcNow.Ticks);

    
    public MapGenerator()
    {
        Rooms = new List<Room>();
        GenerateMap();
    }

    Room? GenerateRoom(byte roomId)
    {
        //find a room width and height
        int w = rand.Next(RoomMaxDimension - RoomMinDimension) + RoomMinDimension;
        int h = rand.Next(RoomMaxDimension - RoomMinDimension) + RoomMinDimension;

        //find a random x,y coordinate ... stay off the edges
        int x = rand.Next(Width - w - 2) + 1;
        int y = rand.Next(Height - h - 2) + 1;

        for (int i = 0; i < w; i++)
        {
            for (int j = 0; j < h; j++)
            {
                if (MapTiles[x + i, y + j] != TileType.Blank ||
                    MapTiles[x + i + 1, y + j] != TileType.Blank ||
                    MapTiles[x + i - 1, y + j] != TileType.Blank ||
                    MapTiles[x + i, y + j + 1] != TileType.Blank ||
                    MapTiles[x + i, y + j - 1] != TileType.Blank ||
                    MapTiles[x + i + 1, y + j + 1] != TileType.Blank ||
                    MapTiles[x + i + 1, y + j - 1] != TileType.Blank ||
                    MapTiles[x + i - 1, y + j + 1] != TileType.Blank ||
                    MapTiles[x + i - 1, y + j - 1] != TileType.Blank)
                {
                    return null;
                }
            }
        }

        return new Room()
        {
            ID = roomId,
            Width = w,
            Height = h,
            Left = x,
            Top = y,
        };
    }

    void UpdateMap(Room room)
    {
        for (int i = 0; i < room.Width; i++)
        {
            for (int j = 0; j < room.Height; j++)
            {
                MapTiles[i + room.Left, j + room.Top] = TileType.Room;
            }
        }
    }

    void AddPaths(Room r)
    {
        bool westPath = false;
        bool eastPath = false;
        bool northPath = false;
        bool southPath = false;

        for (int i = 0; i < r.Width; i++)
        {
            for (int j = 0; j < r.Height; j++)
            {
                if(i == 0)
                {
                    MapTiles[r.Left + i, r.Top + j] = TileType.Wall;
                    if(westPath == false)
                    {
                        for (int k = r.Left - 1; k > 0; k--)
                        {
                            if(MapTiles[k, r.Top + j] == TileType.Wall)
                            {
                                //found a wall .... fill in the path
                                for (int p = r.Left - 1; p > k; p--)
                                {
                                    MapTiles[p, r.Top + j] = TileType.Path;
                                }
                                westPath = true;
                                break;
                            }
                        }
                    }
                }
                else if (i == r.Width - 1)
                {
                    MapTiles[r.Left + i, r.Top + j] = TileType.Wall;
                    if (eastPath == false)
                    {
                        for (int k = r.Left + i + 1; k < Width; k++)
                        {
                            if (MapTiles[k, r.Top + j] == TileType.Wall)
                            {
                                //found a wall .... fill in the path
                                for (int p = r.Left + i + 1; p < k; p++)
                                {
                                    MapTiles[p, r.Top + j] = TileType.Path;
                                }
                                eastPath = true;
                                break;
                            }
                        }
                    }
                }
                else if (j == 0)
                {
                    MapTiles[r.Left + i, r.Top + j] = TileType.Wall;
                    if (northPath == false)
                    {
                        for (int k = r.Top - 1; k > 0; k--)
                        {
                            if (MapTiles[r.Left + i, k] == TileType.Wall)
                            {
                                //found a wall .... fill in the path
                                for (int p = r.Top - 1; p > k; p--)
                                {
                                    MapTiles[r.Left + i, p] = TileType.Path;
                                }
                                northPath = true;
                                break;
                            }
                        }
                    }
                }
                else if (j == r.Height - 1)
                {
                    MapTiles[r.Left + i, r.Top + j] = TileType.Wall;
                    if (southPath == false)
                    {
                        for (int k = r.Top + j + 1; k < Height; k++)
                        {
                            if (MapTiles[r.Left + i, k] == TileType.Wall)
                            {
                                //found a wall .... fill in the path
                                for (int p = r.Top + j + 1; p < k; p++)
                                {
                                    MapTiles[r.Left + i, p] = TileType.Path;
                                }
                                southPath = true;
                                break;
                            }
                        }
                    }
                }
                else
                {
                    MapTiles[r.Left + i, r.Top + j] = TileType.Room;
                }
                
            }
        }
    }

    public bool GenerateMap()
    {
        Rooms.Clear();
        Room? room;
        Clear();
        for (byte r = 0; r < MaxRooms; r++)
        {
            int index = 0;
            room = null;
            do
            {
                room = GenerateRoom(r);
                index++;
            }
            while (room == null && index < 1000);

            if(room == null )
            {
                break;
            }

            Rooms.Add(room);
            UpdateMap(room);
            AddPaths(room);
        }
        Console.WriteLine("Rooms defined");

        return true;
    }

    void Clear ()
    {
        MapTiles = new TileType[Width, Height];
    }
}
