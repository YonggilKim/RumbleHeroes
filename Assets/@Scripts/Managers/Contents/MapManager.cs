using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

public struct Pos
{
    public Pos(int y, int x)
    {
        Y = y;
        X = x;
    }

    public int Y;
    public int X;
}

public struct PQNode : IComparable<PQNode>
{
    public int F;
    public int G;
    public int Y;
    public int X;

    public int CompareTo(PQNode other)
    {
        if (F == other.F)
            return 0;
        return F < other.F ? 1 : -1;
    }
}

public struct ObjectSpawnInfo
{
    public ObjectSpawnInfo(int dataId, int x, int y)
    {
        DataId = dataId;
        Vector3Int pos = new Vector3Int(x, y, 0);
        SpawnPos = pos;
    }

    public int DataId;
    public Vector3Int SpawnPos;
}

class Cell
{
    public HashSet<BaseController> Objects { get; } = new HashSet<BaseController>();
}

public class MapManager
{
    public Grid CurrentGrid { get; private set; }
    Dictionary<Vector3Int, Cell> _cells = new Dictionary<Vector3Int, Cell>();

    public int MinX { get; set; }
    public int MaxX { get; set; }
    public int MinY { get; set; }
    public int MaxY { get; set; }

    public int SizeX
    {
        get { return MaxX - MinX + 1; }
    }

    public int SizeY
    {
        get { return MaxY - MinY + 1; }
    }

    public List<ObjectSpawnInfo> MonsterSpawnInfos { get; private set; } = new List<ObjectSpawnInfo>();
    public List<ObjectSpawnInfo> GatheringResourceSpawnInfos { get; private set; } = new List<ObjectSpawnInfo>();
    bool[,] _collision;
    private int[,] _monsters;

    public bool CanGo(Vector3Int cellPos)
    {
        if (cellPos.x < MinX || cellPos.x > MaxX)
            return false;
        if (cellPos.y < MinY || cellPos.y > MaxY)
            return false;

        int x = cellPos.x - MinX;
        int y = MaxY - cellPos.y;
        return !_collision[y, x];
    }

    public void LoadMap(string mapName)
    {
        DestroyMap();

        GameObject map = Managers.Resource.Instantiate(mapName);
        map.transform.position = Vector3.zero;
        map.name = $"@Map_{mapName}";

        #region collision

        GameObject collision = Util.FindChild(map, "Tilemap_Collision", true);
        if (collision != null)
            collision.SetActive(false);

        // CurrentGrid = Util.FindChild<Grid>(go, "SmallGrid");
        CurrentGrid = map.GetComponent<Grid>();

        // Collision 관련 파일
        TextAsset txt = Managers.Resource.Load<TextAsset>($"{mapName}Collision");
        StringReader reader = new StringReader(txt.text);

        MinX = int.Parse(reader.ReadLine());
        MaxX = int.Parse(reader.ReadLine());
        MinY = int.Parse(reader.ReadLine());
        MaxY = int.Parse(reader.ReadLine());

        int xCount = MaxX - MinX + 1;
        int yCount = MaxY - MinY + 1;
        _collision = new bool[yCount, xCount];

        for (int y = 0; y < yCount; y++)
        {
            string line = reader.ReadLine();
            for (int x = 0; x < xCount; x++)
            {
                _collision[y, x] = (line[x] == '1' ? true : false);
            }
        }

        #endregion

        #region 몬스터 위치정보 가져오기

        Tilemap tmBase = Util.FindChild<Tilemap>(map, "Tilemap_Base", true);
        Tilemap tmMonster = Util.FindChild<Tilemap>(map, "Tilemap_Monster", true);


        if (tmMonster != null)
            tmMonster.gameObject.SetActive(false);

        for (int y = tmMonster.cellBounds.yMax; y >= tmMonster.cellBounds.yMin; y--)
        {
            for (int x = tmMonster.cellBounds.xMin; x <= tmMonster.cellBounds.xMax; x++)
            {
                TileBase tile = tmMonster.GetTile(new Vector3Int(x, y, 0));
                if (tile != null)
                {
                    Debug.Log($"({x},{y}) name : {tile.name}");

                    int dataId = int.Parse(tile.name);

                    ObjectSpawnInfo info = new ObjectSpawnInfo(dataId, x, y);
                    MonsterSpawnInfos.Add(info);
                }
            }
        }

        #endregion

        #region 나무 위치정보 가져오기

        Tilemap tmTree = Util.FindChild<Tilemap>(map, "Tilemap_Trees", true);

        if (tmTree != null)
            tmTree.gameObject.SetActive(false);

        for (int y = tmTree.cellBounds.yMax; y >= tmTree.cellBounds.yMin; y--)
        {
            for (int x = tmTree.cellBounds.xMin; x <= tmTree.cellBounds.xMax; x++)
            {
                TileBase tile = tmTree.GetTile(new Vector3Int(x, y, 0));
                if (tile != null)
                {
                    Debug.Log($"({x},{y}) name : {tile.name}");

                    int dataId = int.Parse(tile.name);

                    ObjectSpawnInfo info = new ObjectSpawnInfo(dataId, x, y);
                    GatheringResourceSpawnInfos.Add(info);
                }
            }
        }

        #endregion
    }

