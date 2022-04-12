using System.Data;
using MySqlX.XDevAPI;

namespace WebApp.Services
{
    public class dbSession
    {
        private Session _MySqlSession;
        public Schema _schema;
        public dbSession(string connectionURL, string schemaName) {
            _MySqlSession = MySQLX.GetSession(connectionURL);
            _schema = _MySqlSession.GetSchema(schemaName);
        }
        public void CloseSession() { _MySqlSession.Close(); }

    }
}
