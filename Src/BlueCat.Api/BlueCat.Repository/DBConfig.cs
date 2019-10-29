namespace BlueCat.Repository
{
    /// <summary>
    /// 数据量配置项
    /// </summary>
    public class DBConfig
    {
        public MongoDB MongoDB { get; set; }
    }

    public class MongoDB
    {
        public string RepositoryTest { get; set; }

        public string APILogs { get; set; }
    }
}
