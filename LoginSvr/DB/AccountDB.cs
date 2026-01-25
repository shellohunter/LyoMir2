using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using SystemModule;
using SystemModule.Packet;

namespace LoginSvr
{
    public class AccountDB
    {
        private static readonly AccountDB instance = new AccountDB();

        public static AccountDB Instance
        {
            get { return instance; }
        }
        private LogQueue _logQueue => LogQueue.Instance;
        private ConfigManager _configManager => ConfigManager.Instance;
        private readonly IList<AccountQuick> _quickList = null;
        private readonly object _quickListLock = new();

        public AccountDB()
        {
            _quickList = new List<AccountQuick>();
        }

        public Config Config => _configManager.Config;

        public void Initialization()
        {
            _logQueue.Enqueue("正在连接SQL服务器...");
            var dbConnection = new MySqlConnection(Config.ConnctionString);
            try
            {
                dbConnection.Open();
                _logQueue.Enqueue("连接SQL服务器成功...");
                LoadQuickList();
            }
            catch (Exception E)
            {
                _logQueue.Enqueue("[警告] SQL 连接失败!请检查SQL设置...");
                _logQueue.Enqueue(Config.ConnctionString);
                _logQueue.Enqueue(E.StackTrace);
            }
        }

        public bool Open(ref MySqlConnection dbConnection)
        {
            bool result = false;
            if (dbConnection == null)
            {
                dbConnection = new MySqlConnection(Config.ConnctionString);
            }
            switch (dbConnection.State)
            {
                case ConnectionState.Open:
                    return true;
                case ConnectionState.Closed:
                    try
                    {
                        dbConnection.Open();
                        result = true;
                    }
                    catch (Exception e)
                    {
                        _logQueue.Enqueue("打开数据库[MySql]失败.");
                        _logQueue.Enqueue(e);
                        result = false;
                    }
                    break;
            }
            return result;
        }

        public void Close(ref MySqlConnection dbConnection)
        {
            if (dbConnection != null)
            {
                dbConnection.Close();
                dbConnection.Dispose();
            }
        }

        private void LoadQuickList()
        {
            int nIndex = 0;
            bool boDeleted;
            string sAccount;
            const string sSQL = "SELECT Id,FLD_DELETED,FLD_LOGINID FROM TBL_ACCOUNT";
            var loaded = new List<AccountQuick>();
            MySqlConnection dbConnection = null;
            if (!Open(ref dbConnection))
            {
                return;
            }
            try
            {
                var command = new MySqlCommand();
                command.CommandText = sSQL;
                command.Connection = (MySqlConnection)dbConnection;
                using var dr = command.ExecuteReader();
                while (dr.Read())
                {
                    nIndex = dr.GetInt32("Id");
                    boDeleted = dr.GetBoolean("FLD_DELETED");
                    sAccount = dr.GetString("FLD_LOGINID");
                    if (!boDeleted && (!string.IsNullOrEmpty(sAccount)))
                    {
                        loaded.Add(new AccountQuick(sAccount.Trim(), nIndex));
                    }
                }
                dr.Close();
                dr.Dispose();
            }
            finally
            {
                Close(ref dbConnection);
            }

            lock (_quickListLock)
            {
                _quickList.Clear();
                foreach (AccountQuick q in loaded)
                    _quickList.Add(q);
            }
            
        }

        public int FindByName(string sName, ref IList<AccountQuick> List)
        {
            AccountQuick[] snapshot;
            lock (_quickListLock)
            {
                snapshot = _quickList.ToArray();
            }

            for (var i = 0; i < snapshot.Length; i++)
            {
                if (HUtil32.CompareLStr(snapshot[i].sAccount, sName, sName.Length))
                    List.Add(new AccountQuick(snapshot[i].sAccount, snapshot[i].nIndex));
            }

            return List.Count;
        }

        public bool GetBy(int nIndex, ref TAccountDBRecord DBRecord)
        {
            var ok = false;
            lock (_quickListLock)
            {
                ok = (nIndex >= 0) && (_quickList.Count > nIndex);
            }

            if (!ok)
                return false;

            return GetRecord(nIndex, ref DBRecord);
        }

