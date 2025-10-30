# 관리자, 훈련생 pc 검출
# 프로시저 삭제
Drop Procedure CheckInstructorPC;

# 프로시저 삭제(존재시)
DROP PROCEDURE IF EXISTS CheckInstructorPC;

# 프로시저 생성
DELIMITER //
CREATE PROCEDURE CheckInstructorPC(
	IN in_ip VARCHAR(255),
    Out return_count BOOL,
    Out return_state INT
)
BEGIN
    DECLARE cnt INT DEFAULT 0;
    
	DECLARE CONTINUE HANDLER FOR SQLEXCEPTION
    BEGIN
        SET return_state = 0;
    END;
    
-- 사용자의 정보를 입력 한다.
	Select COUNT(*) into cnt 
    from `lllegaldrone`.`instructordb` 
    where ip = in_ip;    
    
	IF cnt > 0 THEN
        SET return_count = TRUE;  -- true로 설정
    ELSE
        SET return_count = FALSE;  -- false로 설정
    END IF;
    
    SET return_state = 1;
END //
DELIMITER ;

# 프로시저 실행
CALL CheckInstructorPC("192.168.1.14", @return_count, @return_state);
CALL CheckInstructorPC('192.168.1.14', @return_count, @return_state);
Select @return_count;
Select @return_state;

#Select * from `lllegaldrone`.`instructordb` where ip = "192.168.1.14";
SELECT COUNT(*) as cnt FROM `lllegaldrone`.`instructordb` WHERE ip = "192.168.1.14";