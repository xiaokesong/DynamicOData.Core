using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Threading;

namespace DynamicOData.Core.DataSource.SqlServer
{
    class DbAccess : IDisposable
    {
        #region Memeber
        SqlConnection _Connection;
        const int _MaxRetryCount = 2;
        const int _IncreasingDelayRetry = 500;
        const int _CommandTimeout = 180;  //超时时间设置为3分钟
        #endregion

        #region Construct
        public DbAccess(string connectionString)
        {
            _Connection = new SqlConnection(connectionString);
            _Connection.Open();
        }
        #endregion

        #region Method`
        public SqlParameterCollection ExecuteReader(string commandText, Action<SqlDataReader> dataReader, Action<SqlParameterCollection> parametersBuilder = null, CommandType commandType = CommandType.StoredProcedure, int commandTimeout = 0)
        {
            SqlParameterCollection pars = null;
            using (SqlDataReader reader = CreateReader(commandText, commandTimeout, commandType, parametersBuilder, out pars))
            {
                if (dataReader == null)
                    return pars;
                while (reader.Read())
                    dataReader(reader);
            }
            return pars;
        }

        public SqlParameterCollection ExecuteReader(string commandText, Action<SqlDataReader> firstReader, Action<SqlDataReader> secondReader, Action<SqlParameterCollection> parametersBuilder = null, CommandType commandType = CommandType.StoredProcedure, int commandTimeout = 0)
        {
            SqlParameterCollection pars = null;
            using (SqlDataReader reader = CreateReader(commandText, commandTimeout, commandType, parametersBuilder, out pars))
            {
                if (firstReader == null)
                    return pars;
                while (reader.Read())
                    firstReader(reader);
                if (secondReader == null)
                    return pars;
                while (reader.NextResult() && reader.Read())
                    secondReader(reader);
            }
            return pars;
        }

        public object ExecuteScalar(string commandText, Action<SqlParameterCollection> parametersBuilder,
           CommandType commandType = CommandType.StoredProcedure, int commandTimeout = 0)
        {
            object rtv = 0;

            for (int retry = 0; ; retry++)
            {
                try
                {
                    rtv = CreateCommand(commandText, commandTimeout, commandType, parametersBuilder).ExecuteScalar();
                    break;
                }
                catch (Exception e)
                {
                    if (retry < _MaxRetryCount && OnConnectionLost(e))
                        ReConnect(retry);
                    else
                        throw;
                }
            }

            return rtv;
        }
        public int ExecuteNonQuery(string commandText, Action<SqlParameterCollection> parametersBuilder,
            CommandType commandType = CommandType.StoredProcedure, int commandTimeout = 0)
        {
            int nAffectedRows = 0;

            for (int retry = 0; ; retry++)
            {
                try
                {
                    nAffectedRows = CreateCommand(commandText, commandTimeout, commandType, parametersBuilder).ExecuteNonQuery();
                    break;
                }
                catch (Exception e)
                {
                    if (retry < _MaxRetryCount && OnConnectionLost(e))
                        ReConnect(retry);
                    else
                        throw;
                }
            }

            return nAffectedRows;
        }
        SqlDataReader CreateReader(string commandText
            , int commandTimeout
            , CommandType commandType
            , Action<SqlParameterCollection> parametersBuilder
            , out SqlParameterCollection pars
            , int resultSetCnt = 1)
        {
            for (int retry = 0; ; retry++)
            {
                try
                {
                    SqlCommand dbCmd = CreateCommand(commandText, commandTimeout, commandType, parametersBuilder);
                    pars = dbCmd.Parameters;
                    if (dbCmd.Connection.State != ConnectionState.Open)
                        dbCmd.Connection.Open();
                    return dbCmd.ExecuteReader(CommandBehavior.CloseConnection);
                }
                catch (Exception e)
                {
                    if (retry < _MaxRetryCount && OnConnectionLost(e))
                        ReConnect(retry);
                    else
                        throw;
                }
            }
        }
        bool OnConnectionLost(Exception dbException)
        {
            bool canRetry = false;

            SqlException e = dbException as SqlException;

            if (e == null)
                canRetry = false;
            else
                switch (e.Number)
                {
                    case 233:
                    case -2: canRetry = true; break;
                    default: canRetry = false; break;
                }
            return canRetry;
        }
        void ReConnect(int retrying)
        {
            if (_Connection != null)
                if (_Connection.State != ConnectionState.Closed)
                {
                    _Connection.Close();

                    if (retrying > 0)
                        Thread.Sleep(retrying * _IncreasingDelayRetry);	// retrying starts at 0, increases delay time for every retry.

                    _Connection.Open();
                }
        }
        SqlCommand CreateCommand(string commandText, int commandTimeout, CommandType commandType, Action<SqlParameterCollection> parametersBuilder)
        {
            if (_Connection == null)
                throw new ObjectDisposedException("DbAccess");

            SqlCommand dbCommand = _Connection.CreateCommand();
            dbCommand.CommandTimeout = _CommandTimeout;
            dbCommand.CommandType = commandType;
            dbCommand.CommandText = commandText;

            if (commandTimeout > 0)
                dbCommand.CommandTimeout = commandTimeout;

            if (parametersBuilder != null)
                parametersBuilder(dbCommand.Parameters);

            return dbCommand;
        }
        #endregion

        #region IDisposable Members
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing && _Connection != null)
            {
                if (_Connection.State != ConnectionState.Closed)
                {
                    _Connection.Dispose();
                }

                _Connection = null;
            }
        }
        #endregion
    }
}