        private bool GetRecord(int nIndex, ref TAccountDBRecord DBRecord)
        {
            const string sSQL = "SELECT * FROM TBL_ACCOUNT WHERE ID={0}";
            var result = true;
            MySqlConnection dbConnection = null;
            if (!Open(ref dbConnection))
            {
                return false;
            }
            var command = new MySqlCommand();
            command.CommandText = string.Format(sSQL, nIndex);
            command.Connection = (MySqlConnection)dbConnection;
            IDataReader dr;
            try
            {
                dr = command.ExecuteReader();
                if (DBRecord == null)
                {
                    DBRecord = new TAccountDBRecord();
                    DBRecord.Header = new TRecordHeader();
                    DBRecord.UserEntry = new TUserEntry();
                    DBRecord.UserEntryAdd = new TUserEntryAdd();
                }
                if (dr.Read())
                {
                    DBRecord.Header.sAccount = dr.GetString("FLD_LOGINID");
                    DBRecord.Header.boDeleted = dr.GetBoolean(dr.GetOrdinal("FLD_DELETED"));
                    DBRecord.Header.CreateDate = dr.GetDateTime("FLD_CREATEDATE");
                    DBRecord.Header.UpdateDate = dr.GetDateTime("FLD_LASTUPDATE");
                    DBRecord.nErrorCount = dr.GetInt32("FLD_ERRORCOUNT");
                    DBRecord.dwActionTick = dr.GetInt32("FLD_ACTIONTICK");
                    DBRecord.UserEntry.sAccount = dr.GetString("FLD_LOGINID");
                    DBRecord.UserEntry.sPassword = dr.GetString("FLD_PASSWORD");
                    DBRecord.UserEntry.sUserName = dr.GetString("FLD_USERNAME");
                    DBRecord.UserEntry.sSSNo = dr.GetString("FLD_SSNO");
                    DBRecord.UserEntry.sPhone = dr.GetString("FLD_PHONE");
                    DBRecord.UserEntry.sQuiz = dr.GetString("FLD_QUIZ1");
                    DBRecord.UserEntry.sAnswer = dr.GetString("FLD_ANSWER1");
                    DBRecord.UserEntry.sEMail = dr.GetString("FLD_EMAIL");
                    DBRecord.UserEntryAdd.sQuiz2 = dr.GetString("FLD_QUIZ2");
                    DBRecord.UserEntryAdd.sAnswer2 = dr.GetString("FLD_ANSWER2");
                    DBRecord.UserEntryAdd.sBirthDay = dr.GetString("FLD_BIRTHDAY");
                    DBRecord.UserEntryAdd.sMobilePhone = dr.GetString("FLD_MOBILEPHONE");
                    DBRecord.UserEntryAdd.sMemo = "";
                    DBRecord.UserEntryAdd.sMemo2 = "";
                }
                AccountQuick quickAccount;
                lock (_quickListLock)
                {
                    quickAccount = _quickList.FirstOrDefault(x => x.nIndex == nIndex);
                }
                if (quickAccount != null)
                {
                    result = string.Equals(DBRecord.Header.sAccount?.Trim(), quickAccount.sAccount?.Trim(), StringComparison.OrdinalIgnoreCase);
                }
                else
                {
                    result = false;
                }
            }
            catch
            {
                result = false;
                _logQueue.Enqueue("[Exception] TFileIDDB.GetRecord (1)");
                return result;
            }
            finally
            {
                Close(ref dbConnection);
            }
            return result;
        }

        public int Index(string sName)
        {
            if (string.IsNullOrWhiteSpace(sName))
                return -1;

            sName = sName.Trim();

            AccountQuick quick;
            lock (_quickListLock)
            {
                quick = _quickList.FirstOrDefault(o => string.Equals(o.sAccount, sName, StringComparison.OrdinalIgnoreCase));
            }

            if (quick != null)
                return quick.nIndex;

            
            if (TryLoadQuickFromDb(sName, out AccountQuick loaded))
            {
                lock (_quickListLock)
                {
                    
                    var existing = _quickList.FirstOrDefault(o => string.Equals(o.sAccount, loaded.sAccount, StringComparison.OrdinalIgnoreCase));
                    if (existing != null)
                        return existing.nIndex;

                    _quickList.Add(loaded);
                }
                return loaded.nIndex;
            }

            return -1;
        }