    public void DestroyMap()
    {
        GameObject map = GameObject.Find("Map");
        if (map != null)
        {
            GameObject.Destroy(map);
            CurrentGrid = null;
        }
    }

    private List<Vector3Int> GatherPoints = new List<Vector3Int>();

    private Vector2 _gatherPoint;

    public Vector2 GatheringPoint
    {
        get => _gatherPoint;
        set
        {
            _gatherPoint = value;
            SetGatherPoints(_gatherPoint);
        }
    }

    public void SetGatherPoints(Vector3 gatherPoint)
    {
        // 3       3
        //   2 2 2
        //   2 O 2
        //   2 2 2
        // 3       3
        int[] dx = { 2, 0, -2, 0, 2, 2, -2, -2, 3, 3, -3, -3 };
        int[] dy = { 0, 2, 0, -2, 2, -2, 2, -2, 3, -3, 3, -3 };

        GatherPoints.Clear();
        Vector3Int cur = CurrentGrid.WorldToCell(gatherPoint);
        for (int dir = 0; dir < dx.Length; dir++)
        {
            Vector3Int temp = new Vector3Int(cur.x + dx[dir], cur.y + dy[dir]);
            GatherPoints.Add(temp);
        }
    }

    public Vector3 GetGatheringPoint(bool isLeader)
    {
        if (isLeader)
            return GatheringPoint;

        var ret = CurrentGrid.GetCellCenterWorld(GatherPoints[Random.Range(0, 11)]);
        return ret;
    }

    #region Map Grid

    public void Add(BaseController go)
    {
        Vector3Int cellPos = CurrentGrid.WorldToCell(go.transform.position);

        Cell cell = GetCell(cellPos);
        if (cell == null)
            return;

        cell.Objects.Add(go);
    }

    public void Remove(BaseController go)
    {
        Vector3Int cellPos = CurrentGrid.WorldToCell(go.transform.position);

        Cell cell = GetCell(cellPos);
        if (cell == null)
            return;

        cell.Objects.Remove(go);
    }

    Cell GetCell(Vector3Int cellPos)
    {
        Cell cell = null;

        if (_cells.TryGetValue(cellPos, out cell) == false)
        {
            cell = new Cell();
            _cells.Add(cellPos, cell);
        }

        return cell;
    }

