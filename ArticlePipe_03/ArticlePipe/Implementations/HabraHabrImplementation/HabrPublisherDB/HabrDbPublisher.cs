using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArticlePipe.Implementations.HabraHabrImplementation.HabrPublisherDB
{
    public sealed class HabrDbPublisher
    {

        private string _connectionString;
        private int _currentRange;
        private SqlConnection _sn;


        #region Constructors
        public HabrDbPublisher(string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=D:\Projects\c#_class_02_02\ArticlePipe_03\ArticlePipe\Implementations\HabraHabrImplementation\HabrPublisherDB\HabrTagsDB.mdf;Integrated Security=True")
        {
            _connectionString = connectionString;

            var currentRange = GetProcessedArticlesCount();

            _currentRange = Math.Max(0, currentRange);

        }
        #endregion


        #region Properties

        public int CurrentRange { get => _currentRange; }

        #endregion
        

        #region Work with DB

        public void StartFill(int maxRange, bool needPrint = false, int Printstep = 1000)
        {
            if (maxRange <= _currentRange)
                throw new Exception("Invalid Range");

            for (var i = _currentRange + 1; i <= maxRange; i++)
            {
                
            }
        }

        public bool AddArticleID(string tag, string articleId)
        {
            using (var cn = new SqlConnection())
            {
                cn.ConnectionString = _connectionString;

                var tagHash = tag.GetHashCode();

                cn.Open();
                var strSQL = $"SELECT ArtiledId FROM Tags WHERE TagHash";
                var myCommand = new SqlCommand(strSQL, cn);
                var i = myCommand.ExecuteNonQuery();
                cn.Close();
            }
            return true;
        }

        #endregion


        #region Private Db methods

        private void OpenConnection()
        {
            _sn = new SqlConnection(_connectionString);
            _sn.Open();
        }

        private void CloseConnection()
        {
            _sn.Close();
        }


        private int GetProcessedArticlesCount()
        {
            OpenConnection();

            var strSQL = "SELECT ProcessedArticleCount FROM ProcessedArticles";
            var myCommand = new SqlCommand(strSQL, _sn);
            var currentRange = (int)myCommand.ExecuteScalar();

            CloseConnection();

            return currentRange;
        }

        public void Insert(int tagHash, string tag, int articlesCount, string articlesId)
        {
            OpenConnection();
            var sql = string.Format("Insert Into Tags" +
                                       "(TagHash, Tag, ArticlesCount, ArticlesId) Values(@TagHash, @Tag, @ArticlesCount, @ArticlesId)");

            using (var cmd = new SqlCommand(sql, _sn))
            {
                cmd.Parameters.AddWithValue("@TagHash", tagHash);
                cmd.Parameters.AddWithValue("@Tag", tag);
                cmd.Parameters.AddWithValue("@ArticlesCount", articlesCount);
                cmd.Parameters.AddWithValue("@ArticlesId", articlesId);

                cmd.ExecuteNonQuery();
            }

            CloseConnection();
        }

        #endregion
    }
}
