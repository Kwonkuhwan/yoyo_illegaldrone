select * from mysql.user;

CREATE USER 'lllegalDrone'@'%' IDENTIFIED BY '1234';
GRANT ALL PRIVILEGES ON *.* TO 'lllegalDrone'@'%' WITH GRANT OPTION;
FLUSH PRIVILEGES;