    public List<BaseController> GatherObjects(Vector3 pos, float range)
    {
        List<BaseController> objects = new List<BaseController>();

        Vector3Int left = CurrentGrid.WorldToCell(pos + new Vector3(-range, 0));
        Vector3Int right = CurrentGrid.WorldToCell(pos + new Vector3(+range, 0));
        Vector3Int bottom = CurrentGrid.WorldToCell(pos + new Vector3(0, -range));
        Vector3Int top = CurrentGrid.WorldToCell(pos + new Vector3(0, +range));

        int minX = left.x;
        int maxX = right.x;
        int minY = bottom.y;
        int maxY = top.y;

        for (int x = minX; x <= maxX; x++)
        {
            for (int y = minY; y <= maxY; y++)
            {
                if (_cells.ContainsKey(new Vector3Int(x, y, 0)) == false)
                    continue;

                objects.AddRange(_cells[new Vector3Int(x, y, 0)].Objects);
            }
        }

        return objects;
    }

    #endregion

    #region A* PathFinding

    // U D L R
    int[] _deltaY = new int[] { 1, -1, 0, 0 };
    int[] _deltaX = new int[] { 0, 0, -1, 1 };
    int[] _cost = new int[] { 10, 10, 10, 10 };

    public List<Vector3Int> FindPath(Vector3Int startCellPos, Vector3Int destCellPos, bool ignoreDestCollision = false)
    {
        List<Pos> path = new List<Pos>();

        // 점수 매기기
        // F = G + H
        // F = 최종 점수 (작을 수록 좋음, 경로에 따라 달라짐)
        // G = 시작점에서 해당 좌표까지 이동하는데 드는 비용 (작을 수록 좋음, 경로에 따라 달라짐)
        // H = 목적지에서 얼마나 가까운지 (작을 수록 좋음, 고정)

        // (y, x) 이미 방문했는지 여부 (방문 = closed 상태)
        bool[,] closed = new bool[SizeY, SizeX]; // CloseList

        // (y, x) 가는 길을 한 번이라도 발견했는지
        // 발견X => MaxValue
        // 발견O => F = G + H
        int[,] open = new int[SizeY, SizeX]; // OpenList
        for (int y = 0; y < SizeY; y++)
        for (int x = 0; x < SizeX; x++)
            open[y, x] = Int32.MaxValue;

        Pos[,] parent = new Pos[SizeY, SizeX];

        // 오픈리스트에 있는 정보들 중에서, 가장 좋은 후보를 빠르게 뽑아오기 위한 도구
        PriorityQueue<PQNode> pq = new PriorityQueue<PQNode>();

        // CellPos -> ArrayPos
        Pos pos = Cell2Pos(startCellPos);
        Pos dest = Cell2Pos(destCellPos);

        // 시작점 발견 (예약 진행)
        open[pos.Y, pos.X] = 10 * (Math.Abs(dest.Y - pos.Y) + Math.Abs(dest.X - pos.X));
        pq.Push(new PQNode()
            { F = 10 * (Math.Abs(dest.Y - pos.Y) + Math.Abs(dest.X - pos.X)), G = 0, Y = pos.Y, X = pos.X });
        parent[pos.Y, pos.X] = new Pos(pos.Y, pos.X);

        while (pq.Count > 0)
        {
            // 제일 좋은 후보를 찾는다
            PQNode node = pq.Pop();
            // 동일한 좌표를 여러 경로로 찾아서, 더 빠른 경로로 인해서 이미 방문(closed)된 경우 스킵
            if (closed[node.Y, node.X])
                continue;

            // 방문한다
            closed[node.Y, node.X] = true;
            // 목적지 도착했으면 바로 종료
            if (node.Y == dest.Y && node.X == dest.X)
                break;

            // 상하좌우 등 이동할 수 있는 좌표인지 확인해서 예약(open)한다
            for (int i = 0; i < _deltaY.Length; i++)
            {
                Pos next = new Pos(node.Y + _deltaY[i], node.X + _deltaX[i]);

                // 유효 범위를 벗어났으면 스킵
                // 벽으로 막혀서 갈 수 없으면 스킵
                if (!ignoreDestCollision || next.Y != dest.Y || next.X != dest.X)
                {
                    if (CanGo(Pos2Cell(next)) == false) // CellPos
                        continue;
                }

                // 이미 방문한 곳이면 스킵
                if (closed[next.Y, next.X])
                    continue;

                // 비용 계산
                int g = 0; // node.G + _cost[i];
                int h = 10 * ((dest.Y - next.Y) * (dest.Y - next.Y) + (dest.X - next.X) * (dest.X - next.X));
                // 다른 경로에서 더 빠른 길 이미 찾았으면 스킵
                if (open[next.Y, next.X] < g + h)
                    continue;

                // 예약 진행
                open[dest.Y, dest.X] = g + h;
                pq.Push(new PQNode() { F = g + h, G = g, Y = next.Y, X = next.X });
                parent[next.Y, next.X] = new Pos(node.Y, node.X);
            }
        }

        return CalcCellPathFromParent(parent, dest);
    }

