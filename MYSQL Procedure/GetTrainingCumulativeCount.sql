# 누적 훈련 수
# 프로시저 삭제
Drop Procedure GetTrainingCumulativeCount;

# 프로시저 삭제(존재시)
DROP PROCEDURE IF EXISTS GetTrainingCumulativeCount;

# 프로시저 생성
DELIMITER //
CREATE PROCEDURE GetTrainingCumulativeCount(
    IN in_creator varchar(255),
	IN in_issearch BOOL,
    OUT return_count INT,
    OUT return_state INT
)
BEGIN
    DECLARE CONTINUE HANDLER FOR SQLEXCEPTION
    BEGIN
        SET return_state = 0;
    END;

    -- 기본 쿼리
    SET @order := CASE WHEN in_asc THEN 'ASC' ELSE 'DESC' END;
	
    IF in_issearch THEN
		SELECT 
			COUNT(DISTINCT s.ScenarioID) INTO return_count
		FROM 
			lllegaldrone.playdb p
		INNER JOIN 
			lllegaldrone.scenariodb s ON p.ScenarioID = s.ScenarioID
        WHERE s.CreateUser = in_creator;
    	
    ELSE
		SELECT 
			COUNT(DISTINCT s.ScenarioID) INTO return_count
		FROM 
			lllegaldrone.playdb p
		INNER JOIN 
			lllegaldrone.scenariodb s ON p.ScenarioID = s.ScenarioID;
    
    END IF;
    
    SET return_state = 1;
END //
DELIMITER ;

# 프로시저 실행
CALL GetTrainingCumulativeCount("", false, @return_count, @return_state);
Select @return_count;
Select @return_state;

SELECT 
    COUNT(DISTINCT s.ScenarioID) as return_count
FROM 
    lllegaldrone.playdb p
INNER JOIN 
    lllegaldrone.scenariodb s ON p.ScenarioID = s.ScenarioID;
