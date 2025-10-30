# 누적 훈련생 수
# 프로시저 삭제
Drop Procedure GetUsersCumulativeCount;

# 프로시저 삭제(존재시)
DROP PROCEDURE IF EXISTS GetUsersCumulativeCount;

# 프로시저 생성
DELIMITER //
CREATE PROCEDURE GetUsersCumulativeCount(
	IN in_name VARCHAR(255),
    IN in_usergroup INT,
    OUT return_count INT,
    OUT return_state INT
)
BEGIN
	DECLARE cnt INT;

    -- 예외 발생 시 처리
    DECLARE CONTINUE HANDLER FOR SQLEXCEPTION
    BEGIN
        SET return_state = 0;  -- 실패 상태
        SET return_count = -1;  -- 실패 카운트
    END;

    -- 훈련 누적 카운트 계산
    IF in_usergroup = 1 THEN
        SELECT COUNT(*) INTO cnt
        FROM lllegaldrone.userdb
        WHERE usergroup IN (0, 1) AND UserName LIKE CONCAT('%', in_name, '%');
    ELSEIF in_usergroup = 2 THEN
        SELECT COUNT(*) INTO cnt
        FROM lllegaldrone.userdb
        WHERE usergroup = 2 AND UserName LIKE CONCAT('%', in_name, '%');
    ELSE
        SET cnt = 0;  -- 다른 경우에는 0으로 설정
    END IF;

    SET return_state = 1;  -- 성공 상태
    SET return_count = cnt;  -- 성공 카운트
END //

DELIMITER ;

# 프로시저 실행
CALL GetUsersCumulativeCount("", 2, @return_count, @return_state);
Select @return_state;
Select @return_count;


SELECT COUNT(*) as cnt
FROM lllegaldrone.userdb
where usergroup = 2;