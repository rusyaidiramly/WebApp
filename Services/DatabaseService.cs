using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using WebApp.Models;

namespace WebApp.Services
{
    public class DatabaseService
    {
        // private static SqlDataAdapter dataAdapter = new SqlDataAdapter();

        public DatabaseService()
        {
            if (dbConnection.GetConnectionStatus != ConnectionState.Open)
            {
                dbConnection.OpenConnection();
            }
        }

        private Int32 MaxRecNo(string tableName)
        {
            try
            {
                if (dbConnection.GetConnectionStatus != ConnectionState.Open)
                {
                    dbConnection.OpenConnection();
                }

                DataSet ds = new DataSet();
                Int32 maxIdx;

                string maxIdxCommand = @"SELECT MAX(""RecNo"") FROM {tableName}";
                maxIdxCommand = maxIdxCommand.Replace("{tableName}", tableName);

                // dataAdapter.SelectCommand = new SqlCommand(maxIdxCommand, dbConnection.GetConnection);
                // dataAdapter.Fill(ds);

                maxIdx = Convert.ToInt32(ds.Tables[0].Rows[0].ItemArray[0].ToString()) + 1;

                return maxIdx;
            }
            catch (Exception)
            {
                return 1;
                //throw;
            }
        }
/*
        public Int64 GetTransactionNo()
        {
            try
            {
                if (dbConnection.GetConnectionStatus != ConnectionState.Open)
                {
                    dbConnection.OpenConnection();
                }

                DataSet ds = new DataSet();
                Int32 lastTransactionNo;

                string transactionCommand = @"SELECT MAX(""TransactionNo"") FROM dbo.Activities";
                dataAdapter.SelectCommand = new SqlCommand(transactionCommand, dbConnection.GetConnection);
                dataAdapter.Fill(ds);

                if (ds.Tables[0].Rows[0].ItemArray[0].ToString() == string.Empty)
                {
                    lastTransactionNo = 0;
                }
                else
                {
                    lastTransactionNo = Convert.ToInt32(ds.Tables[0].Rows[0].ItemArray[0].ToString()) + 1;
                }

                return lastTransactionNo;
            }
            catch (Exception ex)
            {
                return -1;
                //throw;
            }
        }

        private void GetPersonalParticular(PersonalParticular personal)
        {
            try
            {
                if (dbConnection.GetConnectionStatus != ConnectionState.Open)
                {
                    dbConnection.OpenConnection();
                }

                DataSet ds = new DataSet();

                string transactionCommand = $"SELECT IDCode, MemberName, Gender, Age, Nationality FROM dbo.Members WHERE NRIC = '{personal.NRIC}'";
                dataAdapter.SelectCommand = new SqlCommand(transactionCommand, dbConnection.GetConnection);
                dataAdapter.Fill(ds);

                if (ds.Tables[0].Rows[0].ItemArray[0].ToString() != string.Empty)
                {
                    personal.IDCode = ds.Tables[0].Rows[0].ItemArray[0].ToString();
                    personal.Gender = ds.Tables[0].Rows[0].ItemArray[2].ToString();
                    personal.Age = Convert.ToInt32(ds.Tables[0].Rows[0].ItemArray[3].ToString());
                    personal.Nationality = ds.Tables[0].Rows[0].ItemArray[4].ToString();
                }
            }
            catch (Exception ex)
            {

                //throw;
            }
        }

        private int HikingActivitiesInsertCommand(ePermitApplicationRecord record, PersonalParticular personal)
        {
            try
            {
                if (dbConnection.GetConnectionStatus != ConnectionState.Open)
                {
                    dbConnection.OpenConnection();
                }

                Int32 recNo = MaxRecNo("HikingActivities");

                string insertCommand = $"INSERT INTO HikingActivities (RecNo, HikingDate, HikingSite, IDCode, Participant, Gender, Age, Nationality, ReferenceTag, CheckIn, Status) " +
                    "VALUES ({0}, CONVERT( datetime, '{1}', 103), '{2}', '{3}', '{4}', '{5}', {6}, '{7}', '{8}', CONVERT( datetime, '{9}', 103), {10})";

                insertCommand = string.Format(insertCommand,
                    recNo,
                    record.HikingDate,
                    record.HikingSite,
                    personal.IDCode,
                    personal.MemberName,
                    personal.Gender,
                    personal.Age,
                    personal.Nationality,
                    record.ReferenceTag,
                    record.DateCheckIn,
                    (record.PaymentCompleted ? 1 : 0));

                dataAdapter.InsertCommand = new SqlCommand(insertCommand, dbConnection.GetConnection);
                dataAdapter.InsertCommand.ExecuteNonQuery();

                return 1;
            }
            catch (Exception ex)
            {
                return -1;
                //throw;
            }
        }

        private int InsertHikingActivities(ePermitApplicationRecord record)
        {
            try 
            {
                string participants = record.Participant;
                string[] splitParticipants = participants.Split('-');
                List<PersonalParticular> participantList = new List<PersonalParticular>();

                ePermitApplicationRecord rec = record;

                foreach (string str in splitParticipants)
                {
                    if (!string.IsNullOrEmpty(str) && str != ";")
                    {
                        string[] participantDetail = str.Split(',');
                        participantList.Add(new PersonalParticular
                        {
                            MemberName = participantDetail[0],
                            NRIC = participantDetail[1],
                            PhoneNo = participantDetail[2]
                        });

                        GetPersonalParticular(participantList[participantList.Count - 1]);
                        HikingActivitiesInsertCommand(rec, participantList[participantList.Count - 1]);
                    }
                }

                return 1;
            }
            catch (Exception ex)
            {
                return -1;
                //throw;
            }
        }

        public void UpdateCheckInCheckOut(ePermitApplicationRecord record)
        {
            try
            {
                if (dbConnection.GetConnectionStatus != ConnectionState.Open)
                {
                    dbConnection.OpenConnection();
                }

                string insertCommand = $"UPDATE Activities SET CheckIn = " +
                    "CONVERT( datetime, '{0}', 103), CheckOut = CONVERT( datetime, '{1}', 103) WHERE ReferenceTag = '{2}'";


                insertCommand = string.Format(insertCommand,
                    record.DateCheckIn,
                    record.DateCheckOut == null ? null : record.DateCheckOut,
                    record.ReferenceTag);

                dataAdapter.InsertCommand = new SqlCommand(insertCommand, dbConnection.GetConnection);
                dataAdapter.InsertCommand.ExecuteNonQuery();

                insertCommand = $"UPDATE HikingActivities SET CheckIn = " +
                    "CONVERT( datetime, '{0}', 103), CheckOut = CONVERT( datetime, '{1}', 103) WHERE ReferenceTag = '{2}'";

                insertCommand = string.Format(insertCommand,
                    record.DateCheckIn,
                    record.DateCheckOut == null ? null : record.DateCheckOut,
                    record.ReferenceTag);

                dataAdapter.InsertCommand = new SqlCommand(insertCommand, dbConnection.GetConnection);
                dataAdapter.InsertCommand.ExecuteNonQuery();

                return;
            }
            catch (Exception ex)
            {

                //throw;
            }
        }

        public int InsertRecordePermit(ePermitApplicationRecord record)
        {
            ePermitApplicationRecord rec = new ePermitApplicationRecord();
            rec.BookingDate = record.BookingDate;
            rec.HikingSite = record.HikingSite;
            rec.HikingDate = record.HikingDate;
            rec.IDCode = record.IDCode;
            rec.MemberID = record.MemberID;
            rec.Participant = record.Participant;
            rec.TotalNoOfParticipant = record.TotalNoOfParticipant;
            rec.TotalFeesAmount = record.TotalFeesAmount;
            rec.TransactionNo = record.TransactionNo;
            rec.ReceiptNo = record.ReceiptNo;
            rec.ReceiptDate = record.ReceiptDate;
            rec.PaymentApprovalCode = record.PaymentApprovalCode;
            rec.PaymentCompleted = record.PaymentCompleted;
            rec.PaymentMode = record.PaymentMode;
            rec.DateCheckIn = record.DateCheckIn;
            rec.DateCheckOut = record.DateCheckOut;
            rec.ReferenceTag = record.ReferenceTag;
            rec.Status = record.Status = "Trasmitted";

            try
            {
                if (dbConnection.GetConnectionStatus != ConnectionState.Open)
                {
                    dbConnection.OpenConnection();
                }

                Int32 recNo = MaxRecNo("Activities");

                //string sqlDateFormat = record.HikingDate.ToString("MM/dd/yyyy");
                string insertCommand = $"INSERT INTO Activities (RecNo, BookingDate, ReferenceTag, HikingDate, HikingSite, MemberID, Participant, NoOfParticipant, FeesAmount, PaymentCompleted, Status, TransactionNo, ReceiptNo, ReceiptDate, PaymentApprovalCode, IDCode, PaymentMode) " +
                    "VALUES ({0}, CONVERT( datetime, '{1}', 103), '{2}', CONVERT( datetime, '{3}', 103), '{4}', '{5}', '{6}', '{7}', '{8}', {9}, '{10}', {11}, '{12}', CONVERT( datetime, '{13}', 103), '{14}', '{15}', '{16}')";

                insertCommand = string.Format(insertCommand,
                    recNo, 
                    rec.BookingDate, 
                    rec.ReferenceTag, 
                    rec.HikingDate, 
                    rec.HikingSite, 
                    rec.MemberID, 
                    rec.Participant, 
                    rec.TotalNoOfParticipant, 
                    rec.TotalFeesAmount,
                    rec.PaymentCompleted ? 1 : 0, 
                    rec.Status, 
                    rec.TransactionNo, 
                    rec.ReceiptNo, 
                    rec.ReceiptDate, 
                    rec.PaymentApprovalCode,
                    rec.IDCode,
                    rec.PaymentMode);

                dataAdapter.InsertCommand = new SqlCommand(insertCommand, dbConnection.GetConnection);
                dataAdapter.InsertCommand.ExecuteNonQuery();

                return InsertHikingActivities(rec);
            }
            catch (Exception ex)
            {

                return -1;
            }
        }

        private Int32 CalculateYearAge(string yearDOB)
        {
            Int32 yearOfBorn = Convert.ToInt32(yearDOB);

            return DateTime.Now.Year - yearOfBorn;
        }

        public User ValidateWebMember(User user)
        {
            try
            {
                if (dbConnection.GetConnectionStatus != ConnectionState.Open)
                {
                    dbConnection.OpenConnection();
                }

                DataSet ds = new DataSet();

                string transactionCommand = $"SELECT IDCode, Login, Password, NRIC, PhoneNo, MemberName, Gender, Dependent FROM dbo.Members WHERE NRIC = '{user.Login}' OR PhoneNo = '{user.Login}'"; //  AND Login = '{user.Login}' AND Password = '{user.Password}'
                dataAdapter.SelectCommand = new SqlCommand(transactionCommand, dbConnection.GetConnection);
                dataAdapter.Fill(ds);

                if (ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        string idcode = ds.Tables[0].Rows[i].ItemArray[0].ToString();
                        string login = $"{ds.Tables[0].Rows[i].ItemArray[3].ToString()}|{ds.Tables[0].Rows[i].ItemArray[4].ToString()}";
                        string password = ds.Tables[0].Rows[i].ItemArray[2].ToString();
                        int dependent = Convert.ToInt32(ds.Tables[0].Rows[0].ItemArray[7]);

                        if (login.IndexOf(user.Login) > -1 &&
                            user.Password == password)
                        {
                            if (user.personal == null)
                            {
                                user.personal = new PersonalDetail();
                            }

                            user.personal.IDCode = ds.Tables[0].Rows[0].ItemArray[0].ToString();
                            user.personal.NRIC = ds.Tables[0].Rows[i].ItemArray[3].ToString();
                            user.personal.PhoneNo = ds.Tables[0].Rows[i].ItemArray[4].ToString();
                            user.personal.MemberName = ds.Tables[0].Rows[0].ItemArray[5].ToString();
                            user.personal.Gender = ds.Tables[0].Rows[0].ItemArray[6].ToString();
                            user.personal.Dependent = (dependent == 1 ? true : false);

                            user.SignedIn = true;
                            return user;
                        }
                    }
                }

                user.SignedIn = false;
                return user;
            }
            catch (Exception ex)
            {
                user.SignedIn = false;
                return user;
                //throw;
            }
        }

        public User ValidateMember(User user)
        {
            try
            {
                if (dbConnection.GetConnectionStatus != ConnectionState.Open)
                {
                    dbConnection.OpenConnection();
                }

                DataSet ds = new DataSet();

                string transactionCommand = $"SELECT IDCode, Login, Password, NRIC, PhoneNo FROM dbo.Members WHERE IDCode = '{user.personal.IDCode}'"; //  AND Login = '{user.Login}' AND Password = '{user.Password}'
                dataAdapter.SelectCommand = new SqlCommand(transactionCommand, dbConnection.GetConnection);
                dataAdapter.Fill(ds);

                if (ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        string idcode = ds.Tables[0].Rows[i].ItemArray[0].ToString();
                        string login = $"{ds.Tables[0].Rows[i].ItemArray[3].ToString()}|{ds.Tables[0].Rows[i].ItemArray[4].ToString()}";
                        string password = ds.Tables[0].Rows[i].ItemArray[2].ToString();

                        if (user.personal.IDCode == idcode &&
                            user.Login == login &&
                            user.Password == password)
                        {
                            user.SignedIn = true;
                            return user;
                        }
                    }
                }

                user.SignedIn = false;
                return user;
            }
            catch (Exception ex)
            {
                user.SignedIn = false;
                return user;
                //throw;
            }
        }

        public Member InsertNewMember(Member member)
        {
            Int32 recNo = MaxRecNo("dbo.Members");

            Member _member = new Member();

            _member.RecNo = recNo;
            _member.IDCode = (DateTime.Now.Ticks + Convert.ToInt64(member.NRIC)).ToString();
            _member.DateOfJoin = DateTime.Now;
            _member.Password = member.Password;
            _member.MemberName = member.MemberName;
            _member.NRIC = member.NRIC;
            _member.PhoneNo = member.PhoneNo;
            _member.Gender = member.Gender;
            _member.CountryOfOrigin = member.CountryOfOrigin;
            _member.Address1 = member.Address1;
            _member.Address2 = member.Address2;
            _member.PostCode = member.PostCode;
            _member.City = member.City;
            _member.State = member.State;
            _member.EmergencyContact = member.EmergencyContact;
            _member.EmergencyContactNo = member.EmergencyContactNo;
            _member.Relationship = member.Relationship;
            _member.MasterIDCode = member.MasterIDCode;
            _member.Dependent = member.Dependent;
            _member.Status = true;
            _member.DateOfBirth = member.DateOfBirth;

            _member.Age = CalculateYearAge(_member.DateOfBirth.ToString("yyyy"));

            try
            {
                if (dbConnection.GetConnectionStatus != ConnectionState.Open)
                {
                    dbConnection.OpenConnection();
                }

                string insertCommand = $"INSERT INTO Members (RecNo, IDCode, DateOfJoin, Password, MemberName, NRIC, Age, PhoneNo, Gender, Nationality, Address1, Address2, PostCode, City, State, EmergencyContact, EmergencyContactNo, Relationship, Dependent, Status, DateOfBirth, MasterIDCode) " +
                    "VALUES ({0}, '{1}', CONVERT( datetime, '{2}', 103), '{3}', '{4}', '{5}', {6}, '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}', '{14}', '{15}', '{16}', '{17}', '{18}', '{19}', CONVERT( datetime, '{20}', 103), '{21}')";

                insertCommand = string.Format(insertCommand,
                    _member.RecNo,
                    _member.IDCode,
                    _member.DateOfJoin.ToString("dd/MM/yyyy HH:mm:ss"),
                    _member.Password,
                    _member.MemberName,
                    _member.NRIC,
                    _member.Age,
                    _member.PhoneNo,
                    _member.Gender,
                    _member.CountryOfOrigin,
                    _member.Address1,
                    _member.Address2,
                    _member.PostCode,
                    _member.City,
                    _member.State,
                    _member.EmergencyContact,
                    _member.EmergencyContactNo,
                    _member.Relationship,
                    _member.Dependent ? 1 : 0,
                    _member.Status ? 1 : 0,
                    _member.DateOfBirth.ToString("dd/MM/yyyy"),
                    _member.MasterIDCode);

                dataAdapter.InsertCommand = new SqlCommand(insertCommand, dbConnection.GetConnection);
                dataAdapter.InsertCommand.ExecuteNonQuery();

                return _member;
            }
            catch (Exception ex)
            {
                return null;
                //throw;
            }
        }

        public List<DashboardClass> GetAllActivities()
        {
            List<DashboardClass> lstActivities = new List<DashboardClass>();

            try
            {
                if (dbConnection.GetConnectionStatus != ConnectionState.Open)
                {
                    dbConnection.OpenConnection();
                }

                DataSet ds = new DataSet();

                string transactionCommand = $"SELECT HikingDate, FeesAmount FROM dbo.Activities";
                dataAdapter.SelectCommand = new SqlCommand(transactionCommand, dbConnection.GetConnection);
                dataAdapter.Fill(ds);

                if (ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        DashboardClass item = new DashboardClass();

                        item.HikingDate = ds.Tables[0].Rows[i].ItemArray[0].ToString();
                        item.Amount = Convert.ToInt32(ds.Tables[0].Rows[i].ItemArray[1].ToString());

                        lstActivities.Add(item);
                    }

                    return lstActivities;
                }

                return null;
            }
            catch (Exception ex)
            {
                return null;
                //throw;
            }
        }*/
    }
}
