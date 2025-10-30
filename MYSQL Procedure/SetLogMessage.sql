# 로그 작성
# 프로시저 삭제
Drop Procedure SetLogMessages;

# 프로시저 삭제(존재시)
DROP PROCEDURE IF EXISTS SetLogMessages;

# 프로시저 생성
DELIMITER //
CREATE PROCEDURE SetLogMessages(
    IN in_instructorID VARCHAR(255),
    IN in_traineeID VARCHAR(255),
    IN in_logMessage VARCHAR(9999),
    OUT return_state INT
)
BEGIN
    DECLARE CONTINUE HANDLER FOR SQLEXCEPTION
    BEGIN
        SET return_state = 0;
    END;

    -- 기본 쿼리
	INSERT INTO `lllegaldrone`.`logdb`
	(`InstructorID`,
	`TraineeID`,
	`LogMessage`,
	`LogWriteDateTime`)
	VALUES
	(in_instructorID,
	in_traineeID,
	in_logMessage,
	Now());
    
    SET return_state = 1;
END //
DELIMITER ;

# 프로시저 실행
CALL SetLogMessages("123456789", "111111111", "테스트로그2", @return_state);
Select @return_state;

SELECT * FROM lllegaldrone.logdb;

INSERT INTO `lllegaldrone`.`logdb`
(`InstructorID`,
`TraineeID`,
`LogMessage`,
`LogWriteDateTime`)
VALUES
("123456789",
"000000000",
"테스트 로그",
NOW());