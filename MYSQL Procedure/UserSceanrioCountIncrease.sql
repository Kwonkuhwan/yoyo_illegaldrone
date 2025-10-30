# 훈련생 시나리오 카운트 증가
# 프로시저 삭제
Drop Procedure UserSceanrioCountIncrease;

# 프로시저 삭제(존재시)
DROP PROCEDURE IF EXISTS UserSceanrioCountIncrease;

# 프로시저 생성
DELIMITER //
CREATE PROCEDURE UserSceanrioCountIncrease(
    IN in_id VARCHAR(255),
    Out return_state INT
)
BEGIN
	DECLARE CONTINUE HANDLER FOR SQLEXCEPTION
    BEGIN
        SET return_state = 0;
    END;

	UPDATE `lllegaldrone`.`userdb`
	SET
	Scenariocount =  Scenariocount + 1
	WHERE `ID` = in_id;
    
    SET return_state = 1;
END //
DELIMITER ;

# 프로시저 실행
CALL UserSceanrioCountIncrease('123456789', @return_state);
Select @return_state;

UPDATE `lllegaldrone`.`userdb`
SET
Scenariocount =  Scenariocount + 1
WHERE `ID` = '123456789';
