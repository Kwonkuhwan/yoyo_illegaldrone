# 유저 패스워드 수정
# 프로시저 삭제
Drop Procedure UserPWModify;

# 프로시저 삭제(존재시)
DROP PROCEDURE IF EXISTS UserPWModify;

# 프로시저 생성
DELIMITER //
CREATE PROCEDURE UserPWModify(
    IN in_id VARCHAR(255), 
    IN in_pw VARCHAR(255),
    Out return_state INT
)
BEGIN
	DECLARE CONTINUE HANDLER FOR SQLEXCEPTION
    BEGIN
        SET return_state = 0;
    END;

-- 사용자의 정보를 입력 한다.
	UPDATE `lllegaldrone`.`userdb`
	SET
	`PW` = in_pw
	WHERE `ID` = in_id;
    
    SET return_state = 1;
END //
DELIMITER ;

# 프로시저 실행
CALL UserPWModify('12', '001100', @return_state);
Select @return_state;