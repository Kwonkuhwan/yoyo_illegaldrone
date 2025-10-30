SELECT * FROM lllegaldrone.playdb;

# 캘린더 데이터 불러오기
# 프로시저 삭제
Drop Procedure GetCalendarData;

# 프로시저 삭제(존재시)
DROP PROCEDURE IF EXISTS GetCalendarData;

# 프로시저 생성
DELIMITER //
CREATE PROCEDURE GetCalendarData(
    IN in_searchDate VARCHAR(255),    
    OUT return_count INT,
	OUT return_msg VARCHAR(255),
    OUT return_state INT
)
BEGIN    
	DECLARE play_count INT;

	DECLARE CONTINUE HANDLER FOR SQLEXCEPTION
    BEGIN
        SET return_state = 0;
    END;

    -- 플레이 데이터 수를 계산합니다.
	SELECT COUNT(DISTINCT p.PlayScenarioDateTime) INTO play_count
	FROM lllegaldrone.playdb p
	WHERE DATE(p.PlayScenarioDateTime) = in_searchDate;

    IF play_count > 0 THEN        
		-- 플레이 데이터를 가져온다.
          WITH RankedData AS (
			SELECT 
				p.Missionresult, 
				p.PlayScenarioTime, 
				p.PlayScenarioDateTime, 
				s.CreateScenarioDateTime, 
                s.PlayingNumber,
				s.PlayMode, 
				s.MissionMap,
				ROW_NUMBER() OVER (PARTITION BY p.PlayScenarioDateTime ORDER BY p.PlayScenarioTime DESC) as rn
			FROM 
				lllegaldrone.playdb p
			INNER JOIN 
				lllegaldrone.scenariodb s
				ON s.ScenarioID = p.ScenarioID
			WHERE 
				DATE(p.PlayScenarioDateTime) = in_searchDate
		)
		SELECT 
			Missionresult, 
			PlayScenarioTime, 
			PlayScenarioDateTime, 
			CreateScenarioDateTime, 
            PlayingNumber,
			PlayMode, 
			MissionMap
		FROM 
			RankedData
		WHERE 
			rn = 1;
        
		SET return_msg = '훈련 이력이 있습니다.';  -- 메시지 설정
        SET return_count = play_count;
	ELSE
		SET return_msg = '훈련 이력이 없습니다.';        
        SET return_count = play_count;
	END IF;
    
    SET return_state = 1;
END //
DELIMITER ;

# 프로시저 실행
CALL GetCalendarData('2025-04-25', @return_count, @return_msg, @return_state);
Select @return_msg;
Select @return_state;
Select @return_count;

SELECT COUNT(DISTINCT p.PlayScenarioDateTime) as cnt
FROM lllegaldrone.playdb p
WHERE DATE(p.PlayScenarioDateTime) = '2025-04-25';

SELECT DISTINCT
    p.Missionresult, 
    p.PlayScenarioTime, 
    p.PlayScenarioDateTime, 
    s.CreateScenarioDateTime, 
    s.PlayMode, 
    s.MissionMap
FROM 
    lllegaldrone.playdb p
INNER JOIN 
    lllegaldrone.scenariodb s
    ON s.ScenarioID = p.ScenarioID
WHERE 
    DATE(p.PlayScenarioDateTime) = '2025-04-25';
    
    
    WITH RankedData AS (
    SELECT 
        p.Missionresult, 
        p.PlayScenarioTime, 
        p.PlayScenarioDateTime, 
        s.CreateScenarioDateTime, 
        s.PlayMode, 
        s.MissionMap,
        ROW_NUMBER() OVER (PARTITION BY p.PlayScenarioDateTime ORDER BY p.PlayScenarioTime DESC) as rn
    FROM 
        lllegaldrone.playdb p
    INNER JOIN 
        lllegaldrone.scenariodb s
        ON s.ScenarioID = p.ScenarioID
    WHERE 
        DATE(p.PlayScenarioDateTime) = '2025-04-25'
)
SELECT 
    Missionresult, 
    PlayScenarioTime, 
    PlayScenarioDateTime, 
    CreateScenarioDateTime, 
    PlayMode, 
    MissionMap
FROM 
    RankedData
WHERE 
    rn = 1;