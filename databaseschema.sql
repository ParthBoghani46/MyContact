CREATE TABLE t_contacts (
c_contactid INT PRIMARY KEY,
c_userid INT,
c_contactname VARCHAR(100),
c_email VARCHAR(100),
c_address VARCHAR(500),
c_mobile VARCHAR(50),
c_group VARCHAR(50),
c_image VARCHAR(4000),
c_status INT
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


CREATE TABLE t_status(
c_statusid int primary key,
c_statusname varchar(255)
);


INSERT INTO t_status (c_statusid, c_statusname) VALUES
(1, 'Active'),
(2, 'Inactive'),
(3, 'Favourite');


CREATE TABLE t_States (
    c_stateid SERIAL PRIMARY KEY,
    c_statename VARCHAR(100) NOT NULL UNIQUE
);

CREATE TABLE t_Cities (
    c_cityid SERIAL PRIMARY KEY,
    c_cityname VARCHAR(100) NOT NULL,
    c_stateid INT NOT NULL,
    CONSTRAINT fk_state FOREIGN KEY (c_stateid) REFERENCES t_States(c_stateid) ON DELETE CASCADE
);

INSERT INTO t_States (c_statename) VALUES 
('Gujarat'), 
('Maharashtra'), 
('Karnataka');

-- Gujarat (State ID: 1)
INSERT INTO t_Cities (c_cityname, c_stateid) VALUES 
('Ahmedabad', 1), 
('Surat', 1), 
('Vadodara', 1), 
('Rajkot', 1), 
('Gandhinagar', 1);

-- Maharashtra (State ID: 2)
INSERT INTO t_Cities (c_cityname, c_stateid) VALUES 
('Mumbai', 2), 
('Pune', 2), 
('Nagpur', 2), 
('Nashik', 2), 
('Aurangabad', 2);

-- Karnataka (State ID: 3)
INSERT INTO t_Cities (c_cityname, c_stateid) VALUES 
('Bangalore', 3), 
('Mysore', 3), 
('Mangalore', 3), 
('Hubli', 3), 
('Belgaum', 3);

