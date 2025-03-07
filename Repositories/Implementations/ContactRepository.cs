using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using MyContact.Models;
using Npgsql;
using Microsoft.AspNetCore.Mvc;

namespace MyContact.Repositories
{
    public class ContactRepository : IContactInterface
    {
        private readonly NpgsqlConnection _conn;
        private readonly IHttpContextAccessor _httpContextAccessor;



        public ContactRepository(NpgsqlConnection connection, IHttpContextAccessor httpContextAccessor)
        {
            _conn = connection;
            _httpContextAccessor = httpContextAccessor;
        }


        public async Task<List<State>> GetState()
        {
            List<State> stateList = new List<State>();

            try
            {
                if (_conn.State == System.Data.ConnectionState.Closed)
                {
                    await _conn.OpenAsync();
                }

                using (var cm = new NpgsqlCommand("SELECT * FROM t_states", _conn))
                using (var datar = await cm.ExecuteReaderAsync()) // ✅ Async reader
                {
                    while (await datar.ReadAsync()) // ✅ Async read
                    {
                        stateList.Add(new State
                        {
                            stateId = datar.GetInt32(datar.GetOrdinal("c_stateid")),
                            stateName = datar.GetString(datar.GetOrdinal("c_statename"))
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error State: " + ex.Message);
            }
            finally
            {
                if (_conn.State == System.Data.ConnectionState.Open)
                {
                    await _conn.CloseAsync();
                }
            }

            return stateList;
        }

        public async Task<List<City>> GetCity(int stateid)
        {
            List<City> cityList = new List<City>();

            try
            {
                if (_conn.State == System.Data.ConnectionState.Closed)
                {
                    await _conn.OpenAsync();
                }

                using (var cm = new NpgsqlCommand("SELECT * FROM t_cities WHERE c_stateid = @stateid", _conn))
                {
                    cm.Parameters.AddWithValue("@stateid", stateid);

                    using (var datar = await cm.ExecuteReaderAsync()) // ✅ Async reader
                    {
                        while (await datar.ReadAsync()) // ✅ Async read
                        {
                            cityList.Add(new City
                            {
                                cityId = datar.GetInt32(datar.GetOrdinal("c_cityid")),
                                cityName = datar.GetString(datar.GetOrdinal("c_cityname")),
                                stateId = datar.GetInt32(datar.GetOrdinal("c_stateid"))
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error City: " + ex.Message);
            }
            finally
            {
                if (_conn.State == System.Data.ConnectionState.Open)
                {
                    await _conn.CloseAsync();
                }
            }

            return cityList;
        }

        public async Task<List<Status>> GetStatusList()
        {
            List<Status> statusList = new List<Status>();

            try
            {
                if (_conn.State == System.Data.ConnectionState.Closed)
                {
                    await _conn.OpenAsync();
                }

                using (var cm = new NpgsqlCommand("SELECT * FROM t_status", _conn))
                using (var datar = await cm.ExecuteReaderAsync()) // ✅ Async reader
                {
                    while (await datar.ReadAsync()) // ✅ Async read
                    {
                        statusList.Add(new Status
                        {
                            statusId = datar.GetInt32(datar.GetOrdinal("c_statusid")),
                            status = datar.GetString(datar.GetOrdinal("c_statusname"))
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error Status: " + ex.Message);
            }
            finally
            {
                if (_conn.State == System.Data.ConnectionState.Open)
                {
                    await _conn.CloseAsync();
                }
            }

            return statusList;
        }


        public async Task<int> Add(t_Contact contactData)
        {
            if (_conn.State == System.Data.ConnectionState.Closed)
            {
                await _conn.OpenAsync();
            }
            try
            {
                var userId = _httpContextAccessor.HttpContext.Session.GetInt32("UserId");
                using (NpgsqlCommand cm = new NpgsqlCommand(@"INSERT INTO t_contacts 
                ( c_userid, c_contactname, c_email, c_address, c_mobile, c_group, c_image, c_status,c_stateid,c_cityid) 
                VALUES 
                (@c_userid, @c_contactname, @c_email, @c_address, @c_mobile, @c_group, @c_image, @c_status,@c_stateid,@c_cityid)", _conn))
                {


                    cm.Parameters.AddWithValue("@c_userid", userId);
                    cm.Parameters.AddWithValue("@c_contactname", contactData.c_contactName);
                    cm.Parameters.AddWithValue("@c_email", contactData.c_Email);
                    cm.Parameters.AddWithValue("@c_address", contactData.c_Address);
                    cm.Parameters.AddWithValue("@c_mobile", contactData.c_Mobile);
                    cm.Parameters.AddWithValue("@c_group", contactData.c_Group);
                    cm.Parameters.AddWithValue("@c_image", contactData.c_Image == null ? DBNull.Value : contactData.c_Image);
                    cm.Parameters.AddWithValue("@c_status", contactData.c_Status);
                    cm.Parameters.AddWithValue("@c_stateid", contactData.c_stateid);
                    cm.Parameters.AddWithValue("@c_cityid", contactData.c_cityid);

                    cm.ExecuteNonQuery();

                }
                return 1;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);

            }
            finally
            {
                if (_conn.State == System.Data.ConnectionState.Open)
                {
                    await _conn.CloseAsync();
                }
            }
            return 0;

        }

        public async Task<int> Delete(string contactid)
        {
            if (_conn.State == System.Data.ConnectionState.Closed)
            {
                await _conn.OpenAsync();
            }
            try
            {
                using (NpgsqlCommand cm = new NpgsqlCommand(@"DELETE FROM t_contacts WHERE c_contactid = @c_contactid", _conn))
                {
                    cm.Parameters.AddWithValue("@c_contactid", int.Parse(contactid));

                    cm.ExecuteNonQuery();
                }
                return 1;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);

            }
            finally
            {
                if (_conn.State == System.Data.ConnectionState.Open)
                {
                    await _conn.CloseAsync();
                }
            }
            return 0;
        }

        public async Task<List<t_Contact>> GetAll()
        {
            if (_conn.State == System.Data.ConnectionState.Closed)
            {
                await _conn.OpenAsync();
            }

            DataTable dt = new DataTable();
            List<t_Contact> contactList = new List<t_Contact>();
            await using (NpgsqlCommand cm = new NpgsqlCommand("select * from t_contacts where c_userid=@userId order by c_contactid", _conn))
            {

                NpgsqlDataReader datar = cm.ExecuteReader();
                if (datar.HasRows)
                {
                    dt.Load(datar);
                }


                contactList = (from DataRow dr in dt.Rows
                               select new t_Contact()
                               {
                                   c_contactId = Convert.ToInt32(dr["c_contactid"]),
                                   c_userId = int.Parse(dr["c_userid"].ToString()),
                                   c_contactName = dr["c_Contactname"].ToString(),
                                   c_Email = dr["c_email"].ToString(),
                                   c_Mobile = dr["c_mobile"].ToString(),
                                   c_Address = dr["c_address"].ToString(),
                                   c_Image = dr["c_image"].ToString(),
                                   c_Group = dr["c_group"].ToString(),
                                   c_Status = (int)dr["c_status"],
                                   c_stateid = (int)dr["c_stateid"],
                                   c_cityid = (int)dr["c_cityid"]

                               }).ToList();
            }
            if (_conn.State == System.Data.ConnectionState.Open)
            {
                await _conn.CloseAsync();
            }
            return contactList;

        }

        public async Task<List<ContactListViewModel>> GetAllByUser(string userid)
        {
            if (_conn.State == System.Data.ConnectionState.Closed)
            {
                await _conn.OpenAsync();
            }
            DataTable dt = new DataTable();
            List<ContactListViewModel> contactList = new List<ContactListViewModel>();
            string query = @"SELECT 
                                t_contacts.*, 
                                t_status.c_statusname, 
                                t_Cities.c_cityname, 
                                t_States.c_statename
                            FROM t_contacts
                            JOIN t_status ON t_contacts.c_status = t_status.c_statusid
                            JOIN t_Cities ON t_contacts.c_cityid = t_Cities.c_cityid
                            JOIN t_States ON t_Cities.c_stateid = t_States.c_stateid
                            WHERE t_contacts.c_userid = @userId order by t_contacts.c_contactid;";

            await using (NpgsqlCommand cm = new NpgsqlCommand(query, _conn))
            {
                cm.Parameters.AddWithValue("@userId", Int32.Parse(userid));


                NpgsqlDataReader datar = cm.ExecuteReader();
                if (datar.HasRows)
                {
                    dt.Load(datar);
                }


                contactList = (from DataRow dr in dt.Rows
                               where dr["c_userid"].ToString() == userid
                               select new ContactListViewModel()
                               {
                                   contact = new t_Contact()
                                   {
                                       c_contactId = Convert.ToInt32(dr["c_contactid"]),
                                       c_userId = int.Parse(dr["c_userid"].ToString()),
                                       c_contactName = dr["c_Contactname"].ToString(),
                                       c_Email = dr["c_email"].ToString(),
                                       c_Mobile = dr["c_mobile"].ToString(),
                                       c_Address = dr["c_address"].ToString(),
                                       c_Image = dr["c_image"].ToString(),
                                       c_Group = dr["c_group"].ToString(),
                                       c_Status = (int)dr["c_status"],
                                       c_stateid = (int)dr["c_stateid"],
                                       c_cityid = (int)dr["c_cityid"]
                                   },
                                   c_StatusName = dr["c_statusname"].ToString(),
                                   c_cityname = dr["c_cityname"].ToString(),
                                   c_statename = dr["c_statename"].ToString()

                               }).ToList();
            }
            if (_conn.State == System.Data.ConnectionState.Open)
            {
                await _conn.CloseAsync();
            }
            return contactList;
        }

        public async Task<t_Contact> GetOne(string contactid)
        {
            DataTable dt = new DataTable();
            t_Contact contact = null;
            if (_conn.State == System.Data.ConnectionState.Closed)
            {
                await _conn.OpenAsync();
            }
            await using (NpgsqlCommand cm = new NpgsqlCommand("select * from t_contacts WHERE c_contactid=@c_contactid", _conn))
            {
                cm.Parameters.AddWithValue("@c_contactid", Convert.ToInt32(contactid));

                NpgsqlDataReader datar = cm.ExecuteReader();
                if (datar.Read())
                {
                    contact = new t_Contact()
                    {
                        c_contactId = Convert.ToInt32(datar["c_contactid"]),
                        c_userId = int.Parse(datar["c_userid"].ToString()),
                        c_contactName = datar["c_Contactname"].ToString(),
                        c_Email = datar["c_email"].ToString(),
                        c_Mobile = datar["c_mobile"].ToString(),
                        c_Address = datar["c_address"].ToString(),
                        c_Image = datar["c_image"].ToString(),
                        c_Group = datar["c_group"].ToString(),
                        c_Status = (int)datar["c_status"],
                        c_stateid = (int)datar["c_stateid"],
                        c_cityid = (int)datar["c_cityid"]
                    };
                }
            }
            if (_conn.State == System.Data.ConnectionState.Open)
            {
                await _conn.CloseAsync();
            }
            return contact;
        }

        public async Task<int> Update(t_Contact contactData)
        {
            if (_conn.State == System.Data.ConnectionState.Closed)
            {
                await _conn.OpenAsync();
            }
            try
            {
                // Update the contact record
                await using (NpgsqlCommand cm = new NpgsqlCommand(@"
            UPDATE t_contacts 
            SET 
                c_userid = @c_userid, 
                c_contactname = @c_contactname, 
                c_email = @c_email, 
                c_address = @c_address, 
                c_mobile = @c_mobile, 
                c_group = @c_group, 
                c_image = @c_image, 
                c_status = @c_status, 
                c_stateid = @c_stateid,
                c_cityid = @c_cityid
            WHERE 
                c_contactid = @c_contactid", _conn))
                {
                    cm.Parameters.AddWithValue("@c_userid", contactData.c_userId);
                    cm.Parameters.AddWithValue("@c_contactname", contactData.c_contactName);
                    cm.Parameters.AddWithValue("@c_email", contactData.c_Email);
                    cm.Parameters.AddWithValue("@c_address", contactData.c_Address);
                    cm.Parameters.AddWithValue("@c_mobile", contactData.c_Mobile);
                    cm.Parameters.AddWithValue("@c_group", contactData.c_Group);
                    cm.Parameters.AddWithValue("@c_image", contactData.c_Image ?? (object)DBNull.Value);
                    cm.Parameters.AddWithValue("@c_status", contactData.c_Status);
                    cm.Parameters.AddWithValue("@c_stateid", contactData.c_stateid);
                    cm.Parameters.AddWithValue("@c_cityid", contactData.c_cityid);
                    cm.Parameters.AddWithValue("@c_contactid", contactData.c_contactId);

                    cm.ExecuteNonQuery();

                }
                if (_conn.State == System.Data.ConnectionState.Open)
                {
                    await _conn.CloseAsync();
                }
                return 1;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return 0;
            }
            finally
            {
                if (_conn.State == System.Data.ConnectionState.Open)
                {
                    await _conn.CloseAsync();
                }
            }
        }
    }
}