        private bool TryLoadQuickFromDb(string loginId, out AccountQuick quick)
        {
            quick = null;
            const string sql = "SELECT Id, FLD_LOGINID FROM TBL_ACCOUNT WHERE FLD_DELETED=0 AND LOWER(FLD_LOGINID)=LOWER(@loginId) LIMIT 1";

            MySqlConnection dbConnection = null;
            if (!Open(ref dbConnection))
                return false;

            try
            {
                using var command = new MySqlCommand(sql, dbConnection);
                command.Parameters.AddWithValue("@loginId", loginId);

                using var dr = command.ExecuteReader();
                if (!dr.Read())
                    return false;

                int id = dr.GetInt32("Id");
                string account = dr.GetString("FLD_LOGINID");
                if (id <= 0 || string.IsNullOrWhiteSpace(account))
                    return false;

                quick = new AccountQuick(account.Trim(), id);
                return true;
            }
            catch (Exception e)
            {
                _logQueue.Enqueue("[Exception] AccountDB.TryLoadQuickFromDb");
                _logQueue.Enqueue(e.Message);
                return false;
            }
            finally
            {
                Close(ref dbConnection);
            }
        }

        public int Get(int nIndex, ref TAccountDBRecord DBRecord)
        {
            int result = -1;
            if (nIndex < 0)
            {
                return result;
            }
            
            
            
            
            if (GetRecord(nIndex, ref DBRecord))
            {
                result = nIndex;
            }
            return result;
        }

        private int UpdateRecord(TAccountDBRecord DBRecord, byte btFlag)
        {
            var result = -1;
            string sdt = "now()";
            const string sUpdateRecord1 = "INSERT INTO TBL_ACCOUNT (FLD_LOGINID, FLD_PASSWORD, FLD_USERNAME, FLD_CREATEDATE, FLD_LASTUPDATE, FLD_DELETED, FLD_ERRORCOUNT, FLD_ACTIONTICK, FLD_SSNO, FLD_BIRTHDAY, FLD_PHONE, FLD_MOBILEPHONE, FLD_EMAIL, FLD_QUIZ1, FLD_ANSWER1, FLD_QUIZ2, FLD_ANSWER2) VALUES('{0}', '{1}', '{2}', {3}, {4}, 0, 0, 0,'{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}');";
            const string sUpdateRecord2 = "UPDATE TBL_ACCOUNT SET FLD_DELETED=1, FLD_CREATEDATE='{0}' WHERE FLD_LOGINID='{1}'";
            const string sUpdateRecord0 = "UPDATE TBL_ACCOUNT SET FLD_PASSWORD='{0}', FLD_USERNAME='{1}',FLD_LASTUPDATE={2}, FLD_ERRORCOUNT={3}, FLD_ACTIONTICK={4},FLD_SSNO='{5}', FLD_BIRTHDAY='{6}', FLD_PHONE='{7}',FLD_MOBILEPHONE='{8}', FLD_EMAIL='{9}', FLD_QUIZ1='{10}', FLD_ANSWER1='{11}', FLD_QUIZ2='{12}',FLD_ANSWER2='{13}' WHERE FLD_LOGINID='{14}'";
            MySqlConnection dbConnection = null;
            if (!Open(ref dbConnection))
            {
                return -1;
            }
            try
            {
                var command = new MySqlCommand();
                command.Connection = (MySqlConnection)dbConnection;
                switch (btFlag)
                {
                    case 1:
                        command.CommandText = string.Format(sUpdateRecord1, new object[] { DBRecord.UserEntry.sAccount, DBRecord.UserEntry.sPassword, DBRecord.UserEntry.sUserName, sdt, sdt, DBRecord.UserEntry.sSSNo, DBRecord.UserEntryAdd.sBirthDay, DBRecord.UserEntry.sPhone, DBRecord.UserEntryAdd.sMobilePhone, DBRecord.UserEntry.sEMail, DBRecord.UserEntry.sQuiz, DBRecord.UserEntry.sAnswer, DBRecord.UserEntryAdd.sQuiz2, DBRecord.UserEntryAdd.sAnswer2 });
                        try
                        {
                            command.ExecuteNonQuery();
                            result = (int)command.LastInsertedId;
                        }
                        catch (Exception E)
                        {
                            _logQueue.Enqueue("[Exception] TFileIDDB.UpdateRecord");
                            _logQueue.Enqueue(E.Message);
                            return -1;
                        }
                        break;
                    case 2:
                        command.CommandText = string.Format(sUpdateRecord2, sdt, DBRecord.UserEntry.sAccount);
                        try
                        {
                            result = command.ExecuteNonQuery();
                        }
                        catch
                        {
                            result = -1;
                            _logQueue.Enqueue("[Exception] TFileIDDB.UpdateRecord (3)");
                        }
                        break;
                    default:
                        command.CommandText = string.Format(sUpdateRecord0, new object[] { DBRecord.UserEntry.sPassword, DBRecord.UserEntry.sUserName, sdt, DBRecord.nErrorCount, DBRecord.dwActionTick, DBRecord.UserEntry.sSSNo, DBRecord.UserEntryAdd.sBirthDay, DBRecord.UserEntry.sPhone, DBRecord.UserEntryAdd.sMobilePhone, DBRecord.UserEntry.sEMail, DBRecord.UserEntry.sQuiz, DBRecord.UserEntry.sAnswer, DBRecord.UserEntryAdd.sQuiz2, DBRecord.UserEntryAdd.sAnswer2, DBRecord.UserEntry.sAccount });
                        try
                        {
                            result = command.ExecuteNonQuery();
                        }
                        catch (Exception E)
                        {
                            result = -1;
                            _logQueue.Enqueue("[Exception] TFileIDDB.UpdateRecord (0)");
                            _logQueue.Enqueue(E.Message);
                            return result;
                        }
                        break;
                }
            }
            finally
            {
                Close(ref dbConnection);
            }
            return result;
        }

