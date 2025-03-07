using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyContact.Models;
using Npgsql;

namespace MyContact.Repositories.Implementations
{
    public class UserRepository : IUserInterface
    {
        private readonly NpgsqlConnection _conn;
        public UserRepository(NpgsqlConnection connection)
        {
            _conn = connection;
        }

        public async Task<t_User> GetUser(int userid)
        {
            if (_conn.State == System.Data.ConnectionState.Closed)
            {
                await _conn.OpenAsync();
            }
            t_User userData = new t_User();
            var query = "SELECT * FROM t_users WHERE c_userid=@c_userid;";
            try
            {
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, _conn))
                {
                    cmd.Parameters.AddWithValue("@c_userid", userid);
                    var reader = await cmd.ExecuteReaderAsync();
                    if (reader.Read())
                    {
                        userData.c_userId = (int)reader["c_userid"];
                        userData.c_userName = (string)reader["c_username"];
                        userData.c_Email = (string)reader["c_email"];
                        userData.c_Gender = (string)reader["c_gender"];
                        userData.c_Mobile = (string)reader["c_mobile"];
                        userData.c_Address = (string)reader["c_address"];
                        userData.c_Image = (string)reader["c_image"];
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("GetUser Error : " + e.Message);
            }
            finally
            {
                if (_conn.State == System.Data.ConnectionState.Open)
                {
                    await _conn.CloseAsync();
                }
            }
            return userData;
        }

        public async Task<t_User> Login(Login user)
        {
            if (_conn.State == System.Data.ConnectionState.Closed)
            {
                await _conn.OpenAsync();
            }
            t_User userData = null; // Change from 'new t_User()' to 'null'

            var query = "SELECT * FROM t_users WHERE c_email=@c_email AND c_password=@c_password;";
            try
            {

                using (NpgsqlCommand cmd = new NpgsqlCommand(query, _conn))
                {
                    cmd.Parameters.AddWithValue("@c_email", user.c_Email);
                    cmd.Parameters.AddWithValue("@c_password", user.c_Password);

                    var reader = await cmd.ExecuteReaderAsync();
                    if (reader.Read())
                    {
                        userData = new t_User // Create an object only if user exists
                        {
                            c_userId = (int)reader["c_userid"],
                            c_userName = (string)reader["c_username"],
                            c_Email = (string)reader["c_email"],
                            c_Gender = (string)reader["c_gender"],
                            c_Mobile = (string)reader["c_mobile"],
                            c_Address = (string)reader["c_address"],
                            c_Image = reader["c_image"] != DBNull.Value ? (string)reader["c_image"] : null
                        };
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Login Error: " + e.Message);
            }
            finally
            {
                if (_conn.State == System.Data.ConnectionState.Open)
                {
                    await _conn.CloseAsync();
                }
            }

            return userData; // Return null if user is not found
        }


        public async Task<int> Register(t_User userData)
        {
            if (_conn.State == System.Data.ConnectionState.Closed)
            {
                await _conn.OpenAsync();
            }
            int status = 0;
            try
            {

                using (NpgsqlCommand comcheck = new NpgsqlCommand(@"SELECT COUNT(*) FROM t_Users WHERE c_email = @c_email;", _conn))
                {
                    comcheck.Parameters.AddWithValue("@c_email", userData.c_Email);
                    int count = Convert.ToInt32(await comcheck.ExecuteScalarAsync());

                    if (count > 0)
                    {
                        await _conn.CloseAsync();
                        return 0; // User already exists
                    }
                }

                using (NpgsqlCommand cm = new NpgsqlCommand(@"INSERT INTO t_users 
            (c_username, c_email, c_password, c_address, c_mobile, c_gender, c_image) 
            VALUES (@c_username, @c_email, @c_password, @c_address, @c_mobile, @c_gender, @c_image);", _conn))
                {
                    cm.Parameters.AddWithValue("@c_username", userData.c_userName);
                    cm.Parameters.AddWithValue("@c_email", userData.c_Email);
                    cm.Parameters.AddWithValue("@c_password", userData.c_Password);
                    cm.Parameters.AddWithValue("@c_address", userData.c_Address);
                    cm.Parameters.AddWithValue("@c_mobile", userData.c_Mobile);
                    cm.Parameters.AddWithValue("@c_gender", userData.c_Gender);
                    cm.Parameters.AddWithValue("@c_image", userData.c_Image ?? (object)DBNull.Value);

                    await cm.ExecuteNonQueryAsync();
                }

                status = 1; // Registration successful
            }
            catch (Exception ex)
            {
                Console.WriteLine("Register failed, error: " + ex.Message);
                status = -1;
            }
            finally
            {
                if (_conn.State == System.Data.ConnectionState.Open)
                {
                    await _conn.CloseAsync();
                }
            }
            return status;
        }


    }
}