    List<Vector3Int> CalcCellPathFromParent(Pos[,] parent, Pos dest)
    {
        List<Vector3Int> cells = new List<Vector3Int>();

        int y = dest.Y;
        int x = dest.X;
        while (parent[y, x].Y != y || parent[y, x].X != x)
        {
            cells.Add(Pos2Cell(new Pos(y, x)));
            Pos pos = parent[y, x];
            y = pos.Y;
            x = pos.X;
        }

        cells.Add(Pos2Cell(new Pos(y, x)));
        cells.Reverse();

        return cells;
    }

    Pos Cell2Pos(Vector3Int cell)
    {
        // CellPos -> ArrayPos
        return new Pos(MaxY - cell.y, cell.x - MinX);
    }

    Vector3Int Pos2Cell(Pos pos)
    {
        // ArrayPos -> CellPos
        return new Vector3Int(pos.X + MinX, MaxY - pos.Y, 0);
    }

    #endregion
}


public class PriorityQueue<T> where T : IComparable<T>
{
    List<T> _heap = new List<T>();

    // O(logN)
    public void Push(T data)
    {
        // 힙의 맨 끝에 새로운 데이터를 삽입한다
        _heap.Add(data);

        int now = _heap.Count - 1;
        // 도장깨기를 시작
        while (now > 0)
        {
            // 도장깨기를 시도
            int next = (now - 1) / 2;
            if (_heap[now].CompareTo(_heap[next]) < 0)
                break; // 실패

            // 두 값을 교체한다
            T temp = _heap[now];
            _heap[now] = _heap[next];
            _heap[next] = temp;

            // 검사 위치를 이동한다
            now = next;
        }
    }

    // O(logN)
    public T Pop()
    {
        // 반환할 데이터를 따로 저장
        T ret = _heap[0];

        // 마지막 데이터를 루트로 이동한다
        int lastIndex = _heap.Count - 1;
        _heap[0] = _heap[lastIndex];
        _heap.RemoveAt(lastIndex);
        lastIndex--;

        // 역으로 내려가는 도장깨기 시작
        int now = 0;
        while (true)
        {
            int left = 2 * now + 1;
            int right = 2 * now + 2;

            int next = now;
            // 왼쪽값이 현재값보다 크면, 왼쪽으로 이동
            if (left <= lastIndex && _heap[next].CompareTo(_heap[left]) < 0)
                next = left;
            // 오른값이 현재값(왼쪽 이동 포함)보다 크면, 오른쪽으로 이동
            if (right <= lastIndex && _heap[next].CompareTo(_heap[right]) < 0)
                next = right;

            // 왼쪽/오른쪽 모두 현재값보다 작으면 종료
            if (next == now)
                break;

            // 두 값을 교체한다
            T temp = _heap[now];
            _heap[now] = _heap[next];
            _heap[next] = temp;
            // 검사 위치를 이동한다
            now = next;
        }

        return ret;
    }

    public int Count
    {
        get { return _heap.Count; }
    }
}