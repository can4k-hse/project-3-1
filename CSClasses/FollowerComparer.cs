// Author: Alexander Yakovlev
// CreatedAt: 31 января 2025 г. 12:56:27
// Filename: FollowerComparer.cs
// Summary: Класс для создания компаратора на основе метода


using CSClasses;

public class FollowerComparer(FollowerComparer.CompareDelegate compare) : IComparer<Follower>
{
    public delegate int CompareDelegate(Follower x, Follower y);

    public int Compare(Follower x, Follower y)
    {
        return compare?.Invoke(x, y) ?? throw new NotImplementedException();
    }
}