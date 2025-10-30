# 훈련생 훈련 성공률
# 프로시저 삭제
Drop Procedure GetTraineeTraingParcent;

# 프로시저 삭제(존재시)
DROP PROCEDURE IF EXISTS GetTraineeTraingParcent;

# 프로시저 생성
DELIMITER //
CREATE PROCEDURE GetTraineeTraingParcent(
    IN in_id VARCHAR(255),
    OUT return_percent DOUBLE,
    OUT return_state INT
)
BEGIN
    -- 변수 선언
    DECLARE total_count INT;
    DECLARE success_count INT;

    -- 예외 발생 시 처리
    DECLARE CONTINUE HANDLER FOR SQLEXCEPTION
    BEGIN
        SET return_state = 0;  -- 실패 상태
        SET return_percent = -1;  -- 실패 시 비율
    END;

    -- 전체 미션 수 계산
    SELECT COUNT(*) INTO total_count
    FROM lllegaldrone.playdb
    WHERE UserID = in_id;

    -- 성공한 미션 수 계산
    SELECT SUM(CASE WHEN TeamMissionresult = 1 THEN 1 ELSE 0 END) INTO success_count
    FROM lllegaldrone.playdb
    WHERE UserID = in_id;

    -- 비율 계산 및 소수점 2자리로 반올림
    IF total_count > 0 THEN
        SET return_percent = ROUND((success_count * 100.0 / total_count), 2);
    ELSE
        SET return_percent = 0;  -- 미션이 없을 경우 비율 0
    END IF;


    SET return_state = 1;  -- 성공 상태
END //

DELIMITER ;

# 프로시저 실행
CALL GetTraineeTraingParcent('yoyo5678', @return_count, @return_state);
CALL GetTraineeTraingParcent("yoyo5678", @return_count, @return_state);
Select @return_count;
Select @return_count;

SELECT 
    COUNT(*) as cnt, 
    SUM(CASE WHEN TeamMissionresult = 1 THEN 1 ELSE 0 END) as mcnt,
    (SUM(CASE WHEN TeamMissionresult = 1 THEN 1 ELSE 0 END) * 100.0 / COUNT(*)) as percentage
FROM 
    lllegaldrone.playdb
WHERE 
    UserID = "yoyo5678";