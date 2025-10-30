# 유저 삭제
# 프로시저 삭제
Drop Procedure UserDelete;

# 프로시저 삭제(존재시)
DROP PROCEDURE IF EXISTS UserDelete;

# 프로시저 생성
DELIMITER //
CREATE PROCEDURE UserDelete(
    IN in_id VARCHAR(255),
    OUT return_state INT
)
BEGIN    
	DECLARE CONTINUE HANDLER FOR SQLEXCEPTION
    BEGIN
        SET return_state = 0;
    END;

    -- 플레어를 삭제한다.
	UPDATE `lllegaldrone`.`userdb`
	SET
	`DeleteIDDate` = Now()
	WHERE `ID` = in_id;
    
    SET return_state = 1;
END //
DELIMITER ;

# 프로시저 실행
CALL UserDelete('123456789', @return_state);
Select @return_state;
