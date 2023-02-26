namespace Endure.Server;

public enum DatabaseType { Sqlite, SqlServer }

public static class Constants
{
    public static readonly DatabaseType DatabaseType = DatabaseType.Sqlite;
}