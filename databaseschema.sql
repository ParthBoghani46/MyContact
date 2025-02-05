CREATE TABLE t_contacts (
c_contactid INT PRIMARY KEY,
c_userid INT,
c_contactname VARCHAR(100),
c_email VARCHAR(100),
c_address VARCHAR(500),
c_mobile VARCHAR(50),
c_group VARCHAR(50),
c_image VARCHAR(4000),
c_status VARCHAR(20)
)

CREATE TABLE t_users(
c_userid INT PRIMARY KEY,
c_username VARCHAR(100),
c_email VARCHAR(100),
c_password VARCHAR(100),
c_address VARCHAR(500),
c_mobile VARCHAR(50),
c_gender VARCHAR(10),
c_image VARCHAR(4000)
)
