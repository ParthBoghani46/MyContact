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
            t_User userData = new t_User();
            var query = "SELECT * FROM t_users WHERE c_userid=@c_userid;";
            try
            {
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, _conn))
                {
                    cmd.Parameters.AddWithValue("@c_userid", userid);
                    await _conn.OpenAsync();
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
                await _conn.CloseAsync();
            }
            return userData;
        }

        public async Task<t_User> Login(vm_Login user)
        {
            t_User userData = new t_User();
            var query = "SELECT * FROM t_users WHERE c_email=@c_email AND c_password = @c_password;";
            try
            {
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, _conn))
                {
                    cmd.Parameters.AddWithValue("@c_email", user.c_Email);
                    cmd.Parameters.AddWithValue("@c_password", user.c_Password);
                    await _conn.OpenAsync();
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
                Console.WriteLine("Login Error : " + e.Message);
            }
            finally
            {
                await _conn.CloseAsync();
            }
            return userData;
        }



        public async Task<int> Register(t_User userData)
        {
            int status = 0;
            try
            {
                await _conn.CloseAsync();
                NpgsqlCommand comcheck = new NpgsqlCommand(@"SELECT * FROM t_Users WHERE c_email = @c_email ;", _conn);
                comcheck.Parameters.AddWithValue("@c_email", userData.c_Email);
                await _conn.OpenAsync();
                using (NpgsqlDataReader datadr = await comcheck.ExecuteReaderAsync())
                {
                    if (datadr.HasRows)
                    {
                        await _conn.CloseAsync();
                        return 0; //Returning 0 if user exists
                    }
                    else
                    {

                        await _conn.CloseAsync();
                        using (NpgsqlCommand cm = new NpgsqlCommand(@"INSERT INTO t_users
                (c_username, c_email, c_password,c_address, c_mobile, c_gender, c_image) 
                VALUES 
                (@c_username, @c_email, @c_password,@c_address,@c_mobile,@c_gender,@c_image)", _conn))
                        {


                            cm.Parameters.AddWithValue("@c_username", userData.c_userName);
                            cm.Parameters.AddWithValue("@c_email", userData.c_Email);
                            cm.Parameters.AddWithValue("@c_password", userData.c_Password);
                            cm.Parameters.AddWithValue("@c_address", userData.c_Address);
                            cm.Parameters.AddWithValue("@c_mobile", userData.c_Mobile);
                            cm.Parameters.AddWithValue("@c_gender", userData.c_Gender);
                            cm.Parameters.AddWithValue("@c_image", userData.c_Image == null ? DBNull.Value : userData.c_Image);

                            _conn.Open();
                            cm.ExecuteNonQuery();
                            _conn.Close();
                            return 1;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                await _conn.CloseAsync();
                Console.WriteLine("Register failed , erroe : " + ex.Message);
                return -1;
            }
        }

    }
}