# 새로운 유저 추가
# 프로시저 삭제
Drop Procedure AddUser;

# 프로시저 삭제(존재시)
DROP PROCEDURE IF EXISTS AddUser;

# 프로시저 생성
DELIMITER //
CREATE PROCEDURE AddUser(
	IN in_usergroup INT,
    IN in_id VARCHAR(255), 
    IN in_pw VARCHAR(255), 
    IN in_username VARCHAR(255), 
    IN in_gender INT,
    IN in_creatorName VARCHAR(255),
    Out return_state INT
)
BEGIN
	DECLARE CONTINUE HANDLER FOR SQLEXCEPTION
    BEGIN
        SET return_state = 0;
    END;

-- 사용자의 정보를 입력 한다.
	INSERT INTO `lllegaldrone`.`userdb`
	(`Usergroup`, `ID`,	`PW`, `UserName`, `Gender`,
	`CreateIDDate`, `DeleteIDDate`,	`EndAccessDate`,
	`TotalPlayTime`, `Scenariocount`, `CreateAdminName`)
	VALUES
	(in_usergroup, in_id, in_pw, in_username, in_gender,
	NOW(), '9999-12-31 12:59:59', '1999-01-01 00:00:00',
	0, 0, in_creatorName);
    
    SET return_state = 1;
END //
DELIMITER ;

# 프로시저 실행
CALL AddUser(2, 'yoyo5678', '000000', '요요_훈련생', 1, '권구환', @return_state);
Select @return_state;

INSERT INTO `lllegaldrone`.`userdb`
(`Usergroup`,`ID`,`PW`,`UserName`,`Gender`,
`CreateIDDate`,`DeleteIDDate`,`EndAccessDate`,
`TotalPlayTime`,`Scenariocount`)
VALUES
(1,'333333333','111111','오정윤', 0,
NOW(),
'9999-12-31 12:59:59',
'1999-01-01 00:00:00',
0,0,
'권구환');