        public bool Update(int nIndex, ref TAccountDBRecord DBRecord)
        {
            if (nIndex < 0)
            {
                return false;
            }

            return UpdateRecord(DBRecord, 0) >= 0;
        }

        public bool Add(ref TAccountDBRecord DBRecord)
        {
            if (DBRecord?.UserEntry == null)
                return false;

            var sAccount = DBRecord.UserEntry.sAccount;
            if (string.IsNullOrWhiteSpace(sAccount))
                return false;

            sAccount = sAccount.Trim();
            DBRecord.UserEntry.sAccount = sAccount;

            if (Index(sAccount) >= 0)
                return false;

            var nIndex = UpdateRecord(DBRecord, 1);
            if (nIndex <= 0)
                return false;

            lock (_quickListLock)
            {
                
                if (_quickList.Any(o => string.Equals(o.sAccount, sAccount, StringComparison.OrdinalIgnoreCase)))
                    return true;

                _quickList.Add(new AccountQuick(sAccount, nIndex));
            }

            return true;
        }

        public bool Delete(int nIndex, ref TAccountDBRecord DBRecord)
        {
            if (nIndex < 0)
            {
                return false;
            }

            if (DBRecord?.UserEntry == null)
                return false;

            
            if (string.IsNullOrWhiteSpace(DBRecord.UserEntry.sAccount))
            {
                lock (_quickListLock)
                {
                    var existing = _quickList.FirstOrDefault(q => q.nIndex == nIndex);
                    if (existing != null)
                        DBRecord.UserEntry.sAccount = existing.sAccount;
                }
            }

            var up = UpdateRecord(DBRecord, 2);
            if (up < 0)
                return false;

            lock (_quickListLock)
            {
                for (var i = 0; i < _quickList.Count; i++)
                {
                    if (_quickList[i].nIndex == nIndex)
                    {
                        _quickList.RemoveAt(i);
                        break;
                    }
                }
            }

            return true;
        }

    }
}
