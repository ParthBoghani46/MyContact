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

        public async Task<List<Status>> GetStatusList()
        {
            DataTable dt = new DataTable();
            List<Status> statusList = new List<Status>();
            try
            {
                using (NpgsqlCommand cm = new NpgsqlCommand("select * from t_status", _conn))
                {

                    _conn.Close();
                    _conn.Open();
                    NpgsqlDataReader datar = cm.ExecuteReader();
                    if (datar.HasRows)
                    {
                        dt.Load(datar);
                    }


                    statusList = (from DataRow dr in dt.Rows
                                  select new Status()
                                  {
                                      statusId = Convert.ToInt32(dr["c_statusid"]),
                                      status = dr["c_statusname"].ToString()

                                  }).ToList();
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("Error status :" + ex.Message);
            }
            _conn.Close();
            return statusList;
        }

        public async Task<int> Add(t_Contact contactData)
        {
            try
            {
                var userId = _httpContextAccessor.HttpContext.Session.GetInt32("UserId");
                using (NpgsqlCommand cm = new NpgsqlCommand(@"INSERT INTO t_contacts 
                ( c_userid, c_contactname, c_email, c_address, c_mobile, c_group, c_image, c_status) 
                VALUES 
                (@c_userid, @c_contactname, @c_email, @c_address, @c_mobile, @c_group, @c_image, @c_status)", _conn))
                {


                    cm.Parameters.AddWithValue("@c_userid", userId);
                    cm.Parameters.AddWithValue("@c_contactname", contactData.c_contactName);
                    cm.Parameters.AddWithValue("@c_email", contactData.c_Email);
                    cm.Parameters.AddWithValue("@c_address", contactData.c_Address);
                    cm.Parameters.AddWithValue("@c_mobile", contactData.c_Mobile);
                    cm.Parameters.AddWithValue("@c_group", contactData.c_Group);
                    cm.Parameters.AddWithValue("@c_image", contactData.c_Image == null ? DBNull.Value : contactData.c_Image);
                    cm.Parameters.AddWithValue("@c_status", contactData.c_Status);

                    _conn.Close();
                    _conn.Open();
                    cm.ExecuteNonQuery();
                    _conn.Close();
                }
                return 1;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return 0;
            }
        }

        public async Task<int> Delete(string contactid)
        {
            try
            {
                using (NpgsqlCommand cm = new NpgsqlCommand(@"DELETE FROM t_contacts WHERE c_contactid = @c_contactid", _conn))
                {


                    cm.Parameters.AddWithValue("@c_contactid", int.Parse(contactid));
                    _conn.Close();
                    _conn.Open();
                    cm.ExecuteNonQuery();
                    _conn.Close();
                }
                return 1;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return 0;
            }
        }

        public async Task<List<t_Contact>> GetAll()
        {

            DataTable dt = new DataTable();
            List<t_Contact> contactList = new List<t_Contact>();
            using (NpgsqlCommand cm = new NpgsqlCommand("select * from t_contacts where c_userid=@userId order by c_contactid", _conn))
            {

                _conn.Close();
                _conn.Open();
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
                                   c_Status = (int)dr["c_status"]

                               }).ToList();
            }
            _conn.Close();
            return contactList;

        }

        public async Task<List<ContactListViewModel>> GetAllByUser(string userid)
        {
            DataTable dt = new DataTable();
            List<ContactListViewModel> contactList = new List<ContactListViewModel>();
            using (NpgsqlCommand cm = new NpgsqlCommand("select * from t_contacts join t_status on t_contacts.c_status = t_status.c_statusid where c_userid=@userId", _conn))
            {
                cm.Parameters.AddWithValue("@userId", Int32.Parse(userid));
                _conn.Close();
                _conn.Open();

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
                                   },
                                   c_StatusName = dr["c_statusname"].ToString()

                               }).ToList();
            }
            _conn.Close();
            return contactList;
        }

        public async Task<t_Contact> GetOne(string contactid)
        {
            DataTable dt = new DataTable();
            t_Contact contact = null;
            using (NpgsqlCommand cm = new NpgsqlCommand("select * from t_contacts WHERE c_contactid=@c_contactid", _conn))
            {
                cm.Parameters.AddWithValue("@c_contactid", Convert.ToInt32(contactid));
                _conn.Close();
                _conn.Open();
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
                        c_Status = (int)datar["c_status"]
                    };
                }
            }
            _conn.Close();
            return contact;
        }

        public async Task<int> Update(t_Contact contactData)
        {
            try
            {
                string oldImagePath = null;

                // Fetch the old image path
                using (NpgsqlCommand getOldImageCmd = new NpgsqlCommand("SELECT c_image FROM t_contacts WHERE c_contactid = @c_contactid", _conn))
                {
                    getOldImageCmd.Parameters.AddWithValue("@c_contactid", contactData.c_contactId);

                    _conn.Open();
                    object result = getOldImageCmd.ExecuteScalar();
                    _conn.Close();

                    if (result != null && result != DBNull.Value)
                    {
                        oldImagePath = result.ToString();
                    }
                }

                // Delete the old image file if it exists
                if (!string.IsNullOrEmpty(oldImagePath))
                {
                    string fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "contact_images", oldImagePath);

                    if (File.Exists(fullPath))
                    {
                        File.Delete(fullPath);
                    }
                }

                // Now proceed with updating the record
                using (NpgsqlCommand cm = new NpgsqlCommand(@"UPDATE t_contacts 
            SET 
                c_userid = @c_userid, 
                c_contactname = @c_contactname, 
                c_email = @c_email, 
                c_address = @c_address, 
                c_mobile = @c_mobile, 
                c_group = @c_group, 
                c_image = @c_image, 
                c_status = @c_status 
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
                    cm.Parameters.AddWithValue("@c_contactid", contactData.c_contactId);

                    _conn.Open();
                    cm.ExecuteNonQuery();
                    _conn.Close();
                }

                return 1;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return 0;
            }
        }

    }
}